using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


[AddComponentMenu("SPACEJAM/GameState/GameplayManager")]
public class GameplayManager : SceneSingleton<GameplayManager>
{
	public enum State
	{
		None,
		Login,
		Market,
		System,
		Planet,
		PlanetSide,
	}

	private State m_currentState = State.None;

	// Guarantee this will be always a singleton only - make the constructor protected!
	protected GameplayManager()
	{
	}

	public State GetCurrentState()
	{
		return m_currentState;
	}

	public void RequestNewState(State newState, bool performAutoLogin = false)
	{
		// Load the scene for the state
		StartCoroutine(AsynchronousLoad(newState, performAutoLogin));
	}

	private IEnumerator AsynchronousLoad(State newState, bool performAutoLogin)
	{
		// Show the loading screen
		LoadingScreen.Instance.Show();

		yield return null;

#if UNITY_EDITOR
		// If we need to perform an auto-login, do this while loading, so that the player can be fetched
		if (performAutoLogin)
		{
			// Get the auto-login username and password to use
			string autoLoginUsername = PlayerPrefs.GetString("AutoLoginUsername");
			string autoLoginPassword = PlayerPrefs.GetString("AutoLoginPassword");
			if (string.IsNullOrEmpty(autoLoginUsername) && string.IsNullOrEmpty(autoLoginUsername))
			{
				// Display error dialog
				InfoDialog.Instance.Show("AUTOLOGIN_ERROR_TITLE", "AUTOLOGIN_ERROR_NOCREDENTIALS", null);

				// Turn off auto-login, so the game can continue, but switch the initial view to Login
				performAutoLogin = false;
				newState = State.Login;
			}
			else
			{
				MessageDialog.Instance.Show("LOGIN_INFO_LOGGING_IN");

				// Log in to the database
				DatabaseAccess.Login(autoLoginUsername, autoLoginPassword, (loginSuccess, loginError) =>
						{
							MessageDialog.Instance.Hide();

							if (!loginSuccess)
							{
								// Display error dialog
								InfoDialog.Instance.Show("AUTOLOGIN_ERROR_TITLE", "AUTOLOGIN_ERROR_FAILED", null);

								// Turn off auto-login, so the game can continue, but switch the initial view to Login
								performAutoLogin = false;
								newState = State.Login;
							}
						}
					);
			}
		}
#else // #if UNITY_EDITOR
		// Turn off auto-login, so the game can continue
		performAutoLogin = false;
#endif // #if UNITY_EDITOR


		// Start loading the new scene, ensuring that it doesn't activate until we are ready
		AsyncOperation ao = SceneManager.LoadSceneAsync(newState.ToString());
		ao.allowSceneActivation = false;

		// While the scene is loading
		while (!ao.isDone)
		{
			// Update the loading progress. Loading progress is [0, 0.9], so map this to [0, 1]
			float progress = Mathf.Clamp01(ao.progress / 0.9f);
			LoadingScreen.Instance.SetProgress(progress);
 
			// Once loading is complete, and any required auto-login is performed
			if ((ao.progress >= 0.9f) && (!performAutoLogin || (DatabaseAccess.LoggedInPlayer != null)))
			{
				// Allow the scene to activate
				ao.allowSceneActivation = true;
			}

			yield return null;
		}

		// Set the new state
		m_currentState = newState;

		// Hide the loading screen
		LoadingScreen.Instance.Hide();
	}
}
