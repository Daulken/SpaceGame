using UnityEditor;
using UnityEngine;
using System;
 
namespace Actions {

	[CustomEditor(typeof(SetParticleSystemEmitting))]
	public class SetParticleSystemEmittingEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			// Start updating the serialized object
			serializedObject.Update(); 

			// Test to ensure we're of the correct type, in case of multi-object editing
			SetParticleSystemEmitting linkedObject = serializedObject.targetObject as SetParticleSystemEmitting;
			if (!linkedObject || !linkedObject.gameObject)
				return;

			// Add the action fields
			ActionEditorHelpers.CommonActionGUI(serializedObject);
			linkedObject.m_particleSystem = (ParticleSystem)EditorGUILayout.ObjectField(new GUIContent("Particle System", "The particle system to set the emittance of"), linkedObject.m_particleSystem, typeof(ParticleSystem), true);
			linkedObject.m_toggle = EditorGUILayout.Toggle(new GUIContent("Toggle", "Toggle the active state of the emitter"), linkedObject.m_toggle);
			if (!linkedObject.m_toggle)
				linkedObject.m_emitting = EditorGUILayout.Toggle(new GUIContent("Emit", "Set the active state of the emitter"), linkedObject.m_emitting);

			// Finish updating the serialized object
			serializedObject.ApplyModifiedProperties();

			// Mark the object as dirty if any fields changed, and recache action values
			if (GUI.changed)
				linkedObject.MarkDirty();
		}
	}


}	// namespace Actions
