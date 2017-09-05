using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[AddComponentMenu("SPACEJAM/Communication/NetworkTest")]
public class NetworkTest : MonoBehaviour
{
	private enum GameState
	{
		LoginScreen,
		LoggingIn,
		LoggedIn,
		FetchingPlayer,
		Running,
	};

	private GameState m_gameState = GameState.LoginScreen;

	// Use this for initialization
	protected void Start()
	{
		// Log in to the database
		m_gameState = GameState.LoggingIn;
		DatabaseAccess.Login("chris", "flibble", (loginSuccess, loginError) =>
				{
					if (loginSuccess)
					{
						DebugHelpers.Log("Logged in as player {0}", WebManager.LoggedInPlayerID);
					}
					else
					{
						DebugHelpers.LogError("Login failed: {0}", loginError);
					}

					m_gameState = GameState.LoggedIn;
				}
			);
	}

	protected void Update()
	{
		if (m_gameState == GameState.LoggedIn)
		{
			// Request data for the player
			m_gameState = GameState.FetchingPlayer;
			DatabaseAccess.GetPlayer((getSuccess, getError, player) =>
					{
						if (getSuccess)
						{
							DebugHelpers.Log("Player Fetched");
							m_gameState = GameState.Running;

							// Increase the max crew count
							player.MaxCrew += 1;

							DatabaseAccess.SavePlayer(player, (saveSuccess, saveError) =>
									{
										if (saveSuccess)
											DebugHelpers.Log("Player saved");
										else
											DebugHelpers.LogError("SavePlayer failed: {0}", saveError);
									}
								);
						}
						else
						{
							DebugHelpers.LogError("GetPlayer failed: {0}", getError);
						}
					}
				);
		}
	}
}
