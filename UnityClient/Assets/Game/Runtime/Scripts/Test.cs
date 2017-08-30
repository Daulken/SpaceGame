using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenSimplexNoise;


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
		DebugHelpers.Log("{0}={1}", m_stringID, Localization.Localize(m_stringID));

		Dictionary<string, object> fields = new Dictionary<string, object>();
		fields["num"] = 30;
		fields["q"] = "test";
		fields["oq"] = "test";
		WebManager.PostJSONResponseJSON("/post.php?dump", fields, OnSearchResponse);

		WebManager.AddHeader("X-RWPVT", "test_token");

		WebManager.GetResponseJSON(string.Format("/post.php?dump"), OnSearchResponse);
	}

	private void OnSearchResponse(WebManager.JsonWebResponse response)
	{
		if (response.IsValid())
		{
			Dictionary<string, string> headers = response.GetResponseHeaders();
			string header = "";
			foreach (string key in headers.Keys)
				header += string.Format("Header: {0} = {1}\n", key, headers[key]);
			DebugHelpers.Log("GET complete.\n{0}\nResponse: {1}", header, response.GetResponse());
		}
		else
		{
			// Log this error
			DebugHelpers.LogError("POST error: {0} - {1}", response.GetErrorCode(), response.GetErrorDescription());
		}
	}
}
