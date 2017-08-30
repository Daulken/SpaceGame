using UnityEditor;
using UnityEngine;
using System;
 
namespace Actions {


	[CustomEditor(typeof(DestroyGameObject))]
	public class DestroyGameObjectEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			// Start updating the serialized object
			serializedObject.Update(); 
			
			// Test to ensure we're of the correct type, in case of multi-object editing
			DestroyGameObject linkedObject = serializedObject.targetObject as DestroyGameObject;
			if (!linkedObject || !linkedObject.gameObject)
				return;
	
			// Add the action fields
			ActionEditorHelpers.CommonActionGUI(serializedObject);
			linkedObject.m_gameObject = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Game Object", "The game object to destroy"), linkedObject.m_gameObject, typeof(GameObject), true);
			
			// Finish updating the serialized object
			serializedObject.ApplyModifiedProperties();
	
			// Mark the object as dirty if any fields changed, and recache action values
			if (GUI.changed)
				linkedObject.MarkDirty();
		}          
	}


}	// namespace Actions
