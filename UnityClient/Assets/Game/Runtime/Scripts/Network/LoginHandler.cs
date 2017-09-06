using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[AddComponentMenu("SPACEJAM/Communication/LoginHandler")]
public class LoginHandler : MonoBehaviour
{
	public UnityEngine.UI.InputField m_usernameInput;
	public UnityEngine.UI.InputField m_passwordInput;

	private enum GameState
	{
		LoginScreen,
		LoggingIn,
		LoggedIn,
		FetchedPlayer,
	};

	private GameState m_gameState = GameState.LoginScreen;

	// Use this for initialization
	public void Login()
	{
		// Log in to the database
		m_gameState = GameState.LoggingIn;
		DatabaseAccess.Login(m_usernameInput.text, m_passwordInput.text, (loginSuccess, loginError) =>
				{
					if (loginSuccess)
					{
						DebugHelpers.Log("Logged in as player {0}", WebManager.LoggedInPlayerID);
						m_gameState = GameState.LoggedIn;

						// Request data for the player
						DatabaseAccess.GetPlayer((getSuccess, getError, player) =>
								{
									if (getSuccess)
									{
										DebugHelpers.Log("Player Fetched");
										m_gameState = GameState.FetchedPlayer;

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
										m_gameState = GameState.LoginScreen;

										// TODO: Display error dialog
									}
								}
							);
					}
					else
					{
						DebugHelpers.LogError("Login failed: {0}", loginError);
						m_gameState = GameState.LoginScreen;

						// TODO: Display error dialog
					}
				}
			);
	}
}
