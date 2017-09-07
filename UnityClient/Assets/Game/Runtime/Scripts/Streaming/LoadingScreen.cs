using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("SPACEJAM/UI/LoadingScreen")]
public class LoadingScreen : SceneSingleton<LoadingScreen>
{
	public Image m_animatedSpinner;

	// Guarantee this will be always a singleton only - make the constructor protected!
	protected LoadingScreen()
	{
	}

	public void Show()
	{
		// Show the dialog
		gameObject.SetActive(true);
	}

	public void SetProgress(float progress)
	{
		m_animatedSpinner.fillAmount = progress;
	}

	public void Hide()
	{
		// Hide the dialog
		gameObject.SetActive(false);
	}

}
