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

	public State CurrentState
	{
		get
		{
			return m_currentState;
		}
		set
		{
			// Set the current state
			m_currentState = value;

			// Load the scene for the state
			SceneManager.LoadScene(value.ToString(), LoadSceneMode.Single);
		}
	}

	// Guarantee this will be always a singleton only - make the constructor protected!
	protected GameplayManager()
	{
	}
}
