using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[AddComponentMenu("SPACEJAM/Streaming/GameFlowLoader")]
public class GameFlowLoader : MonoBehaviour
{
	private GameplayManager.State m_initialState;

#if UNITY_EDITOR
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
				// Suppress any default game state changes
				SetGameStateOnStart.SuppressInitialState = true;

				// Ensure we aren't destroyed when removing the current scene
				DontDestroyOnLoad(gameObject);

				// Replace the current scene with the Game scene, monitoring when it finishes
				SceneManager.sceneLoaded += OnSceneChanged;
				SceneManager.LoadScene("Game");
			}
		}
	}

	private void OnSceneChanged(Scene scene, LoadSceneMode mode)
	{
		if (scene.name != "Game")
			return;

		SceneManager.sceneLoaded -= OnSceneChanged;

		// Disable any child camera(s) used for Editor display, to ensure there only 1 UI camera in the game (comes from the Game scene)
		Camera[] cameras = transform.GetComponentsInChildren<Camera>();
		foreach (Camera camera in cameras)
			camera.enabled = false;

		// Switch the current state back to this scene, performing auto-login if not initially on the login screen
		GameplayManager.Instance.RequestNewState(m_initialState, (m_initialState != GameplayManager.State.Login));
	}
#endif // #if UNITY_EDITOR

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
