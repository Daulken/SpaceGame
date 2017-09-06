using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("SPACEJAM/UI/Dialogs/InfoDialog")]
public class InfoDialog : SceneSingleton<InfoDialog>
{
	public LocaliseUIText m_title;
	public LocaliseUIText m_message;

	private Action m_response;

	// Guarantee this will be always a singleton only - make the constructor protected!
	protected InfoDialog()
	{
	}

	public void Show(string titleKey, string messageKey, Action response)
	{
		// If already active, hide the previous dialog first
		if (gameObject.activeSelf)
			Hide();

		// Set the response
		m_response = response;

		// Set the localisation keys
		m_title.key = titleKey;
		m_message.key = messageKey;

		// Show the dialog
		gameObject.SetActive(true);
	}

	public void Hide()
	{
		// Hide the dialog
		gameObject.SetActive(false);

		// Backup and clear the response
		Action response = m_response;
		m_response = null;

		// Call the response
		if (response != null)
			response();
	}
}
