using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


[AddComponentMenu("SPACEJAM/GameState/Login/LoginHandler")]
public class LoginHandler : MonoBehaviour
{
	public UnityEngine.UI.InputField m_usernameInput;
	public UnityEngine.UI.InputField m_passwordInput;

	protected void Start()
	{
		// Ensure that the user name is the first selected game object
		EventSystem.current.SetSelectedGameObject(m_usernameInput.gameObject);
	}

	// Use this for initialization
	public void Login()
	{
		MessageDialog.Instance.Show("LOGIN_INFO_LOGGING_IN");

		// Log in to the database
		DatabaseAccess.Login(m_usernameInput.text, m_passwordInput.text, (loginSuccess, loginError) =>
				{
					if (loginSuccess)
					{
						DebugHelpers.Log("Logged in as player {0}. Fetching", DatabaseAccess.LoggedInPlayerID);

						MessageDialog.Instance.Show("LOGIN_INFO_FETCHING_PLAYER");

						// Request data for the player
						DatabaseAccess.GetPlayer((getSuccess, getError, player) =>
								{
									if (getSuccess)
									{
										MessageDialog.Instance.Show("LOGIN_INFO_LOADING_WORLD");

										// TODO: Change game state to the system view initially
										//GameplayManager.Instance.CurrentState = GameplayManager.State.System;

										// Change game state to the market view initially
										GameplayManager.Instance.CurrentState = GameplayManager.State.Market;

										MessageDialog.Instance.Hide();
									}
									else
									{
										// Display error dialog
										MessageDialog.Instance.Hide();
										InfoDialog.Instance.Show("LOGIN_ERROR_TITLE", getError, null);
									}
								}
							);
					}
					else
					{
						// Display error dialog
						MessageDialog.Instance.Hide();
						InfoDialog.Instance.Show("LOGIN_ERROR_TITLE", loginError, null);
					}
				}
			);
	}
}
