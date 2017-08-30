using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace EventDelegates {

	// Delegate callback entry
	[System.Serializable]
	public class FunctionDelegate
	{
		public MonoBehaviour m_target;
		public string m_methodName;

		public delegate bool Callback();
		private Callback m_cachedCallback;


		public FunctionDelegate()
		{
		}

		public FunctionDelegate(Callback call)
		{
			if ((call == null) || (call.Method == null))
			{
				m_target = null;
				m_methodName = null;
				m_cachedCallback = null;
			}
			else
			{
				m_target = call.Target as MonoBehaviour;
				m_methodName = call.Method.Name;
			}
		}

		public FunctionDelegate(MonoBehaviour target, string methodName)
		{
			m_target = target;
			m_methodName = methodName;
			m_cachedCallback = null;
		}

		// Function delegate's target object.
		public MonoBehaviour target
		{
			get
			{
				return m_target;
			}
			set
			{
				m_target = value;
				m_cachedCallback = null;
			}
		}

		// Function delegate's method name.
		public string methodName
		{
			get
			{
				return m_methodName;
			}
			set
			{
				m_methodName = value;
				m_cachedCallback = null;
			}
		}

		// Whether this delegate's values have been set.
		public bool isValid
		{
			get
			{
				return (m_target != null) && !string.IsNullOrEmpty(m_methodName);
			}
		}

		// Whether the target script is actually enabled.
		public bool isEnabled
		{
			get
			{
				return (m_target != null) && m_target.enabled;
			}
		}

		// Equality operator.
		public override bool Equals(object obj)
		{
			if (obj == null)
				return !isValid;

			if (obj is Callback)
			{
				Callback callback = obj as Callback;
				return ((m_target == (MonoBehaviour)callback.Target) && (m_methodName == callback.Method.Name));
			}
			
			if (obj is FunctionDelegate)
			{
				FunctionDelegate del = obj as FunctionDelegate;
				return ((m_target == del.m_target) && (m_methodName == del.m_methodName));
			}

			return false;
		}

		// Equality operator.
		public bool Equals(Callback callback)
		{
			if (callback == null)
				return !isValid;

			return ((m_target == (MonoBehaviour)callback.Target) && (m_methodName == callback.Method.Name));
		}

		// Equality operator.
		public bool Equals(FunctionDelegate del)
		{
			if (del == null)
				return !isValid;

			return ((m_target == del.m_target) && (m_methodName == del.m_methodName));
		}

		// Used in equality operators.
		private static int ms_hashCode = "FunctionDelegate".GetHashCode();
		public override int GetHashCode()
		{
			return ms_hashCode;
		}

		// Execute the delegate, if possible.
		// This will only be used when the application is playing in order to prevent unintentional state changes.
		public bool Execute()
		{
#if UNITY_EDITOR
			if (Application.isPlaying)
#endif
			{
				// Update the cached callback from the target and method name
				if ((m_cachedCallback == null) || ((MonoBehaviour)m_cachedCallback.Target != m_target) || (m_cachedCallback.Method.Name != m_methodName))
				{
					if ((m_target == null) || string.IsNullOrEmpty(m_methodName))
						m_cachedCallback = null;
					else
						m_cachedCallback = (Callback)System.Delegate.CreateDelegate(typeof(Callback), m_target, m_methodName);
				}
				
				// Call the callback if available
				if (m_cachedCallback != null)
					return m_cachedCallback();
			}
			
			// Assume the function returned false
			return false;
		}

		// Convert the delegate to its string representation.
		public override string ToString()
		{
			if ((m_target != null) && !string.IsNullOrEmpty(methodName))
			{
				string typeName = m_target.GetType().ToString();
				int period = typeName.LastIndexOf('.');
				if (period > 0)
					typeName = typeName.Substring(period + 1);
				return typeName + "." + methodName;
			}

			return null;
		}

	};
		
	// Delegate callback that Unity can serialize and set via Inspector.
	[System.Serializable]
	public class FunctionDelegateList
	{
		public enum OperatorType
		{
			And,
			Or,
		};
		
		[SerializeField]
		public List<FunctionDelegate> m_functions = new List<FunctionDelegate>();
		[SerializeField]
		public OperatorType m_operator;

		// Execute a function delegate list
		static public bool Execute(FunctionDelegateList list)
		{
			if (list == null)
				return false;

			bool value = true;
			foreach (FunctionDelegate del in list.m_functions)
			{
				bool result = (del != null) ? del.Execute() : false;
				switch (list.m_operator)
				{
				case OperatorType.And:
					value = value && result;
					break;
				case OperatorType.Or:
					value = value || result;
					break;
				}
			}
			return value;
		}

		// Convenience function to check if the specified function delegate list can be executed.
		static public bool IsValid(FunctionDelegateList list)
		{
			if (list == null)
				return false;

			foreach (FunctionDelegate del in list.m_functions)
			{
				if ((del == null) || !del.isValid)
					return false;
			}

			return true;
		}

		// Assign a singular new function delegate.
		static public void SetFunction(FunctionDelegateList list, FunctionDelegate.Callback callback)
		{
			if (list == null)
				return;

			list.m_functions.Clear();
			list.m_functions.Add(new FunctionDelegate(callback));
		}

		// Append a new function delegate to the list.
		static public void AddFunction(FunctionDelegateList list, FunctionDelegate.Callback callback)
		{
			if (list == null)
			{
				UnityEngine.Debug.LogWarning("Attempting to add a callback to a list that's null");
				return;
			}
			
			// If already exists, do nothing
			foreach (FunctionDelegate del in list.m_functions)
			{
				if ((del != null) && del.Equals(callback))
					return;
			}
			
			// Create a new delegate and add to the list
			FunctionDelegate fd = new FunctionDelegate(callback);
			list.m_functions.Add(fd);
		}

		// Append a new function delegate to the list.
		static public void AddFunction(FunctionDelegateList list, FunctionDelegate func)
		{
			if (list == null)
			{
				UnityEngine.Debug.LogWarning("Attempting to add a delegate to a list that's null");
				return;
			}

			// If already exists, do nothing
			foreach (FunctionDelegate del in list.m_functions)
			{
				if ((del != null) && del.Equals(func))
					return;
			}

			// Create a new delegate and add to the list
			FunctionDelegate fd = new FunctionDelegate(func.m_target, func.m_methodName);
			list.m_functions.Add(fd);
		}

		// Remove an existing function delegate from the list.
		static public bool RemoveFunction(FunctionDelegateList list, FunctionDelegate.Callback callback)
		{
			if (list == null)
				return false;

			for (int i = 0, imax = list.m_functions.Count; i < imax; ++i)
			{
				FunctionDelegate del = list.m_functions[i];
				if ((del != null) && del.Equals(callback))
				{
					list.m_functions.RemoveAt(i);
					return true;
				}
			}

			return false;
		}

		// Check if an existing function delegate is in the list.
		static public bool Contains(FunctionDelegateList list, FunctionDelegate.Callback callback)
		{
			if (list == null)
				return false;

			foreach (FunctionDelegate del in list.m_functions)
			{
				if ((del != null) && del.Equals(callback))
					return true;
			}

			return false;
		}
	}

}	// namespace EventDelegates
