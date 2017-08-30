#if (!UNITY_FLASH && !UNITY_WP8 && !UNITY_METRO)

using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace EventDelegates {

	/// <summary>
	/// Delegate system that Unity can serialize and set via Inspector.
	/// </summary>
	[System.Serializable]
	public class EventDelegateEx
	{
		[Serializable]
		private class ParamInfo
		{
			public string mType;
			public string mValue;
		};
		
		public class ParamValue
		{
			public System.Type mType;
			public object mValue;
			public string mName;
		};
		
		[SerializeField] MonoBehaviour mTarget;
		[SerializeField] string mMethodName;
		[SerializeField] ParamInfo[] mSerializedParameters;
		[SerializeField] string[] mParameterNames;

		/// <summary>
		/// Whether the event delegate will be removed after execution.
		/// </summary>
		public bool oneShot = false;

		private MethodInfo mCachedMethod = null;
		private ParamValue[] mRealParameters = null;

		private static Dictionary<System.Type, TypeConverter> mCachedTypeConverters = new Dictionary<System.Type, TypeConverter>();
		
		/// <summary>
		/// Event delegate's target object.
		/// </summary>
		public MonoBehaviour target
		{
			get
			{
				return mTarget;
			}
			set
			{
				mTarget = value;
				mCachedMethod = null;
			}
		}

		/// <summary>
		/// Event delegate's method
		/// </summary>
		public string methodName
		{
			get
			{
				return mMethodName;
			}
			set
			{
				mMethodName = value;
				mCachedMethod = null;
			}
		}

		/// <summary>
		/// Event delegate's real parameter values
		/// </summary>
		public ParamValue[] parameterValues
		{
			get
			{
				int length = (mSerializedParameters != null) ? mSerializedParameters.Length : 0;
				
				// Check if the real parameters are up to date
				bool valid = false;
				if ((mRealParameters != null) && (mRealParameters.Length == length))
				{
					valid = true;
					for (int paramIndex = 0; paramIndex < length; ++paramIndex)
					{
						if (mRealParameters[paramIndex] == null)
						{
							valid = false;
							break;
						}
						if (mRealParameters[paramIndex].mType == null)
						{
							valid = false;
							break;
						}
					}
				}

				// Re-create parameters from serialized values if required
				if (!valid)
				{
					mRealParameters = new ParamValue[length];
					for (int paramIndex = 0; paramIndex < length; ++paramIndex)
					{
						mRealParameters[paramIndex] = new ParamValue();
						mRealParameters[paramIndex].mType = Type.GetType(mSerializedParameters[paramIndex].mType);

						// Cache type converters, since these are expensive to look up
						TypeConverter typeConverter = null;
						if (!mCachedTypeConverters.TryGetValue(mRealParameters[paramIndex].mType, out typeConverter))
						{
							typeConverter = TypeDescriptor.GetConverter(mRealParameters[paramIndex].mType);
							mCachedTypeConverters[mRealParameters[paramIndex].mType] = typeConverter;
						}
						
						mRealParameters[paramIndex].mValue = typeConverter.ConvertFromString(mSerializedParameters[paramIndex].mValue);
						mRealParameters[paramIndex].mName = ((mParameterNames != null) && (paramIndex < mParameterNames.Length)) ? mParameterNames[paramIndex] : null;
					}
				}
				
				// Return the real parameter data
				return mRealParameters;
			}
			set
			{
				mRealParameters = value;
				mCachedMethod = null;

				// Construct an array of serialized object information from the current parameters
				mSerializedParameters = new ParamInfo[value.Length];
				for (int paramIndex = 0; paramIndex < value.Length; ++paramIndex)
				{
					mSerializedParameters[paramIndex] = new ParamInfo();
					mSerializedParameters[paramIndex].mType = value[paramIndex].mType.AssemblyQualifiedName;
					mSerializedParameters[paramIndex].mValue = ((value[paramIndex].mValue != null) && !value[paramIndex].mValue.Equals(null)) ? value[paramIndex].mValue.ToString() : String.Empty;
				}
			}
		}
		
		/// <summary>
		/// Event delegate's real parameter names
		/// </summary>
		public string[] parameterNames
		{
			get
			{
				// Return the parameter names
				return mParameterNames;
			}
			set
			{
				// Set the parameter names
				mParameterNames = value;

				// Update the real parameter names if required
				int length = (mRealParameters != null) ? mRealParameters.Length : 0;
				for (int paramIndex = 0; paramIndex < length; ++paramIndex)
					mRealParameters[paramIndex].mName = ((mParameterNames != null) && (paramIndex < mParameterNames.Length)) ? mParameterNames[paramIndex] : null;
			}
		}
		
		/// <summary>
		/// Whether this delegate's values have been set.
		/// </summary>
		public bool isValid
		{
			get
			{
				// Check if there is a target and method
				if ((mTarget == null) || string.IsNullOrEmpty(mMethodName))
					return false;

				// Check if the method with our parameter types is present
				UpdateCachedMethod(parameterValues);
				if (mCachedMethod == null)
					return false;

				return true;
			}
		}

		/// <summary>
		/// Whether the target script is actually enabled.
		/// </summary>
		public bool isEnabled
		{
			get
			{
				return (mTarget != null) && mTarget.enabled;
			}
		}

		/// <summary>
		/// Default constructor.
		/// </summary>
		public EventDelegateEx()
		{
		}

		/// <summary>
		/// Constructor from a method, target and value information.
		/// </summary>
		public EventDelegateEx(MethodInfo methodInfo, MonoBehaviour target, params ParamValue[] values)
		{
			if (methodInfo == null)
				return;

			ParameterInfo[] methodParams = methodInfo.GetParameters();
			if (values.Length != methodParams.Length)
				return;
			for (int paramIndex = 0; paramIndex < methodParams.Length; ++paramIndex)
			{
				if (values[paramIndex].mType != methodParams[paramIndex].ParameterType)
					return;
			}

			mTarget = target;
			mMethodName = methodInfo.Name;
			mCachedMethod = methodInfo;
			parameterValues = values;
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		public EventDelegateEx(EventDelegateEx del)
		{
			mTarget = del.mTarget;
			mMethodName = del.mMethodName;
			mCachedMethod = del.mCachedMethod;
			parameterValues = del.parameterValues;
		}

		/// <summary>
		/// Equality operator.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj == null)
				return !isValid;

			if (obj is EventDelegateEx)
			{
				EventDelegateEx del = obj as EventDelegateEx;
				if (mTarget != del.mTarget)
					return false;
				if ((mMethodName != del.mMethodName))
					return false;
				ParamValue[] values = parameterValues;
				ParamValue[] delValues = del.parameterValues;
				if (values.Length != delValues.Length)
					return false;
				for (int paramIndex = 0; paramIndex < values.Length; ++paramIndex)
				{
					if (values[paramIndex].mType != delValues[paramIndex].mType)
						return false;
					if (values[paramIndex].mValue != delValues[paramIndex].mValue)
						return false;
				}

				return true;
			}

			return false;
		}

		/// <summary>
		/// Equality operator.
		/// </summary>
		public bool Equals(EventDelegateEx del)
		{
			if (del == null)
				return !isValid;

			if (mTarget != del.mTarget)
				return false;
			if ((mMethodName != del.mMethodName))
				return false;
			ParamValue[] values = parameterValues;
			ParamValue[] delValues = del.parameterValues;
			if (values.Length != delValues.Length)
				return false;
			for (int paramIndex = 0; paramIndex < values.Length; ++paramIndex)
			{
				if (values[paramIndex].mType != delValues[paramIndex].mType)
					return false;
				if (values[paramIndex].mValue != delValues[paramIndex].mValue)
					return false;
			}

			return true;
		}

		
		private static int s_Hash = "EventDelegateEx".GetHashCode();

		/// <summary>
		/// Used in equality operators.
		/// </summary>
		public override int GetHashCode()
		{
			return s_Hash;
		}

		// Ensures the cached method is valid if possible
		private void UpdateCachedMethod(ParamValue[] values)
		{
			if (mCachedMethod != null)
			{
				ParameterInfo[] methodParams = mCachedMethod.GetParameters();
				if (values.Length != methodParams.Length)
				{
					mCachedMethod = null;
				}
				else
				{
					for (int paramIndex = 0; paramIndex < methodParams.Length; ++paramIndex)
					{
						if (values[paramIndex].mType != methodParams[paramIndex].ParameterType)
						{
							mCachedMethod = null;
							break;
						}
					}
				}
			}
			
			if (mCachedMethod == null)
			{
				if ((mTarget != null) && !string.IsNullOrEmpty(mMethodName))
				{
					System.Type[] paramTypes = new System.Type[values.Length];
					for (int paramIndex = 0; paramIndex < values.Length; ++paramIndex)
						paramTypes[paramIndex] = values[paramIndex].mType;
					mCachedMethod = mTarget.GetType().GetMethod(mMethodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, paramTypes, null);
				}
			}
		}
		
		/// <summary>
		/// Execute the delegate, if possible.
		/// This will only be used when the application is playing in order to prevent unintentional state changes.
		/// </summary>
		public bool Execute()
		{
	#if UNITY_EDITOR
			if (Application.isPlaying)
	#endif
			{
				ParamValue[] values = parameterValues;
				UpdateCachedMethod(values);
				if (mCachedMethod != null)
				{
					object[] realValues = new object[values.Length];
					for (int valueIndex = 0; valueIndex < values.Length; ++valueIndex)
						realValues[valueIndex] = values[valueIndex].mValue;
					mCachedMethod.Invoke(mTarget, realValues);
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Prepare for execution of the delegate, but don't execute
		/// This will only be used when the application is playing in order to prevent unintentional state changes.
		/// </summary>
		public void PreCacheExecute()
		{
	#if UNITY_EDITOR
			if (Application.isPlaying)
	#endif
			{
				ParamValue[] values = parameterValues;
				UpdateCachedMethod(values);
			}
		}

		/// <summary>
		/// Convert the parameters to its string representation.
		/// </summary>
		public static string GetDescription(MonoBehaviour _target, string _methodName, ParamValue[] _parameterValues)
		{
			if ((_target == null) || string.IsNullOrEmpty(_methodName))
				return null;

			string typeName = _target.GetType().ToString();
			int period = typeName.LastIndexOf('.');
			if (period > 0)
				typeName = typeName.Substring(period + 1);
			
			// Get the basic function type
			StringBuilder sb = new StringBuilder();
			sb.Append(typeName + "." + _methodName);

			// Get the parameters for this delegate
			sb.Append("(");
			for (int paramIndex = 0; paramIndex < _parameterValues.Length; ++paramIndex)
			{
				if (paramIndex > 0)
					sb.Append(", ");
				sb.Append(_parameterValues[paramIndex].mType.FullName.Replace("+", "."));
			}
			sb.Append(")");

			return sb.ToString();
		}
		
		/// <summary>
		/// Convert the parameters to its string representation.
		/// </summary>
		public static string GetDescription(MonoBehaviour _target, string _methodName, ParamValue[] _parameterValues, string[] _parameterNames)
		{
			if ((_target == null) || string.IsNullOrEmpty(_methodName))
				return null;

			string typeName = _target.GetType().ToString();
			int period = typeName.LastIndexOf('.');
			if (period > 0)
				typeName = typeName.Substring(period + 1);
			
			// Get the basic function type
			StringBuilder sb = new StringBuilder();
			sb.Append(typeName + "." + _methodName);

			// Get the parameters for this delegate
			sb.Append("(");
			for (int paramIndex = 0; paramIndex < _parameterValues.Length; ++paramIndex)
			{
				if (paramIndex > 0)
					sb.Append(", ");
				sb.Append(_parameterValues[paramIndex].mType.FullName.Replace("+", "."));
				if ((_parameterNames != null) && (paramIndex < _parameterNames.Length))
				{
					sb.Append(" ");
					sb.Append(_parameterNames[paramIndex]);
				}
				else if (_parameterValues[paramIndex].mName != null)
				{
					sb.Append(" ");
					sb.Append(_parameterValues[paramIndex].mName);
				}
			}
			sb.Append(")");

			return sb.ToString();
		}
		
		/// <summary>
		/// Convert the delegate to its string representation.
		/// </summary>
		public string GetDescription(bool withParamNames)
		{
			if (withParamNames)
				return GetDescription(mTarget, mMethodName, parameterValues, null);
			else
				return GetDescription(mTarget, mMethodName, parameterValues);
		}

		/// <summary>
		/// Convert the delegate to its string representation.
		/// </summary>
		public override string ToString()
		{
			return GetDescription(true);
		}

		/// <summary>
		/// Execute an entire list of delegates.
		/// </summary>
		public static void Execute(List<EventDelegateEx> list)
		{
			if (list == null)
				return;

			for (int i = 0; i < list.Count;)
			{
				EventDelegateEx del = list[i];
				if (del != null)
				{
					del.Execute();
					if (del.oneShot)
					{
						list.RemoveAt(i);
						continue;
					}
				}
				++i;
			}
		}

		/// <summary>
		/// Prepare for execution a list of delegates, but don't execute
		/// This will only be used when the application is playing in order to prevent unintentional state changes.
		/// </summary>
		public static void PreCacheExecute(List<EventDelegateEx> list)
		{
			if (list == null)
				return;

			for (int i = 0; i < list.Count; ++i)
			{
				EventDelegateEx del = list[i];
				if (del != null)
					del.PreCacheExecute();
			}
		}
		
		/// <summary>
		/// Convenience function to check if the specified list of delegates can be executed.
		/// </summary>
		public static bool IsValid(List<EventDelegateEx> list)
		{
			if (list != null)
			{
				for (int i = 0, imax = list.Count; i < imax; ++i)
				{
					EventDelegateEx del = list[i];
					if ((del != null) && del.isValid)
						return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Append a new event delegate to the list.
		/// </summary>
		public static void Add(List<EventDelegateEx> list, MethodInfo methodInfo, MonoBehaviour target, params ParamValue[] values)
		{
			Add(list, false, methodInfo, target, values);
		}

		/// <summary>
		/// Append a new event delegate to the list.
		/// </summary>
		public static void Add(List<EventDelegateEx> list, bool oneShot, MethodInfo methodInfo, MonoBehaviour target, params ParamValue[] values)
		{
			if (list != null)
			{
				for (int i = 0, imax = list.Count; i < imax; ++i)
				{
					EventDelegateEx del = list[i];
					if ((del != null) && (del.methodName == methodInfo.Name) && (del.target == target) && del.parameterValues.Equals(values))
						return;
				}

				EventDelegateEx ed = new EventDelegateEx(methodInfo, target, values);
				ed.oneShot = oneShot;
				list.Add(ed);
			}
			else
			{
				UnityEngine.Debug.LogWarning("Attempting to add a delegate to a list that's null");
			}
		}

		/// <summary>
		/// Append a new event delegate to the list.
		/// </summary>
		public static void Add(List<EventDelegateEx> list, EventDelegateEx ev)
		{
			Add(list, false, ev);
		}

		/// <summary>
		/// Append a new event delegate to the list.
		/// </summary>
		public static void Add(List<EventDelegateEx> list, bool oneShot, EventDelegateEx ev)
		{
			if (list != null)
			{
				for (int i = 0, imax = list.Count; i < imax; ++i)
				{
					EventDelegateEx del = list[i];
					if ((del != null) && del.Equals(ev))
						return;
				}

				EventDelegateEx ed = new EventDelegateEx(ev);
				ed.oneShot = oneShot;
				list.Add(ed);
			}
			else
			{
				UnityEngine.Debug.LogWarning("Attempting to add a delegate to a list that's null");
			}
		}

		/// <summary>
		/// Remove an existing event delegate from the list.
		/// </summary>
		public static bool Remove(List<EventDelegateEx> list, MethodInfo methodInfo, MonoBehaviour target, params ParamValue[] values)
		{
			if (list != null)
			{
				for (int i = 0, imax = list.Count; i < imax; ++i)
				{
					EventDelegateEx del = list[i];
					if ((del != null) && (del.methodName == methodInfo.Name) && (del.target == target) && del.parameterValues.Equals(values))
					{
						list.RemoveAt(i);
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Check if an existing event delegate is in the list.
		/// </summary>
		public static bool Contains(List<EventDelegateEx> list, MethodInfo methodInfo, MonoBehaviour target, params ParamValue[] values)
		{
			if (list != null)
			{
				for (int i = 0, imax = list.Count; i < imax; ++i)
				{
					EventDelegateEx del = list[i];
					if ((del != null) && (del.methodName == methodInfo.Name) && (del.target == target) && del.parameterValues.Equals(values))
						return true;
				}
			}
			return false;
		}

	}

}	// namespace EventDelegates

#endif
