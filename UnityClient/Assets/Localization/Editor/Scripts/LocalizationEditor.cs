﻿using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Wizard that allows editing of localization settings
/// </summary>
public class LocalizationEditor : EditorWindow
{
	private Dictionary<string, Dictionary<SystemLanguage, string>> m_lastAppliedTranslations;
	private Vector2 m_scrollPositionTranslations = Vector2.zero;
	private bool m_translating = false;

	[MenuItem("SPACEJAM/Localization/Open Editor")]
	public static void OpenLocalizationEditor()
	{
		EditorWindow.GetWindow<LocalizationEditor>(false, "Localization", true);
	}

	[MenuItem("SPACEJAM/Localization/Open Stringtable")]
	public static void OpenStringtable()
	{
		string[] guids = AssetDatabase.FindAssets("Strings");
		foreach (string guid in guids)
		{
			string name = AssetDatabase.GUIDToAssetPath(guid);
			if (name.ToLower().Contains("strings.ods"))
			{
				Object stringtable = AssetDatabase.LoadAssetAtPath(name, typeof(Object));
				AssetDatabase.OpenAsset(stringtable);
			}
		}
	}

	/// <summary>
	/// Draw the custom wizard.
	/// </summary>
	void OnGUI()
	{
		// Add the editor fields
		GUIStyle wrappedBoxStyle = new GUIStyle(EditorStyles.textField);
		wrappedBoxStyle.wordWrap = true;
		string stringtableResourceName = PlayerPrefs.GetString("LocalisationStringTableResourceName", "Strings_Stringtable");
		EditorGUILayout.LabelField("Stringtable Resource: ", stringtableResourceName, wrappedBoxStyle);

		// Update the localisation if changed
		if (!m_translating)
		{
			bool applyLocalisation = GUILayout.Button("Auto-translate missing entries from English");
			if ((GUI.changed && Application.isPlaying) || applyLocalisation)
			{
				m_translating = true;
				Localization.Instance.AutoTranslateNonEnglishLanguages((changes) =>
						{
							m_lastAppliedTranslations = changes;
							m_translating = false;
						}
					);
			}
		}
		else
		{
			if (GUILayout.Button("Cancel translation"))
			{
				Localization.Instance.CancelAutoTranslate();
				m_translating = false;
			}
		}

		// Show the last applied auto-translations
		if ((m_lastAppliedTranslations != null) && (m_lastAppliedTranslations.Count > 0))
		{
			GUILayout.Label("Applied translations:");
			m_scrollPositionTranslations = GUILayout.BeginScrollView(m_scrollPositionTranslations, GUILayout.ExpandHeight(true));
			EditorHelpers.StartBox();

			foreach (KeyValuePair<string, Dictionary<SystemLanguage, string>> keyDictPair in m_lastAppliedTranslations)
			{
				GUILayout.Label(keyDictPair.Key, EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
				foreach (KeyValuePair<SystemLanguage, string> languageStringPair in keyDictPair.Value)
				{
					GUILayout.Label(string.Format("{0}: {1}", languageStringPair.Key.ToString(), languageStringPair.Value), EditorStyles.label, GUILayout.ExpandWidth(true));
				}
			}
			EditorHelpers.EndBox();
			GUILayout.EndScrollView();
		}
	}
}

// Container for localisation property drawer for automatic display in the inspector, without an editor script
public static class LocalisationPropertyDrawer
{
	// Helper to automatically render a localisation key, rather than a string in the Inspector window
	[CustomPropertyDrawer(typeof(LocalizationKeyAttribute))]
	private class LocalizationKeyPropertyDrawer : PropertyDrawer
	{
		private void DisplayKey(Rect position, SerializedProperty property, GUIContent label, Localization.LocalisationData cachedData)
		{
			// If not a string, do nothing
			if (property.stringValue == null)
				return;

			// If no keys are available, just edit the key normally, via a text field, so that we preserve the key
			if ((cachedData.m_editorKeys == null) || (cachedData.m_editorKeys.Count <= 1))
			{
				property.stringValue = EditorGUI.TextField(position, label, property.stringValue);
				return;
			}

			// Get the current key index, and if the key can no longer be found,
			// just edit the text normally, via a text field, so that we preserve the key
			int selectedIndex = string.IsNullOrEmpty(property.stringValue) ? 0 : cachedData.m_editorKeys.IndexOf(property.stringValue);
			if (selectedIndex < 0)
			{
				property.stringValue = EditorGUI.TextField(position, label, property.stringValue);
				return;
			}

			// Remove a bit of padding above
			position.y -= 3.0f;

			// Display a popup with the list of keys, and translations as tooltips
			int newIndex = EditorGUI.Popup(position, label, selectedIndex, cachedData.m_editorNamesWithTooltips.ToArray(), "Popup");

			// Set the new key, coping for blank selection
			property.stringValue = (newIndex == 0) ? "" : cachedData.m_editorKeys[newIndex];
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			// Update the cached localisation keys for this stringtable if required, and return the cached data
			Localization.LocalisationData cachedData = Localization.Instance.UpdateLocalisationData();

			// If the property is an array
			if (typeof(System.Array).IsAssignableFrom(fieldInfo.FieldType) && (property.type != "string"))
			{
				// Reset the height of each element
				position.height = 16.0f;

				// Add a foldout for this enumeration entry
				property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
				position.y += 16.0f;
				if (property.isExpanded)
				{
					// Indent all members of this array
					position.x += 25.0f;
					position.width -= 25.0f;

					// Show the size field
					property.arraySize = EditorGUI.IntField(position, "Size", property.arraySize);
					position.y += 16.0f;

					// Go through each entry in the array
					for (int index = 0; index < property.arraySize; ++index)
					{
						// Display this key
						SerializedProperty arrayProperty = property.GetArrayElementAtIndex(index);
						DisplayKey(position, arrayProperty, new GUIContent("Element " + index.ToString()), cachedData);
						position.y += 16.0f;
					}
				}
			}
			// A single entry
			else
			{
				// Display this key
				DisplayKey(position, property, label, cachedData);
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			// If the property is an array
			if (typeof(System.Array).IsAssignableFrom(fieldInfo.FieldType) && (property.type != "string"))
			{
				// Calculate the height, starting with the initial foldout, and size
				float totalHeight = 32.0f;
				if (property.isExpanded)
				{
					// Go through each entry in the array
					for (int index = 0; index < property.arraySize; ++index)
					{
						// Don't render anything if the property is not a string
						SerializedProperty arrayProperty = property.GetArrayElementAtIndex(index);
						if (arrayProperty.stringValue != null)
							totalHeight += 16.0f;
					}
				}

				// Return the total height, plus a bit to allow for the larger popup than the usual 'mini' one
				return totalHeight;
			}

			// Otherwise, don't render anything if the property is not a string
			if (property.stringValue == null)
				return 0.0f;

			// Return the normal height, plus a bit to allow for the larger popup than the usual 'mini' one
			return 16.0f;
		}
	}
}
