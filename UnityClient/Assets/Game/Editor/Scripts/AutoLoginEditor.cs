using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Wizard that allows editing of auto-login settings
/// </summary>
public class AutoLoginEditor : EditorWindow
{
	private string m_username;
	private string m_password;

	[MenuItem("SPACEJAM/AutoLogin/Edit Settings")]
	public static void OpenAutoLoginEditor()
	{
		EditorWindow.GetWindow<AutoLoginEditor>(false, "AutoLogin", true);
	}

	protected void Awake()
	{
		// Read current properties
		m_username = PlayerPrefs.GetString("AutoLoginUsername");
		m_password = PlayerPrefs.GetString("AutoLoginPassword");
	}

	/// <summary>
	/// Draw the custom wizard.
	/// </summary>
	void OnGUI()
	{
		m_username = EditorGUILayout.TextField(new GUIContent("Username:"), m_username);
		m_password = EditorGUILayout.TextField(new GUIContent("Password:"), m_password);

		if (GUILayout.Button("Save"))
		{
			PlayerPrefs.SetString("AutoLoginUsername", m_username);
			PlayerPrefs.SetString("AutoLoginPassword", m_password);
		}
	}
}
