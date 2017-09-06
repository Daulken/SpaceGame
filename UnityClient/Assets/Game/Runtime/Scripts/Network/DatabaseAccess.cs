using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;


public static class DatabaseAccess
{
	// The player ID for the current login
	// TODO: Set this to -1 until logged in. Requires the Login functionality first though
	private static int ms_playerID = 1;

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
					if (response.IsValid)
					{
						// Login was successful. Store the player ID for this account
						ms_playerID = WebManager.LoggedInPlayerID;
						result(true, "");
					}
					else
					{
						DebugHelpers.LogError("Login Error: {0} - {1}", response.ErrorCode, response.ErrorDescription);
						result(false, GetErrorMessage(response.ErrorCode));
					}
				}
			);
	}

	public static int LoggedInPlayerID
	{
		get
		{
			return ms_playerID;
		}
	}

	/// <summary>
	/// Fetch the current state of the logged in player
	/// </summary>
	/// <param name="result">Action containing whether the fetch was successful, a localised error message if not, and the fetched player if valid</param>
	public static void GetPlayer(Action<bool, string, SpaceLibrary.Player> result)
	{
		// If the user hasn't yet logged in, the player ID will not be known
		if (ms_playerID < 0)
		{
			result(false, GetErrorMessage(WebManager.ErrorCode.Game_InvalidCredentials), null);
			return;
		}

		// Get the player for the logged in player ID
		WebManager.RequestResponseText(WebManager.RequestType.Post, "SpaceService.asmx/GetPlayer", new Dictionary<string, string>() { { "PlayerId", ms_playerID.ToString() } }, null,
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
	/// Save the current state of the logged in player
	/// </summary>
	/// <param name="result">Action containing whether the fetch was successful, and a localised error message if not</param>
	public static void SavePlayer(SpaceLibrary.Player player, Action<bool, string> result)
	{
		// If the user hasn't yet logged in, the player ID will not be known
		if (ms_playerID < 0)
		{
			result(false, GetErrorMessage(WebManager.ErrorCode.Game_InvalidCredentials));
			return;
		}

		// Re-serialise the player
		string newPlayerData = JsonConvert.SerializeObject(player);

		// Save the player data
		WebManager.RequestResponseText(WebManager.RequestType.Post, "SpaceService.asmx/SavePlayer", new Dictionary<string, string>() { { "PlayerId", ms_playerID.ToString() }, { "PlayerData", newPlayerData } }, null,
				(WebManager.TextWebResponse response) =>
				{
					if (response.IsValid)
					{
						result(true, "");
					}
					else
					{
						DebugHelpers.LogError("SavePlayer Error: {0} - {1}", response.ErrorCode, response.ErrorDescription);
						result(false, GetErrorMessage(response.ErrorCode));
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
		if (ms_playerID < 0)
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
		if (ms_playerID < 0)
		{
			result(false, GetErrorMessage(WebManager.ErrorCode.Game_InvalidCredentials));
			return;
		}

		// Get the player for the logged in player ID
		WebManager.RequestResponseText(WebManager.RequestType.Post, "SpaceService.asmx/CreateMarketOrder",
				new Dictionary<string, string>() {
					{ "PlayerId", ms_playerID.ToString() },
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
