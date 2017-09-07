using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[AddComponentMenu("SPACEJAM/Streaming/GameFlowLoader")]
public class GameFlowLoader : MonoBehaviour
{
	public string m_autoLoginUsername;
	public string m_autoLoginPassword;

	private GameplayManager.State m_initialState;

	// This is called for all components before Start, even if not enabled
	protected void Awake()
	{
		// If the gameplay manager isn't available in the current scene, load the game scene
		if (GameplayManager.Instance == null)
		{
			// Get the game play state for the current scene
			m_initialState = (GameplayManager.State)Enum.Parse(typeof(GameplayManager.State), SceneManager.GetActiveScene().name);
			if (Enum.IsDefined(typeof(GameplayManager.State), m_initialState))
			{
				// If this is a valid gameplay state, then we can load the game additively to this scene
				SceneManager.sceneLoaded += OnSceneChanged;
				SceneManager.LoadScene("Game", LoadSceneMode.Additive);
			}
		}
	}

	private void OnSceneChanged(Scene scene, LoadSceneMode mode)
	{
		if (scene.name != "Game")
			return;

		SceneManager.sceneLoaded -= OnSceneChanged;

		// If the initial state isn't the Login state, perform an auto-login, so that the player can be fetched
		if (m_initialState != GameplayManager.State.Login)
		{
			MessageDialog.Instance.Show("LOGIN_INFO_LOGGING_IN");

			// Log in to the database
			DatabaseAccess.Login(m_autoLoginUsername, m_autoLoginPassword, (loginSuccess, loginError) =>
					{
						MessageDialog.Instance.Hide();

						if (loginSuccess)
						{
							// Switch the current state back to this scene
							GameplayManager.Instance.CurrentState = m_initialState;
						}
						else
						{
							// Display error dialog
							InfoDialog.Instance.Show("LOGIN_ERROR_TITLE", loginError, null);
						}
					}
				);
		}
	}

	// This is called whenever the node is enabled.
	// NOTE: ALL scripts require this function, even if not used, to avoid a bug with script execution order being ignored
	protected void OnEnable()
	{
	}

	// Update is called once per frame
	protected void Update()
	{
		// Destroy ourself
		Destroy(gameObject);
	}
}
