﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("SPACEJAM/UI/Dialogs/LoginDialog")]
public class LoginDialog : SceneSingleton<LoginDialog>
{
	public LocaliseUIText m_message;

	// Guarantee this will be always a singleton only - make the constructor protected!
	protected LoginDialog()
	{
	}

	public void Show(string messageKey)
	{
		// Set the localisation key
		m_message.key = messageKey;

		// Show the dialog
		gameObject.SetActive(true);
	}

	public void Hide()
	{
		// Hide the dialog
		gameObject.SetActive(false);
	}
}
