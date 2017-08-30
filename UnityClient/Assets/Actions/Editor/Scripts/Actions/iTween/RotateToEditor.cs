using UnityEditor;
using UnityEngine;
using System;

namespace Actions {

	
	[CustomEditor(typeof(RotateTo))]
	public class RotateToEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			// Start updating the serialized object
			serializedObject.Update(); 
			
			// Test to ensure we're of the correct type, in case of multi-object editing
			RotateTo linkedObject = serializedObject.targetObject as RotateTo;
			if (!linkedObject || !linkedObject.gameObject)
				return;
			
			// Add the action fields
			ActionEditorHelpers.CommonActionGUI(serializedObject);
			linkedObject.m_transform = (Transform)EditorGUILayout.ObjectField(new GUIContent("Game Object", "The object to alter the orientation of"), linkedObject.m_transform, typeof(Transform), true);
			if (linkedObject.m_transform != null)
			{
				linkedObject.m_degrees = EditorGUILayout.Vector3Field("Degrees", linkedObject.m_degrees);
				linkedObject.m_localSpace = EditorGUILayout.Toggle(new GUIContent("Local Space", "Should the change be object-relative, or world-relative?"), linkedObject.m_localSpace);
				ActionEditorHelpers.CommonITweenGUI(serializedObject);
			}
	
			// Finish updating the serialized object
			serializedObject.ApplyModifiedProperties();
	
			// Mark the object as dirty if any fields changed, and recache action values
			if (GUI.changed)
				linkedObject.MarkDirty();
		}
	}

	
}	// namespace Actions
