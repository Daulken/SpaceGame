using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

 
public class FindMissingScripts : EditorWindow
{
	private Vector2 m_scrollPosition = Vector2.zero;

	[SerializeField] private List<GameObject> m_gameObjectsWithMissingComponents = new List<GameObject>();

	[MenuItem("SPACEJAM/Assets/Find Missing Scripts", false, 100)]
	private static void OnSearchForReferences()
	{
		// Show the window with the selected object
		FindMissingScripts window = GetWindow<FindMissingScripts>(false, "FindMissing", true);
		window.Show();
	}

	public void OnGUI()
	{
		EditorGUIUtility.labelWidth = 180.0f;

		// Find missing script references
		if (GUILayout.Button("Find Missing Scripts"))
			FindMissingScriptReferences();

		EditorGUILayout.Space();

		// Show references to the exact object
		if (m_gameObjectsWithMissingComponents.Count > 0)
		{
			GUILayout.Label("References to Missing Scripts:");
			m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition);
			EditorHelpers.StartBox();

			GUIStyle displayStyle = new GUIStyle(EditorStyles.textField);
			displayStyle.alignment = TextAnchor.MiddleLeft;
			displayStyle.wordWrap = false;
			
			foreach (GameObject go in m_gameObjectsWithMissingComponents)
			{
				if (GUILayout.Button(go.GetNameWithPath(), displayStyle, GUILayout.ExpandWidth(true)))
				{
					Selection.activeObject = go;
					EditorGUIUtility.PingObject(go);
				}
			}
			EditorHelpers.EndBox();
			GUILayout.EndScrollView();
		}

		// Add this, so the controls don't space out
		GUILayout.FlexibleSpace();

		EditorGUIUtility.labelWidth = 0.0f;
	}

	/// <summary>
	/// Class that can be used to compare matches for sorting.
	/// </summary>
	private class MatchComparer : Comparer<GameObject>
	{
		public override int Compare(GameObject a, GameObject b)
		{
			// Handle one or more null references
			if (a == null)
			{
				if (b == null)
					return 0;
				else
					return 1;
			}
			else if (b == null)
			{
				return -1;
			}

			return string.Compare(a.GetNameWithPath(), b.GetNameWithPath());
		}
	}
	
	private void FindMissingScriptReferences()
	{
		// Ensure we clear the last results incase no matches are found
		m_gameObjectsWithMissingComponents.Clear();

		// Find all GameObjects in the scene, including inactive ones
		GameObject[] gameObjects = HelperFunctions.FindAllGameObjects(true);

		// Loop through all game objects
		for (int goIndex = 0, noofGameObjects = gameObjects.Length; goIndex < noofGameObjects; ++goIndex)
		{
			// Display the progress bar, checking for user cancel
			if (EditorUtility.DisplayCancelableProgressBar("Scanning...", "Scanning for missing scripts in the current scene", (goIndex + 1) / (float)noofGameObjects))
				break;

			// Loop through all components on this game object
			Component[] components = gameObjects[goIndex].GetComponents<Component>();
			for (int componentIndex = 0, noofComponents = components.Length; componentIndex < noofComponents; ++componentIndex)
			{
				// If the component is null, it's script could not be found, so add the GameObject it was found on.
				// Also stop searching components, since there's no point in listing the GameObject more than once
				if (components[componentIndex] == null)
				{
					m_gameObjectsWithMissingComponents.Add(gameObjects[goIndex]);
					break;
				}
			}
		}

		// Clear the progress bar
		EditorUtility.ClearProgressBar();

		// Sort matches to make them easier to read
		m_gameObjectsWithMissingComponents.Sort(new MatchComparer());
	}
}
