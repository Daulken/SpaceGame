using UnityEditor;
using UnityEngine;
using System;
 
namespace Actions {


	[CustomEditor(typeof(ExecuteIfElseActionList))]
	public class ExecuteIfElseActionListEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			// Start updating the serialized object
			serializedObject.Update(); 
			
			// Test to ensure we're of the correct type, in case of multi-object editing
			ExecuteIfElseActionList linkedObject = serializedObject.targetObject as ExecuteIfElseActionList;
			if (!linkedObject || !linkedObject.gameObject)
				return;
	
			// Add the action fields
			ActionEditorHelpers.CommonActionGUI(serializedObject);
			if (EditorHelpers.StartBox("IF", "ExecuteIfElseActionListEditor"))
			{
				EventDelegates.FunctionDelegateEditor.Field(linkedObject, linkedObject.m_if, null, null);
				EditorHelpers.EndBox();
			}
			linkedObject.m_actionListObjectIf = (GameObject)EditorGUILayout.ObjectField(new GUIContent("THEN ActionList", "The game object with actions on to execute if the test is true"), linkedObject.m_actionListObjectIf, typeof(GameObject), true);
			linkedObject.m_actionListObjectElse = (GameObject)EditorGUILayout.ObjectField(new GUIContent("ELSE ActionList", "The game object with actions on to execute if the test is false"), linkedObject.m_actionListObjectElse, typeof(GameObject), true);
			if ((linkedObject.m_actionListObjectIf != null) || (linkedObject.m_actionListObjectElse != null))
			{
				linkedObject.m_forceRealTime = EditorGUILayout.Toggle(new GUIContent("Force RealTime?", "Should the actions explicitly set their realtime state, rather than inherit it?"), linkedObject.m_forceRealTime);
				if (linkedObject.m_forceRealTime)
					linkedObject.m_realTime = EditorGUILayout.Toggle(new GUIContent("RealTime", "Should the actions continue while the game is paused?"), linkedObject.m_realTime);
				linkedObject.m_immediate = EditorGUILayout.Toggle(new GUIContent("Immediate", "Should the actions ignore their durations and delays, and pla through immediately?"), linkedObject.m_immediate);
			}
			
			// Finish updating the serialized object
			serializedObject.ApplyModifiedProperties();
	
			// Mark the object as dirty if any fields changed, and recache action values
			if (GUI.changed)
				linkedObject.MarkDirty();
		}          
	}


}	// namespace Actions
