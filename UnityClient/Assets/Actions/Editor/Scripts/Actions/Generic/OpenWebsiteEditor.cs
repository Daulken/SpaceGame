using UnityEditor;
using UnityEngine;
using System;
 
namespace Actions {

	
	[CustomEditor(typeof(OpenWebsite))]
	public class OpenWebsiteEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			// Start updating the serialized object
			serializedObject.Update(); 
			
			// Test to ensure we're of the correct type, in case of multi-object editing
			OpenWebsite linkedObject = serializedObject.targetObject as OpenWebsite;
			if (!linkedObject || !linkedObject.gameObject)
				return;

			// Add the action fields
			ActionEditorHelpers.CommonActionGUI(serializedObject);
			linkedObject.m_url = EditorGUILayout.TextField(new GUIContent("URL", "The fully formed URL to open"), linkedObject.m_url);
			
			// Finish updating the serialized object
			serializedObject.ApplyModifiedProperties();
	
			// Mark the object as dirty if any fields changed, and recache action values
			if (GUI.changed)
				linkedObject.MarkDirty();
		}          
	}
	

}	// namespace Actions
