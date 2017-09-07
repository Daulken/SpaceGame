using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;


public static class DatabaseAccess
{
	// The most up to date cache of the player for the current login
	private static SpaceLibrary.Player ms_player = null;

	/// <summary>
	/// Get the localised message for a given error code
	/// </summary>
	private static string GetErrorMessage(WebManager.ErrorCode errorCode)
	{
		string localisedErrorDesc = Localization.Instance.Localize("WEBERROR_" + errorCode.ToString().ToUpper());
		return localisedErrorDesc;
	}

	/// <summary>
	/// Log into the database as a user
	/// </summary>
	/// <param name="result">Action containing whether the login was successful, a localised error message if not</param>
	public static void Login(string username, string password, Action<bool, string> result)
	{
		// Log in with the given username and password
		WebManager.LogIn(username, password,
				(WebManager.WebResponse response) =>
				{
					/*
					if (response.IsValid)
					{
						// Login was successful. Store the player ID for this account
						result(true, "");
					}
					else
					{
						DebugHelpers.LogError("Login Error: {0} - {1}", response.ErrorCode, response.ErrorDescription);
						result(false, GetErrorMessage(response.ErrorCode));
					}
					*/
					// Get the player for the logged in player ID
					WebManager.RequestResponseText(WebManager.RequestType.Post, "SpaceService.asmx/GetPlayer", new Dictionary<string, string>() { { "PlayerId", "1" } }, null,
							(WebManager.TextWebResponse playerResponse) =>
							{
								if (playerResponse.IsValid)
								{
									SpaceLibrary.Player player = JsonConvert.DeserializeObject<SpaceLibrary.Player>(playerResponse.ResponseText);
									if (player == null)
									{
										result(false, GetErrorMessage(WebManager.ErrorCode.Game_PlayerNotFound));
									}
									else
									{
										ms_player = player;
										result(true, "");
									}
								}
								else
								{
									result(false, GetErrorMessage(response.ErrorCode));
								}
							}
						);
				}
			);
	}

	public static SpaceLibrary.Player LoggedInPlayer
	{
		get
		{
			return ms_player;
		}
	}

	/// <summary>
	/// Fetch the current state of the logged in player
	/// </summary>
	/// <param name="result">Action containing whether the fetch was successful, a localised error message if not, and the fetched player if valid</param>
	public static void GetPlayer(Action<bool, string, SpaceLibrary.Player> result)
	{
		// If the user hasn't yet logged in, the player ID will not be known
		if (ms_player == null)
		{
			result(false, GetErrorMessage(WebManager.ErrorCode.Game_InvalidCredentials), null);
			return;
		}

		// Get the player for the logged in player ID
		WebManager.RequestResponseText(WebManager.RequestType.Post, "SpaceService.asmx/GetPlayer", new Dictionary<string, string>() { { "PlayerId", ms_player.PlayerId.ToString() } }, null,
				(WebManager.TextWebResponse response) =>
				{
					if (response.IsValid)
					{
						SpaceLibrary.Player player = JsonConvert.DeserializeObject<SpaceLibrary.Player>(response.ResponseText);
						if (player == null)
						{
							DebugHelpers.LogError("GetPlayer Error: Game_PlayerNotFound - Response was valid, but contained invalid player data");
							result(false, GetErrorMessage(WebManager.ErrorCode.Game_PlayerNotFound), null);
						}
						else
						{
							ms_player = player;
							result(true, "", player);
						}
					}
					else
					{
						DebugHelpers.LogError("GetPlayer Error: {0} - {1}", response.ErrorCode, response.ErrorDescription);
						result(false, GetErrorMessage(response.ErrorCode), null);
					}
				}
			);
	}

	/// <summary>
	/// Fetch the current market orders for a star
	/// </summary>
	/// <param name="result">Action containing whether the fetch was successful, a localised error message if not, and the fetched list of market orders if valid</param>
	public static void GetMarketOrders(int starId, Action<bool, string, List<SpaceLibrary.MarketOrder>> result)
	{
		// If the user hasn't yet logged in, the player ID will not be known
		if (ms_player == null)
		{
			result(false, GetErrorMessage(WebManager.ErrorCode.Game_InvalidCredentials), null);
			return;
		}

		// Get the player for the logged in player ID
		WebManager.RequestResponseText(WebManager.RequestType.Post, "SpaceService.asmx/GetMarketOrders", new Dictionary<string, string>() { { "StarId", starId.ToString() } }, null,
				(WebManager.TextWebResponse response) =>
				{
					if (response.IsValid)
					{
						List<SpaceLibrary.MarketOrder> marketOrders = JsonConvert.DeserializeObject<List<SpaceLibrary.MarketOrder>>(response.ResponseText);
						if (marketOrders == null)
							marketOrders = new List<SpaceLibrary.MarketOrder>();
						result(true, "", marketOrders);
					}
					else
					{
						DebugHelpers.LogError("GetMarketOrders Error: {0} - {1}", response.ErrorCode, response.ErrorDescription);
						result(false, GetErrorMessage(response.ErrorCode), null);
					}
				}
			);
	}

	/// <summary>
	/// Create a market order for a star
	/// </summary>
	/// <param name="result">Action containing whether the fetch was successful, a localised error message if not</param>
	public static void CreateMarketOrder(int starId, int buy, int materialId, int quantity, double price, Action<bool, string> result)
	{
		// If the user hasn't yet logged in, the player ID will not be known
		if (ms_player == null)
		{
			result(false, GetErrorMessage(WebManager.ErrorCode.Game_InvalidCredentials));
			return;
		}

		// Get the player for the logged in player ID
		WebManager.RequestResponseText(WebManager.RequestType.Post, "SpaceService.asmx/CreateMarketOrder",
				new Dictionary<string, string>() {
					{ "PlayerId", ms_player.PlayerId.ToString() },
					{ "StarId", starId.ToString() },
					{ "Buy", buy.ToString() },
					{ "MaterialId", materialId.ToString() },
					{ "Quantity", quantity.ToString() },
					{ "Price", price.ToString() },
				},
				null,
				(WebManager.TextWebResponse response) =>
				{
					if (response.IsValid)
					{
						result(true, "");
					}
					else
					{
						DebugHelpers.LogError("GetMarketOrders Error: {0} - {1}", response.ErrorCode, response.ErrorDescription);
						result(false, GetErrorMessage(response.ErrorCode));
					}
				}
			);
	}
}
