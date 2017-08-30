using UnityEditor;
using UnityEngine;
using System;
 
namespace Actions {


	[CustomEditor(typeof(SetActive))]
	public class SetActiveEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			// Start updating the serialized object
			serializedObject.Update(); 
			
			// Test to ensure we're of the correct type, in case of multi-object editing
			SetActive linkedObject = serializedObject.targetObject as SetActive;
			if (!linkedObject || !linkedObject.gameObject)
				return;
			
			// Add the action fields
			ActionEditorHelpers.CommonActionGUI(serializedObject);
			linkedObject.m_gameObject = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Game Object", "The object to change"), linkedObject.m_gameObject, typeof(GameObject), true);
			if (linkedObject.m_gameObject != null)
			{
				linkedObject.m_setActive = EditorGUILayout.Toggle(new GUIContent("Active", "The new active state"), linkedObject.m_setActive);
			}
			
			// Finish updating the serialized object
			serializedObject.ApplyModifiedProperties();
	
			// Mark the object as dirty if any fields changed, and recache action values
			if (GUI.changed)
				linkedObject.MarkDirty();
		}          
	}


}	// namespace Actions
