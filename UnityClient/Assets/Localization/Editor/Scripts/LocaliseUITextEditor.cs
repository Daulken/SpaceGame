using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LocaliseUIText))]
public class LocaliseUITextEditor : Editor
{
	private void DisplayKey(GUIContent label, SerializedProperty property, Localization.LocalisationData cachedData)
	{
		// If not a string, do nothing
		if (property.stringValue == null)
			return;

		// If no keys are available, just edit the key normally, via a text field, so that we preserve the key
		if ((cachedData.m_editorKeys == null) || (cachedData.m_editorKeys.Count <= 1))
		{
			property.stringValue = EditorGUILayout.TextField(label, property.stringValue);
			return;
		}

		// Get the current key index, and if the key can no longer be found,
		// just edit the text normally, via a text field, so that we preserve the key
		int selectedIndex = string.IsNullOrEmpty(property.stringValue) ? 0 : cachedData.m_editorKeys.IndexOf(property.stringValue);
		if (selectedIndex < 0)
		{
			property.stringValue = EditorGUILayout.TextField(label, property.stringValue);
			return;
		}

		// Display a popup with the list of keys, and translations as tooltips
		int newIndex = EditorGUILayout.Popup(label, selectedIndex, cachedData.m_editorNamesWithTooltips.ToArray(), "Popup");

		// Set the new key, coping for blank selection
		property.stringValue = (newIndex == 0) ? "" : cachedData.m_editorKeys[newIndex];
	}

	public override void OnInspectorGUI()
	{
		// Start updating the serialized object
		serializedObject.Update();

		LocaliseUIText localizer = target as LocaliseUIText;

		// Update the cached localisation keys for this stringtable if required, and return the cached data
		Localization.LocalisationData cachedData = Localization.Instance.UpdateLocalisationData();

		// Display this key
		DisplayKey(new GUIContent("Key"), serializedObject.FindProperty("m_key"), cachedData);

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
