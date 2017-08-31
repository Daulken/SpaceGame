using System.Collections;
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

		// Log in to secure server
		WebManager.RequestResponseText(WebManager.RequestType.Post, "SpaceService.asmx/Login", new Dictionary<string, string>() { { "username", "Chris" }, { "password", "GVsdftghiovhiovgvhk" }, }, null,
				(WebManager.TextWebResponse response) =>
				{
					if (response.IsValid)
					{
						// Add authentication token for all other queries
						WebManager.AddHeader("X-RWPVT", response.ResponseText);
						DebugHelpers.Log("AuthToken={0}", response.ResponseText);
					}
					else
					{
						// Log this error
						DebugHelpers.LogError("POST error: {0} - {1}", response.ErrorCode, response.ErrorDescription);
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
						DebugHelpers.LogError("POST error: {0} - {1}", response.ErrorCode, response.ErrorDescription);
					}
				}
			);
	}


}
