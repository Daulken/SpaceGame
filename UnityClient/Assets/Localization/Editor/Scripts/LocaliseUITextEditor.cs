using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LocaliseUIText))]
public class LocaliseUITextEditor : Editor
{
	public override void OnInspectorGUI()
	{
		// Start updating the serialized object
		serializedObject.Update();

		LocaliseUIText localizer = target as LocaliseUIText;

		EditorGUILayout.PropertyField(serializedObject.FindProperty("key"));

		// Update the localisation if changed
		bool applyLocalisation = GUILayout.Button("Apply");
		if ((GUI.changed && Application.isPlaying) || applyLocalisation)
		{
			localizer.LocalizeInEditor();
			Repaint();
		}

		// Finish updating the serialized object
		serializedObject.ApplyModifiedProperties();

		// Mark the object as dirty if any fields changed
		if (GUI.changed)
			EditorUtility.SetDirty(serializedObject.targetObject);
	}
}
