using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;


[AddComponentMenu("SPACEJAM/NetworkTest")]
public class NetworkTest : MonoBehaviour
{
	[LocalizationKey]
	public string m_stringID;

	public enum BitMasks
	{
		A = 0x01,
		B = 0x02,
		C = 0x04,
		D = 0x08
	};

	[BitMask(typeof(BitMasks))]
	public BitMasks m_bitfield = BitMasks.A | BitMasks.C;

	// Use this for initialization
	void Start()
	{
/*
		WebManager.LogIn("chris", "flibble",
				(WebManager.WebResponse response) =>
				{
					if (!response.IsValid)
					{
						// Log this error. Should be Game_InvalidCredentials if there wasn't an HTTP error
						string localisedErrorDesc = Localization.Instance.Localize("WEBERROR_" + response.ErrorCode.ToString().ToUpper());
						DebugHelpers.LogError("Login error: {0} ({1}) - {2}", response.ErrorCode, localisedErrorDesc, response.ErrorDescription);
					}
				}
			);
*/

		// Request data for a player
		WebManager.RequestResponseText(WebManager.RequestType.Post, "SpaceService.asmx/GetPlayer", new Dictionary<string, string>() { { "PlayerId", "1" } }, null,
				(WebManager.TextWebResponse getResponse) =>
				{
					if (getResponse.IsValid)
					{
						SpaceLibrary.Player player = JsonConvert.DeserializeObject<SpaceLibrary.Player>(getResponse.ResponseText);
						DebugHelpers.Log("Player={0}", player.ToString());

						// Increase the max crew count
						player.MaxCrew += 1;

						// Re-serialise the player
						string newPlayerData = JsonConvert.SerializeObject(player);

						// Save the player data
						WebManager.RequestResponseText(WebManager.RequestType.Post, "SpaceService.asmx/SavePlayer", new Dictionary<string, string>() { { "PlayerId", "1" }, { "PlayerData", newPlayerData } }, null,
								(WebManager.TextWebResponse saveResponse) =>
								{
									if (saveResponse.IsValid)
									{
										DebugHelpers.Log("Player saved (Response=\"{0}\")", saveResponse.ResponseText);
									}
									else
									{
										// Log this error
										string localisedErrorDesc = Localization.Instance.Localize("WEBERROR_" + saveResponse.ErrorCode.ToString().ToUpper());
										DebugHelpers.LogError("SavePlayer error: {0} ({1}) - {2}", saveResponse.ErrorCode, localisedErrorDesc, saveResponse.ErrorDescription);
									}
								}
							);

					}
					else
					{
						// Log this error
						string localisedErrorDesc = Localization.Instance.Localize("WEBERROR_" + getResponse.ErrorCode.ToString().ToUpper());
						DebugHelpers.LogError("GetPlayer error: {0} ({1}) - {2}", getResponse.ErrorCode, localisedErrorDesc, getResponse.ErrorDescription);
					}
				}
			);
	}

}
