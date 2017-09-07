using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetGameStateOnStart : MonoBehaviour
{
	public GameplayManager.State m_state = GameplayManager.State.None;

	// Use this for initialization
	void Start()
	{
		// Change game state to the selected state
		GameplayManager.Instance.CurrentState = m_state;
	}
}
