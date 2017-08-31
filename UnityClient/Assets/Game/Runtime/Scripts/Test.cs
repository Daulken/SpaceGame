using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenSimplexNoise;
using Newtonsoft.Json;


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

	private static string PasswordSalt = "testSalt";

	// Use this for initialization
	void Start()
	{
		DebugHelpers.Log("{0}={1}", m_stringID, Localization.Localize(m_stringID));

		// Log in to secure server
		//Dictionary<string, string> parameters = new Dictionary<string, string>();
		//parameters["username"] = "Chris";
		//parameters["password"] = PasswordSalt + "password";
		//WebManager.RequestResponseText(WebManager.RequestType.Post, "SpaceService.svc/Login/", parameters, OnLoginResponse);

		WebManager.RequestResponseText(WebManager.RequestType.Get, "SpaceService.svc/GetPlayerList/", null, OnSearchResponse);
	}

	private void OnLoginResponse(WebManager.TextWebResponse response)
	{
		if (response.IsValid())
		{
			Dictionary<string, string> headers = response.GetResponseHeaders();
			string header = "";
			foreach (string key in headers.Keys)
				header += string.Format("Header: {0} = {1}\n", key, headers[key]);
			DebugHelpers.Log("TextWebResponse.\n{0}\nResponse: {1}", header, response.GetResponse());

			// Add authentication token for all other queries
			WebManager.AddHeader("X-RWPVT", response.GetResponse());
		}
		else
		{
			// Log this error
			DebugHelpers.LogError("POST error: {0} - {1}", response.GetErrorCode(), response.GetErrorDescription());
		}
	}

	public class Player
	{
		public int PlayerId
		{
			get;
			set;
		}

		public string PlayerName
		{
			get;
			set;
		}
	}

	private void OnSearchResponse(WebManager.TextWebResponse response)
	{
		if (response.IsValid())
		{
			Dictionary<string, string> headers = response.GetResponseHeaders();
			string header = "";
			foreach (string key in headers.Keys)
				header += string.Format("Header: {0} = {1}\n", key, headers[key]);
			DebugHelpers.Log("TextWebResponse.\n{0}\nResponse: {1}", header, response.GetResponse());

			Player[] players = JsonConvert.DeserializeObject<Player[]>(response.GetResponse());
		}
		else
		{
			// Log this error
			DebugHelpers.LogError("POST error: {0} - {1}", response.GetErrorCode(), response.GetErrorDescription());
		}
	}
}
