using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetGameStateOnStart : MonoBehaviour
{
	public GameplayManager.State m_state = GameplayManager.State.None;

	private static bool ms_suppressInitialState = false;
	public static bool SuppressInitialState
	{
		get
		{
			return ms_suppressInitialState;
		}
		set
		{
			ms_suppressInitialState = value;
		}
	}

	// Use this for initialization
	void Start()
	{
		// Change game state to the selected state, if not suppressed
		if (!SuppressInitialState)
			GameplayManager.Instance.RequestNewState(m_state);
	}
}
