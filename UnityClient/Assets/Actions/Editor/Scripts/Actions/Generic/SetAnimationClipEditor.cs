using UnityEditor;
using UnityEngine;
using System;
 
namespace Actions {


	[CustomEditor(typeof(SetAnimationClip))]
	public class SetAnimationClipEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			// Start updating the serialized object
			serializedObject.Update(); 
			
			// Test to ensure we're of the correct type, in case of multi-object editing
			SetAnimationClip linkedObject = serializedObject.targetObject as SetAnimationClip;
			if (!linkedObject || !linkedObject.gameObject)
				return;
	
			// Add the action fields
			ActionEditorHelpers.CommonActionGUI(serializedObject);
			linkedObject.m_animation = (Animation)EditorGUILayout.ObjectField(new GUIContent("Animation", "The animation to play the clip on"), linkedObject.m_animation, typeof(Animation), true);
			if (linkedObject.m_animation != null)
			{
				linkedObject.m_clip = (AnimationClip)EditorGUILayout.ObjectField(new GUIContent("Clip", "The animation clip to play"), linkedObject.m_clip, typeof(AnimationClip), false);
				if (linkedObject.m_clip != null)
				{
					linkedObject.m_duration = EditorGUILayout.FloatField(new GUIContent("Duration", "The time in seconds to cross-fade in the new animation clip"), linkedObject.m_duration);
				}
			}
	
			// Finish updating the serialized object
			serializedObject.ApplyModifiedProperties();
	
			// Mark the object as dirty if any fields changed, and recache action values
			if (GUI.changed)
				linkedObject.MarkDirty();
		}          
	}


}	// namespace Actions
