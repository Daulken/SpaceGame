using UnityEditor;
using UnityEngine;
using System;
 
namespace Actions {


	[CustomEditor(typeof(ColourTo))]
	public class ColourToEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			// Start updating the serialized object
			serializedObject.Update(); 
			
			// Test to ensure we're of the correct type, in case of multi-object editing
			ColourTo linkedObject = serializedObject.targetObject as ColourTo;
			if (!linkedObject || !linkedObject.gameObject)
				return;
			
			// Add the action fields
			ActionEditorHelpers.CommonActionGUI(serializedObject);
			linkedObject.m_gameObject = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Game Object", "The object to alter the colour of"), linkedObject.m_gameObject, typeof(GameObject), true);
			if (linkedObject.m_gameObject != null)
			{
				linkedObject.m_colour = EditorGUILayout.ColorField(new GUIContent("Colour", "The colour to change to"), linkedObject.m_colour);
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
