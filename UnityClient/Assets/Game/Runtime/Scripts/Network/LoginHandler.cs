using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[AddComponentMenu("SPACEJAM/Communication/LoginHandler")]
public class LoginHandler : MonoBehaviour
{
	public UnityEngine.UI.InputField m_usernameInput;
	public UnityEngine.UI.InputField m_passwordInput;

	// Use this for initialization
	public void Login()
	{
		LoginDialog.Instance.Show("LOGIN_INFO_LOGGING_IN");

		// Log in to the database
		DatabaseAccess.Login(m_usernameInput.text, m_passwordInput.text, (loginSuccess, loginError) =>
				{
					if (loginSuccess)
					{
						DebugHelpers.Log("Logged in as player {0}. Fetching", DatabaseAccess.LoggedInPlayerID);

						LoginDialog.Instance.Show("LOGIN_INFO_FETCHING_PLAYER");

						// Request data for the player
						DatabaseAccess.GetPlayer((getSuccess, getError, player) =>
								{
									if (getSuccess)
									{
										LoginDialog.Instance.Show("LOGIN_INFO_LOADING_WORLD");

										// Change game state to the system view
										GameplayManager.Instance.CurrentState = GameplayManager.State.System;

										LoginDialog.Instance.Hide();
									}
									else
									{
										// Display error dialog
										LoginDialog.Instance.Hide();
										InfoDialog.Instance.Show("LOGIN_ERROR_TITLE", getError, null);
									}
								}
							);
					}
					else
					{
						// Display error dialog
						LoginDialog.Instance.Hide();
						InfoDialog.Instance.Show("LOGIN_ERROR_TITLE", loginError, null);
					}
				}
			);
	}
}
