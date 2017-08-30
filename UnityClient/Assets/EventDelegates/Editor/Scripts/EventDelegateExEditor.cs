#if (!UNITY_FLASH && !UNITY_WP8 && !UNITY_METRO)

using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Text;
using System.Collections.Generic;

namespace EventDelegates {

	public static class EventDelegateExEditor
	{
		class Entry
		{
			public MonoBehaviour target;
			public MethodInfo method;
			public EventDelegateEx.ParamValue[] parameterValues;
			public string[] parameterNames;
		}

		/// <summary>
		/// Collect a list of usable delegates from the specified target game object.
		/// The delegates must be of type "void Delegate()".
		/// </summary>
		private static List<Entry> GetMethods(GameObject target)
		{
			List<Entry> list = new List<Entry>();

			MonoBehaviour[] comps = target.GetComponents<MonoBehaviour>();
			for (int i = 0, imax = comps.Length; i < imax; ++i)
			{
				MonoBehaviour mb = comps[i];
				if (mb == null)
					continue;

				MethodInfo[] methods = mb.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
				for (int b = 0; b < methods.Length; ++b)
				{
					MethodInfo mi = methods[b];

					// Ignore methods that return values
					if (mi.ReturnType != typeof(void))
						continue;
					
					// Ignore known built-in methods that we don't want to call
					List<string> ignoreMethods = new List<string> {
						"StopCoroutine",
						"StopAllCoroutines",
						"CancelInvoke",
						"Invoke",
						"InvokeRepeating",
						"set_useGUILayout",
						"set_enabled",
						"set_active",
						"set_tag",
						"set_name",
						"set_hideFlags",
						"set_runInEditMode",
						"SendMessageUpwards",
						"SendMessage",
						"BroadcastMessage",
						"Finalize",
					};
					if (ignoreMethods.Contains(mi.Name))
						continue;

					// Check if this is a compiler generated method (eg, anonymous delegate/action), and if so, ignore it
					object[] compilerGeneratedAttributes = mi.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false);
					if (compilerGeneratedAttributes.Length > 0)
						continue;

					// Ignore methods that don't contain only allowed types
					ParameterInfo[] methodParams = mi.GetParameters();
					bool matches = true;
					List<System.Type> allowedTypes = new List<System.Type> {
						typeof(System.Int32),
						typeof(System.Single),
						typeof(System.Boolean),
						typeof(System.String),
						typeof(System.Enum),
					};
					for (int paramIndex = 0; paramIndex < methodParams.Length; ++paramIndex)
					{
						ParameterInfo param = methodParams[paramIndex];
						bool match = false;
						foreach (System.Type type in allowedTypes)
						{
							if (type.IsAssignableFrom(param.ParameterType))
							{
								match = true;
								break;
							}
						}
						if (!match)
						{
							matches = false;
							break;
						}
					}
					if (!matches)
						continue;

					// Construct a new method entry
					Entry ent = new Entry();
					ent.target = mb;
					ent.method = mi;
					EventDelegateEx.ParamValue[] parameterValues = new EventDelegateEx.ParamValue[methodParams.Length];
					string[] parameterNames = new string[methodParams.Length];
					for (int paramIndex = 0; paramIndex < methodParams.Length; ++paramIndex)
					{
						System.Type paramType = methodParams[paramIndex].ParameterType;
						parameterValues[paramIndex] = new EventDelegateEx.ParamValue();
						parameterValues[paramIndex].mType = paramType;
						parameterValues[paramIndex].mValue = paramType.IsValueType ? Activator.CreateInstance(paramType) : null;
						parameterNames[paramIndex] = methodParams[paramIndex].Name;
					}
					ent.parameterValues = parameterValues;
					ent.parameterNames = parameterNames;

					// Add this method
					list.Add(ent);
				}
			}

			return list;
		}

		/// <summary>
		/// Convert the specified list of delegate entries into a string array.
		/// </summary>
		private static string[] GetMethodNames(List<Entry> list, EventDelegateEx del, out int selectedIndex, out int listOffset, out bool found)
		{
			// Get the two description strings for the current delegate
			string selectedTextWithoutNames = del.GetDescription(false);
			string selectedTextWithNames = del.GetDescription(true);
			bool textDifferent = (selectedTextWithoutNames != selectedTextWithNames);

			// Default to the first selection until we know otherwise
			selectedIndex = 0;
			listOffset = 0;

			// Scan to see if the method exists
			found = false;
			for (int methodIndex = 0; methodIndex < list.Count; ++methodIndex)
			{
				Entry entry = list[methodIndex];

				// Check if this method is the selected one
				string entryTextWithoutNames = EventDelegateEx.GetDescription(entry.target, entry.method.Name, entry.parameterValues);
				if (entryTextWithoutNames == selectedTextWithoutNames)
				{
					found = true;
					break;
				}
			}

			List<string> nameList = new List<string>();

			// If no selection, add a new option at the top
			if (string.IsNullOrEmpty(selectedTextWithNames) || !found)
			{
				nameList.Add("<Choose>");
				listOffset = -1;
			}

			for (int methodIndex = 0; methodIndex < list.Count; ++methodIndex)
			{
				Entry entry = list[methodIndex];

				// Get the description strings for this entry
				string entryTextWithoutNames = EventDelegateEx.GetDescription(entry.target, entry.method.Name, entry.parameterValues);
				string entryTextWithNames = EventDelegateEx.GetDescription(entry.target, entry.method.Name, entry.parameterValues, entry.parameterNames);

				// Check if this method is the selected one
				bool isCurrent = (entryTextWithoutNames == selectedTextWithoutNames);

				// Set the selected index if this is the current one
				if (isCurrent)
					selectedIndex = methodIndex;

				// Add the method to the list
				nameList.Add(entryTextWithNames);

				// Check if we need to update the parameter names
				if (isCurrent && !textDifferent && (entryTextWithoutNames != entryTextWithNames))
					del.parameterNames = entry.parameterNames;
			}
			
			return nameList.ToArray();
		}

		/// <summary>
		/// Draw an editor field for the Unity Delegate.
		/// </summary>
		public static void Field(UnityEngine.Object undoObject, EventDelegateEx del)
		{
			Field(undoObject, del, true);
		}

		/// <summary>
		/// Draw an editor field for the Unity Delegate.
		/// </summary>
		public static void Field(UnityEngine.Object undoObject, EventDelegateEx del, bool removeButton)
		{
			if (del == null)
				return;

			MonoBehaviour target = null;
			if (removeButton && del.target != null)
			{
				target = EditorGUILayout.ObjectField("Notify", del.target, typeof(MonoBehaviour), true) as MonoBehaviour;

				GUILayout.Space(-20f);
				GUILayout.BeginHorizontal();
				GUILayout.Space(74f);
				if (GUILayout.Button("", "ToggleMixed", GUILayout.Width(20f)))
					target = null;
				GUILayout.EndHorizontal();
			}
			else
			{
				target = EditorGUILayout.ObjectField("Notify", del.target, typeof(MonoBehaviour), true) as MonoBehaviour;
			}

			if (del.target != target)
			{
				Undo.RegisterCompleteObjectUndo(undoObject, "Delegate Selection");
				del.target = target;
				EditorUtility.SetDirty(undoObject);
			}

			if (del.target != null && del.target.gameObject != null)
			{
				GameObject go = del.target.gameObject;
				List<Entry> list = GetMethods(go);

				int selectedIndex;
				int listOffset;
				bool found;
				string[] names = GetMethodNames(list, del, out selectedIndex, out listOffset, out found);

				if (!found)
				{
					string description = del.GetDescription(true);
					if (!string.IsNullOrEmpty(description))
						EditorGUILayout.HelpBox("Method \"" + description + "\" not found", MessageType.Error);
				}

				GUILayout.BeginHorizontal();
				int newIndex = EditorGUILayout.Popup("Method", selectedIndex, names);
				GUILayout.Space(18f);
				GUILayout.EndHorizontal();

				if (newIndex != selectedIndex)
				{
					Entry entry = list[newIndex + listOffset];
					Undo.RegisterCompleteObjectUndo(undoObject, "Delegate Selection");
					del.target = entry.target;
					del.methodName = entry.method.Name;
					del.parameterValues = entry.parameterValues;
					del.parameterNames = entry.parameterNames;
					EditorUtility.SetDirty(undoObject);
				}
				
				bool changed = false;
				EventDelegateEx.ParamValue[] parameterValues = del.parameterValues;
				string[] parameterNames = del.parameterNames;
				for (int paramIndex = 0; paramIndex < parameterValues.Length; ++paramIndex)
				{
					System.Type paramType = parameterValues[paramIndex].mType;
					string paramName = ((parameterNames != null) && (paramIndex < parameterNames.Length) && (parameterNames[paramIndex] != null)) ? parameterNames[paramIndex] : paramType.ToString();
					if (typeof(System.Int32) == paramType)
					{
						int newValue = EditorGUILayout.IntField(paramName, (int)parameterValues[paramIndex].mValue);
						if (newValue != (int)parameterValues[paramIndex].mValue)
						{
							changed = true;
							parameterValues[paramIndex].mValue = newValue;
						}
					}
					else if (typeof(System.Single) == paramType)
					{
						float newValue = EditorGUILayout.FloatField(paramName, (float)parameterValues[paramIndex].mValue);
						if (newValue != (float)parameterValues[paramIndex].mValue)
						{
							changed = true;
							parameterValues[paramIndex].mValue = newValue;
						}
					}
					else if (typeof(System.Boolean) == paramType)
					{
						bool newValue = EditorGUILayout.Toggle(paramName, (bool)parameterValues[paramIndex].mValue);
						if (newValue != (bool)parameterValues[paramIndex].mValue)
						{
							changed = true;
							parameterValues[paramIndex].mValue = newValue;
						}
					}
					else if (typeof(System.String) == paramType)
					{
						System.String newValue = EditorGUILayout.TextField(paramName, (System.String)parameterValues[paramIndex].mValue);
						if (newValue != (System.String)parameterValues[paramIndex].mValue)
						{
							changed = true;
							parameterValues[paramIndex].mValue = newValue;
						}
					}
					else if (typeof(System.Enum).IsAssignableFrom(paramType))
					{
						System.Enum newValue = (System.Enum)EditorGUILayout.EnumPopup(paramName, (System.Enum)parameterValues[paramIndex].mValue);
						if (newValue != (System.Enum)parameterValues[paramIndex].mValue)
						{
							changed = true;
							parameterValues[paramIndex].mValue = newValue;
						}
					}
				}
				if (changed)
				{
					Undo.RegisterCompleteObjectUndo(undoObject, "Delegate Selection");
					del.parameterValues = parameterValues;
					EditorUtility.SetDirty(undoObject);
				}
			}
		}

		/// <summary>
		/// Draw a list of fields for the specified list of delegates.
		/// </summary>
		public static void Field(UnityEngine.Object undoObject, List<EventDelegateEx> list)
		{
			Field(undoObject, list, null, null);
		}

		/// <summary>
		/// Draw a list of fields for the specified list of delegates.
		/// </summary>
		public static void Field(UnityEngine.Object undoObject, List<EventDelegateEx> list, string noTarget, string notValid)
		{
			bool targetPresent = false;
			bool isValid = false;

			// Draw existing delegates
			for (int i = 0; i < list.Count;)
			{
				EventDelegateEx del = list[i];

				if (del == null || del.target == null)
				{
					list.RemoveAt(i);
					continue;
				}

				Field(undoObject, del);
				EditorGUILayout.Space();

				if (del.target == null)
				{
					list.RemoveAt(i);
					continue;
				}
				else
				{
					targetPresent = true;
					if (del.isValid)
						isValid = true;
				}
				++i;
			}

			// Draw a new delegate
			EventDelegateEx newDel = new EventDelegateEx();
			Field(undoObject, newDel);
			
			if (newDel.target != null)
			{
				targetPresent = true;
				list.Add(newDel);
			}

			if (!targetPresent)
			{
				if (!string.IsNullOrEmpty(noTarget))
				{
					GUILayout.Space(6f);
					EditorGUILayout.HelpBox(noTarget, MessageType.Info, true);
					GUILayout.Space(6f);
				}
			}
			else if (!isValid)
			{
				if (!string.IsNullOrEmpty(notValid))
				{
					GUILayout.Space(6f);
					EditorGUILayout.HelpBox(notValid, MessageType.Warning, true);
					GUILayout.Space(6f);
				}
			}
		}
	}

}	// namespace EventDelegates

#endif
