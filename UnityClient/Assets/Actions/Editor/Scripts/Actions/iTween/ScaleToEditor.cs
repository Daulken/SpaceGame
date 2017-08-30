using UnityEditor;
using UnityEngine;
using System;

namespace Actions {
			
	[CustomEditor(typeof(ScaleTo))]
	public class ScaleToEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			// Start updating the serialized object
			serializedObject.Update(); 
			
			// Test to ensure we're of the correct type, in case of multi-object editing
			ScaleTo linkedObject = serializedObject.targetObject as ScaleTo;
			if (!linkedObject || !linkedObject.gameObject)
				return;
			
			// Add the action fields
			ActionEditorHelpers.CommonActionGUI(serializedObject);
			linkedObject.m_transform = (Transform)EditorGUILayout.ObjectField(new GUIContent("Game Object", "The object to alter the scale of"), linkedObject.m_transform, typeof(Transform), true);
			if (linkedObject.m_transform != null)
			{
				linkedObject.m_scale = EditorGUILayout.Vector3Field("Scale", linkedObject.m_scale);
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
