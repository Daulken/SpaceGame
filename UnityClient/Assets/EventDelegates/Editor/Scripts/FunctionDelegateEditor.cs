using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;

namespace EventDelegates {

	public static class FunctionDelegateEditor
	{
		class Entry
		{
			public MonoBehaviour m_target;
			public MethodInfo m_methodName;
		}

		/// <summary>
		/// Collect a list of usable delegates from the specified target game object.
		/// The delegates must be of type "void Delegate()".
		/// </summary>
		private static List<Entry> GetMethods(GameObject target)
		{
			MonoBehaviour[] comps = target.GetComponents<MonoBehaviour>();

			List<Entry> list = new List<Entry>();

			for (int i = 0, imax = comps.Length; i < imax; ++i)
			{
				MonoBehaviour mb = comps[i];
				if (mb == null)
					continue;

				MethodInfo[] methods = mb.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

				for (int b = 0; b < methods.Length; ++b)
				{
					MethodInfo mi = methods[b];

					if ((mi.GetParameters().Length == 0) && (mi.ReturnType == typeof(bool)))
					{
						if ((mi.Name != "IsInvoking") &&
							(mi.Name != "get_useGUILayout") &&
							(mi.Name != "get_runInEditMode") &&
							(mi.Name != "get_isActiveAndEnabled") &&
							(mi.Name != "get_enabled") &&
							(mi.Name != "get_active"))
						{
							Entry ent = new Entry();
							ent.m_target = mb;
							ent.m_methodName = mi;
							list.Add(ent);
						}
					}
				}
			}
			return list;
		}

		/// <summary>
		/// Convert the specified list of delegate entries into a string array.
		/// </summary>
		private static string[] GetMethodNames(List<Entry> list, string choice, out int index)
		{
			index = 0;
			string[] names = new string[list.Count + 1];
			names[0] = string.IsNullOrEmpty(choice) ? "<Choose>" : choice;

			for (int i = 0; i < list.Count; )
			{
				Entry ent = list[i];
				string type = ent.m_target.GetType().ToString();
				int period = type.LastIndexOf('.');
				if (period > 0)
					type = type.Substring(period + 1);

				string del = type + "." + ent.m_methodName.Name;
				names[++i] = del;
				
				if ((index == 0) && (del == choice))
					index = i;
			}
			return names;
		}

		/// <summary>
		/// Draw an editor field for the Unity Delegate.
		/// </summary>
		public static bool Field(Object undoObject, FunctionDelegate del)
		{
			return Field(undoObject, del, true);
		}

		/// <summary>
		/// Draw an editor field for the Unity Delegate.
		/// </summary>
		public static bool Field(Object undoObject, FunctionDelegate del, bool removeButton)
		{
			if (del == null)
				return false;

			bool prev = GUI.changed;
			GUI.changed = false;

			MonoBehaviour target = null;
			if (removeButton && (del.m_target != null))
			{
				target = EditorGUILayout.ObjectField("GameObject", del.m_target, typeof(MonoBehaviour), true) as MonoBehaviour;

				GUILayout.Space(-20f);
				GUILayout.BeginHorizontal();
				GUILayout.Space(84f);
				if (GUILayout.Button("", "ToggleMixed", GUILayout.Width(20f)))
					target = null;
				GUILayout.EndHorizontal();
			}
			else
			{
				target = EditorGUILayout.ObjectField("GameObject", del.m_target, typeof(MonoBehaviour), true) as MonoBehaviour;
			}

			if (del.m_target != target)
			{
				Undo.RegisterCompleteObjectUndo(undoObject, "FunctionDelegate Selection");
				del.m_target = target;
				EditorUtility.SetDirty(undoObject);
			}

			if ((del.m_target) != null && (del.m_target.gameObject != null))
			{
				GameObject go = del.m_target.gameObject;
				List<Entry> list = GetMethods(go);

				int index = 0;
				string[] names = GetMethodNames(list, del.ToString(), out index);

				GUILayout.BeginHorizontal();
				int choice = EditorGUILayout.Popup("Function", index, names);
				GUILayout.Space(18f);
				GUILayout.EndHorizontal();

				if (choice > 0)
				{
					if (choice != index)
					{
						Entry entry = list[choice - 1];
						Undo.RegisterCompleteObjectUndo(undoObject, "Delegate Selection");
						del.m_target = entry.m_target;
						del.m_methodName = entry.m_methodName.Name;
						EditorUtility.SetDirty(undoObject);
						GUI.changed = prev;
						return true;
					}
				}
			}

			bool retVal = GUI.changed;
			GUI.changed = prev;
			return retVal;
		}

		/// <summary>
		/// Draw a list of fields for the specified list of delegates.
		/// </summary>
		public static void Field(Object undoObject, FunctionDelegateList list)
		{
			Field(undoObject, list, null, null);
		}

		/// <summary>
		/// Draw a list of fields for the specified list of delegates.
		/// </summary>
		public static void Field(Object undoObject, FunctionDelegateList list, string noTargetHelpText, string notValidHelpText)
		{
			FunctionDelegateList.OperatorType operatorType = (FunctionDelegateList.OperatorType)EditorGUILayout.EnumPopup("Operator", list.m_operator);
			if (list.m_operator != operatorType)
			{
				Undo.RegisterCompleteObjectUndo(undoObject, "FunctionDelegate Operator");
				list.m_operator = operatorType;
				EditorUtility.SetDirty(undoObject);
			}
			
			// Draw existing delegates
			bool targetPresent = false;
			bool isValid = false;
			for (int i = 0; i < list.m_functions.Count; )
			{
				FunctionDelegate del = list.m_functions[i];
				
				// Remove any dead delegates from the list
				if ((del == null) || (del.m_target == null))
				{
					list.m_functions.RemoveAt(i);
					continue;
				}

				Field(undoObject, del);
				EditorGUILayout.Space();

				if (del.m_target == null)
				{
					list.m_functions.RemoveAt(i);
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

			// Draw a new, empty delegate, ready to add
			FunctionDelegate newDel = new FunctionDelegate();
			Field(undoObject, newDel);

			// Add it if filled in
			if (newDel.m_target != null)
			{
				targetPresent = true;
				list.m_functions.Add(newDel);
			}

			// Draw help text if requried
			if (!targetPresent && !string.IsNullOrEmpty(noTargetHelpText))
			{
				GUILayout.Space(6f);
				EditorGUILayout.HelpBox(noTargetHelpText, MessageType.Info, true);
				GUILayout.Space(6f);
			}
			else if (!isValid && !string.IsNullOrEmpty(notValidHelpText))
			{
				GUILayout.Space(6f);
				EditorGUILayout.HelpBox(notValidHelpText, MessageType.Warning, true);
				GUILayout.Space(6f);
			}
		}
	}

}	// namespace EventDelegates
