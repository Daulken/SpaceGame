using UnityEditor;
using UnityEngine;
using System;
 
namespace Actions {


		[CustomEditor(typeof(InvokeMethodsEx))]
		public class InvokeMethodsExEditor : Editor
		{
			public override void OnInspectorGUI()
			{
				// Start updating the serialized object
				serializedObject.Update(); 
				
				// Test to ensure we're of the correct type, in case of multi-object editing
				InvokeMethodsEx linkedObject = serializedObject.targetObject as InvokeMethodsEx;
				if (!linkedObject || !linkedObject.gameObject)
					return;
		
				// Add the action fields
				ActionEditorHelpers.CommonActionGUI(serializedObject);
				if (EditorHelpers.StartBox("Methods", "InvokeMethodsExEditor"))
				{
					EventDelegates.EventDelegateExEditor.Field(linkedObject, linkedObject.m_methodList, null, null);
					EditorHelpers.EndBox();
				}
		
				// Finish updating the serialized object
				serializedObject.ApplyModifiedProperties();
		
				// Mark the object as dirty if any fields changed, and recache action values
				if (GUI.changed)
					linkedObject.MarkDirty();
			}
		}


}	// namespace Actions
