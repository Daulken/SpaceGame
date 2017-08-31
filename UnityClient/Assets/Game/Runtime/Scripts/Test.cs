﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenSimplexNoise;
using Newtonsoft.Json;
using SpaceLibrary;


[AddComponentMenu("SPACEJAM/Test")]
public class Test : MonoBehaviour
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

	//private OpenSimplexNoise.OpenSimplexNoise m_noise = new OpenSimplexNoise.OpenSimplexNoise();

	// Use this for initialization
	void Start()
	{
		//DebugHelpers.Log("{0}={1}", m_stringID, Localization.Localize(m_stringID));

		WebManager.LogIn("chris", "flibble",
				(WebManager.WebResponse response) =>
				{
					if (!response.IsValid)
					{
						// Log this error. Should be Game_InvalidCredentials if there wasn't an HTTP error
						string localisedErrorDesc = Localization.Localize("WEBERROR_" + response.ErrorCode.ToString().ToUpper());
						DebugHelpers.LogError("Login error: {0} ({1}) - {2}", response.ErrorCode, localisedErrorDesc, response.ErrorDescription);
					}
				}
			);

		// Request data for a player
		WebManager.RequestResponseText(WebManager.RequestType.Post, "SpaceService.asmx/GetPlayer", new Dictionary<string, string>() { { "PlayerId", "0" } }, null,
				(WebManager.TextWebResponse response) =>
				{
					if (response.IsValid)
					{
						SpaceLibrary.Player player = JsonConvert.DeserializeObject<SpaceLibrary.Player>(response.ResponseText);
						DebugHelpers.Log("Player={0}", player.ToString());
					}
					else
					{
						// Log this error
						string localisedErrorDesc = Localization.Localize("WEBERROR_" + response.ErrorCode.ToString().ToUpper());
						DebugHelpers.LogError("GetPlayer error: {0} ({1}) - {2}", response.ErrorCode, localisedErrorDesc, response.ErrorDescription);
					}
				}
			);
	}

	protected void Update()
	{
	
	}

}
