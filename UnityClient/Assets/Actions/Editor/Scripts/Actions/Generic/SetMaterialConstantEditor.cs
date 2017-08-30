using UnityEditor;
using UnityEngine;
using System;
 
namespace Actions {

	[CustomEditor(typeof(SetMaterialConstant))]
	public class SetMaterialConstantEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			// Start updating the serialized object
			serializedObject.Update(); 

			// Test to ensure we're of the correct type, in case of multi-object editing
			SetMaterialConstant linkedObject = serializedObject.targetObject as SetMaterialConstant;
			if (!linkedObject || !linkedObject.gameObject)
				return;

			// Add the action fields
			ActionEditorHelpers.CommonActionGUI(serializedObject);
			linkedObject.m_material = (Material)EditorGUILayout.ObjectField(new GUIContent("Material", "The material to set the constant on"), linkedObject.m_material, typeof(Material), false);
			linkedObject.m_name = EditorGUILayout.TextField(new GUIContent("Name", "The name of the constant to set"), linkedObject.m_name);
			linkedObject.m_type = (SetMaterialConstant.ConstantType)EditorGUILayout.EnumPopup(new GUIContent("Type", "The type of the constant to set"), linkedObject.m_type);
			switch (linkedObject.m_type)
			{
			case SetMaterialConstant.ConstantType.Float:
				linkedObject.m_constantFloat = EditorGUILayout.FloatField(new GUIContent("Float", "The float to set"), linkedObject.m_constantFloat);
				break;
			case SetMaterialConstant.ConstantType.Vector4:
				linkedObject.m_constantVector = EditorGUILayout.Vector4Field("Vector4", linkedObject.m_constantVector);
				break;
			case SetMaterialConstant.ConstantType.Color:
				linkedObject.m_constantColour = EditorGUILayout.ColorField(new GUIContent("Colour", "The colour to set"), linkedObject.m_constantColour);
				break;
			}

			// Finish updating the serialized object
			serializedObject.ApplyModifiedProperties();

			// Mark the object as dirty if any fields changed, and recache action values
			if (GUI.changed)
				linkedObject.MarkDirty();
		}          
	}


}	// namespace Actions
