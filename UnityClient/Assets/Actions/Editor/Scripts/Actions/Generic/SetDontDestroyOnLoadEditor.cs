using UnityEditor;
using UnityEngine;
using System;
 
namespace Actions {


	[CustomEditor(typeof(SetDontDestroyOnLoad))]
	public class SetDontDestroyOnLoadEditor : Editor
	{
		SerializedProperty m_gameObjectProperty = null;

		public virtual void OnEnable()
		{
			m_gameObjectProperty = serializedObject.FindProperty("m_gameObjects");
		}

		public override void OnInspectorGUI()
		{
			// Start updating the serialized object
			serializedObject.Update(); 
			
			// Test to ensure we're of the correct type, in case of multi-object editing
			SetDontDestroyOnLoad linkedObject = serializedObject.targetObject as SetDontDestroyOnLoad;
			if (!linkedObject || !linkedObject.gameObject)
				return;
			
			// Add the action fields
			ActionEditorHelpers.CommonActionGUI(serializedObject);
			EditorGUILayout.PropertyField(m_gameObjectProperty, true);

			// Finish updating the serialized object
			serializedObject.ApplyModifiedProperties();

			// Mark the object as dirty if any fields changed, and recache action values
			if (GUI.changed)
				linkedObject.MarkDirty();
		}
	}


}	// namespace Actions
