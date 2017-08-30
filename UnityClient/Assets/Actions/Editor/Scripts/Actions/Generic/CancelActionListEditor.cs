using UnityEditor;
using UnityEngine;
using System;
 
namespace Actions {


	[CustomEditor(typeof(CancelActionList))]
	public class CancelActionListEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			// Start updating the serialized object
			serializedObject.Update(); 
			
			// Test to ensure we're of the correct type, in case of multi-object editing
			CancelActionList linkedObject = serializedObject.targetObject as CancelActionList;
			if (!linkedObject || !linkedObject.gameObject)
				return;
			
			// Add the action fields
			ActionEditorHelpers.CommonActionGUI(serializedObject);
			linkedObject.m_actionListObject = (GameObject)EditorGUILayout.ObjectField(new GUIContent("ActionList GameObject", "The game object with actions on to cancel"), linkedObject.m_actionListObject, typeof(GameObject), true);
			
			// Finish updating the serialized object
			serializedObject.ApplyModifiedProperties();
	
			// Mark the object as dirty if any fields changed, and recache action values
			if (GUI.changed)
				linkedObject.MarkDirty();
		}          
	}


}	// namespace Actions
