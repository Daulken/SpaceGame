using UnityEditor;
using UnityEngine;
using System;
 
namespace Actions {


	[CustomEditor(typeof(DestroyComponent))]
	public class DestroyComponentEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			// Start updating the serialized object
			serializedObject.Update(); 
			
			// Test to ensure we're of the correct type, in case of multi-object editing
			DestroyComponent linkedObject = serializedObject.targetObject as DestroyComponent;
			if (!linkedObject || !linkedObject.gameObject)
				return;
	
			// Add the action fields
			ActionEditorHelpers.CommonActionGUI(serializedObject);
			linkedObject.m_component = (MonoBehaviour)EditorGUILayout.ObjectField(new GUIContent("Component", "The component to destroy"), linkedObject.m_component, typeof(MonoBehaviour), true);
			
			// Finish updating the serialized object
			serializedObject.ApplyModifiedProperties();
	
			// Mark the object as dirty if any fields changed, and recache action values
			if (GUI.changed)
				linkedObject.MarkDirty();
		}          
	}


}	// namespace Actions
