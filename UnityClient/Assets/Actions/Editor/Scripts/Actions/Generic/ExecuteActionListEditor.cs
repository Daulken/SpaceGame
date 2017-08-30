using UnityEditor;
using UnityEngine;
using System;
 
namespace Actions {


	[CustomEditor(typeof(ExecuteActionList))]
	public class ExecuteActionListEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			// Start updating the serialized object
			serializedObject.Update(); 
			
			// Test to ensure we're of the correct type, in case of multi-object editing
			ExecuteActionList linkedObject = serializedObject.targetObject as ExecuteActionList;
			if (!linkedObject || !linkedObject.gameObject)
				return;
			
			// Add the action fields
			ActionEditorHelpers.CommonActionGUI(serializedObject);
			linkedObject.m_actionListObject = (GameObject)EditorGUILayout.ObjectField(new GUIContent("ActionList GameObject", "The game object with actions on to execute"), linkedObject.m_actionListObject, typeof(GameObject), true);
			if (linkedObject.m_actionListObject != null)
			{
				linkedObject.m_forceRealTime = EditorGUILayout.Toggle(new GUIContent("Force RealTime", "Should the actions explicitly set their realtime state, rather than inherit it?"), linkedObject.m_forceRealTime);
				if (linkedObject.m_forceRealTime)
					linkedObject.m_realTime = EditorGUILayout.Toggle(new GUIContent("RealTime", "Should the actions continue while the game is paused?"), linkedObject.m_realTime);
			}
			linkedObject.m_immediate = EditorGUILayout.Toggle(new GUIContent("Immediate", "Should the actions ignore their durations and delays, and pla through immediately?"), linkedObject.m_immediate);
			
			// Finish updating the serialized object
			serializedObject.ApplyModifiedProperties();
	
			// Mark the object as dirty if any fields changed, and recache action values
			if (GUI.changed)
				linkedObject.MarkDirty();
		}          
	}


}	// namespace Actions
