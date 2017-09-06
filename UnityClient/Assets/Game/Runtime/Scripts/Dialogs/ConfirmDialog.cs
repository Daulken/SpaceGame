using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("SPACEJAM/UI/Dialogs/ConfirmDialog")]
public class ConfirmDialog : SceneSingleton<ConfirmDialog>
{
	public LocaliseUIText m_title;
	public LocaliseUIText m_message;

	private Action<bool> m_response;

	// Guarantee this will be always a singleton only - make the constructor protected!
	protected ConfirmDialog()
	{
	}

	public void Show(string titleKey, string messageKey, Action<bool> response)
	{
		// If already active, hide the previous dialog first
		if (gameObject.activeSelf)
			Hide(false);

		// Set the response
		m_response = response;

		// Set the localisation keys
		m_title.key = titleKey;
		m_message.key = messageKey;

		// Show the dialog
		gameObject.SetActive(true);
	}

	public void Hide(bool confirm)
	{
		// Hide the dialog
		gameObject.SetActive(false);

		// Backup and clear the response
		Action<bool> response = m_response;
		m_response = null;

		// Call the response
		if (response != null)
			response(confirm);
	}
}
