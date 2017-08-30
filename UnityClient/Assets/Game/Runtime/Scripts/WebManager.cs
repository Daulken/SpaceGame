using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;
using UnityEngine.Networking;
using MiniJSON;


public static class WebManager
{
	// Generic communication response
	public class WebResponse
	{
		protected long m_carrierErrorCode;
		protected string m_errorString;
		protected Dictionary<string, string> m_responseHeaders;

		public WebResponse(long errorCode, string errorString)
		{
			m_carrierErrorCode = errorCode;
			m_errorString = errorString;
			m_responseHeaders = new Dictionary<string, string>();
		}

		public WebResponse(Dictionary<string, string> responseHeaders)
		{
			m_carrierErrorCode = 0;
			m_errorString = "";
			m_responseHeaders = responseHeaders;
		}

		// Query whether the response is valid, or had an error
		public bool IsValid()
		{
			return m_carrierErrorCode == 0;
		}

		// Get any error code, if the response isn't valid
		public long GetErrorCode()
		{
			return m_carrierErrorCode;
		}

		// Get any description of the error, if the response isn't valid
		public string GetErrorDescription()
		{
			return m_errorString;
		}

		// Get the response headers, if the response is valid
		public Dictionary<string, string> GetResponseHeaders()
		{
			return (m_carrierErrorCode == 0) ? m_responseHeaders : new Dictionary<string, string>();
		}
	}

	// Binary specialised communication response
	public class BinaryWebResponse : WebResponse
	{
		private byte[] m_responseData;

		public BinaryWebResponse(long errorCode, string errorString)
			: base(errorCode, errorString)
		{
			m_responseData = new byte[0];
		}

		public BinaryWebResponse(Dictionary<string, string> responseHeaders, byte[] responseData)
			: base(responseHeaders)
		{
			m_responseData = responseData;
		}

		// Get the response data, if the response is valid
		public byte[] GetResponse()
		{
			return m_responseData;
		}
	}

	// JSON specialised communication response
	public class JsonWebResponse : WebResponse
	{
		private Dictionary<string, object> m_json;
		private long m_responseError;

		public JsonWebResponse(long errorCode, string errorString)
			: base(errorCode, errorString)
		{
			m_json = new Dictionary<string, object>();
		}

		public JsonWebResponse(Dictionary<string, string> responseHeaders, string responseText)
			: base(responseHeaders)
		{
			// Parse the response as a JSON dictionary
			Dictionary<string, object> json = Json.Deserialize(responseText) as Dictionary<string, object>;

			// Set up an error if the JSON object could not be parsed
			if (json == null)
			{
				m_responseError = 404;
				m_errorString = "Response is not a JSON string";
			}
			// JSON could be parsed. Check if there is a valid result in it
			else if (json.ContainsKey("error"))
			{
				m_responseError = (long)json["error"];
				if (json.ContainsKey("errorstring"))
					m_errorString = (string)json["errorstring"];
			}
			// No known error, but check if a result was given
			else if (!m_json.ContainsKey("result"))
			{
				m_responseError = 404;
				m_errorString = "Response JSON does not contain a result";
			}
			// Valid response
			else
			{
				m_json = (Dictionary<string, object>)json["result"];
			}
		}

		// Query whether the response is valid, or had an error
		public new bool IsValid()
		{
			return base.IsValid() && (m_responseError == 0);
		}

		// Get any error code, if the response isn't valid
		public new long GetErrorCode()
		{
			if (m_responseError != 0)
				return m_responseError;

			return base.GetErrorCode();
		}

		// Get the JSON response, if the response is valid
		public Dictionary<string, object> GetResponse()
		{
			return m_json;
		}
	}

	// Register a common header to add to every request
	public static void AddHeader(string header, string value)
	{
		WebCommunicator.GetInstance().AddHeader(header, value);
	}

	// Unregister a common header from every request
	public static void RemoveHeader(string header)
	{
		WebCommunicator.GetInstance().RemoveHeader(header);
	}

	// Get the value of a registered common header added to every request
	public static bool GetHeader(string header, out string value)
	{
		return WebCommunicator.GetInstance().GetHeader(header, out value);
	}

	// Get a Binary response
	public static void GetResponseBinary(string subAddress, Action<BinaryWebResponse> response)
	{
		// Get the JSON response content in parallel
		WebCommunicator.GetInstance().StartCoroutine(WebCommunicator.GetInstance().GetResponseBinaryInternal(subAddress, response));
	}

	// Get a JSON response
	public static void GetResponseJSON(string subAddress, Action<JsonWebResponse> response)
	{
		// Get the JSON response content in parallel
		WebCommunicator.GetInstance().StartCoroutine(WebCommunicator.GetInstance().GetResponseJSONInternal(subAddress, response));
	}

	// Post a JSON object, and get a binary response
	public static void PostJSONResponseBinary(string subAddress, Dictionary<string, object> jsonContent, Action<BinaryWebResponse> response)
	{
		// Post the JSON message content in parallel
		WebCommunicator.GetInstance().StartCoroutine(WebCommunicator.GetInstance().PostJSONResponseBinaryInternal(subAddress, jsonContent, response));
	}

	// Post a JSON object, and get a JSON response
	public static void PostJSONResponseJSON(string subAddress, Dictionary<string, object> jsonContent, Action<JsonWebResponse> response)
	{
		// Post the JSON message content in parallel
		WebCommunicator.GetInstance().StartCoroutine(WebCommunicator.GetInstance().PostJSONResponseJSONInternal(subAddress, jsonContent, response));
	}
}


// Component used for parallel processing communications
[AddComponentMenu("")]
internal class WebCommunicator : MonoBehaviour
{
	private static string WebAddress = "http://www.posttestserver.com";
	private static int TimeoutSeconds = 60;

	private static WebCommunicator ms_instance = null;
	private Dictionary<string, string> m_headers = new Dictionary<string, string>();

	// Awake is called at game startup, regardless of enable state
	private void Awake()
	{
		// If no instance exists, use this one
		if (ms_instance == null)
			ms_instance = this;

		// If we're not the instance that things use
		if (ms_instance != this)
		{
			// Remove ourself, as we're duplicate
			bool destroyGameObject = true;
			Component[] components = gameObject.GetComponents(typeof(Component));
			foreach (Component component in components)
			{
				if (component is Transform)
					continue;
				if (component is WebCommunicator)
					continue;
				destroyGameObject = false;
				break;
			}

			if (destroyGameObject)
				Destroy(gameObject);
			else
				Destroy(this);
		}
		// This is our instance
		else
		{
			// Ensure we don't get destroyed between scenes
			DontDestroyOnLoad(gameObject);
		}
	}

	// Called when this object is destroyed
	protected virtual void OnDestroy()
	{
		// Remove our global instance pointer
		if (ms_instance == this)
			ms_instance = null;
	}

	// Ensures that there is an instance available to work with
	private static void EnsureInstanceExists()
	{
		// If an instance exists already, do nothing
		if ((ms_instance != null) && (ms_instance.gameObject != null))
			return;

		// Look for an instance of this type in the scene already, in case a singleton was added
		ms_instance = (WebCommunicator)GameObject.FindObjectOfType(typeof(WebCommunicator));

		// Automatically create one if not found
		if (ms_instance == null)
		{
			GameObject go = new GameObject("_Global_WebCommunicator");
			go.hideFlags = HideFlags.HideAndDontSave;
			ms_instance = go.AddComponent<WebCommunicator>();
		}
	}

	// Get the instance of the web communicator
	public static WebCommunicator GetInstance()
	{
		EnsureInstanceExists();
		return ms_instance;
	}

	public void AddHeader(string header, string value)
	{
		m_headers.Add(header, value);
	}

	public void RemoveHeader(string header)
	{
		m_headers.Remove(header);
	}

	public bool GetHeader(string header, out string value)
	{
		return m_headers.TryGetValue(header, out value);
	}

	public IEnumerator GetResponseBinaryInternal(string subAddress, Action<WebManager.BinaryWebResponse> response)
	{
		// Create a response downloader
		DownloadHandler downloader = new DownloadHandlerBuffer();

		// Post the JSON data to the web address
		UnityWebRequest request = new UnityWebRequest(WebAddress + subAddress, UnityWebRequest.kHttpVerbGET, downloader, null);
		request.redirectLimit = 0;
		request.timeout = TimeoutSeconds;
		foreach (string key in m_headers.Keys)
			request.SetRequestHeader(key, m_headers[key]);
		yield return request.Send();
		while (!request.isDone)
			yield return new WaitForEndOfFrame();

		// If an error was found, return the error, otherwise return the result
		if (request.isNetworkError || request.isHttpError)
			response(new WebManager.BinaryWebResponse(request.responseCode, request.error));
		else
			response(new WebManager.BinaryWebResponse(request.GetResponseHeaders(), downloader.data));
	}

	public IEnumerator GetResponseJSONInternal(string subAddress, Action<WebManager.JsonWebResponse> response)
	{
		// Create a response downloader
		DownloadHandler downloader = new DownloadHandlerBuffer();

		// Post the JSON data to the web address
		UnityWebRequest request = new UnityWebRequest(WebAddress + subAddress, UnityWebRequest.kHttpVerbGET, downloader, null);
		request.redirectLimit = 0;
		request.timeout = TimeoutSeconds;
		foreach (string key in m_headers.Keys)
			request.SetRequestHeader(key, m_headers[key]);
		yield return request.Send();
		while (!request.isDone)
			yield return new WaitForEndOfFrame();

		// If an error was found, return the error, otherwise return the result
		if (request.isNetworkError || request.isHttpError)
			response(new WebManager.JsonWebResponse(request.responseCode, request.error));
		else
			response(new WebManager.JsonWebResponse(request.GetResponseHeaders(), downloader.text));
	}

	public IEnumerator PostJSONResponseBinaryInternal(string subAddress, Dictionary<string, object> jsonContent, Action<WebManager.BinaryWebResponse> response)
	{
		// Encode the JSON object into a string, and get the raw bytes
		string jsonString = Json.Serialize(jsonContent);
		byte[] rawBytes = Encoding.ASCII.GetBytes(jsonString);

		// Create a JSON upload handler
		UploadHandler uploader = new UploadHandlerRaw(rawBytes);
		uploader.contentType = "application/json";

		// Create a response downloader
		DownloadHandler downloader = new DownloadHandlerBuffer();

		// Post the JSON data to the web address
		UnityWebRequest request = new UnityWebRequest(WebAddress + subAddress, UnityWebRequest.kHttpVerbPOST, downloader, uploader);
		request.redirectLimit = 0;
		request.timeout = TimeoutSeconds;
		foreach (string key in m_headers.Keys)
			request.SetRequestHeader(key, m_headers[key]);
		yield return request.Send();
		while (!request.isDone)
			yield return new WaitForEndOfFrame();

		// If an error was found, return the error, otherwise return the result
		if (request.isNetworkError || request.isHttpError)
			response(new WebManager.BinaryWebResponse(request.responseCode, request.error));
		else
			response(new WebManager.BinaryWebResponse(request.GetResponseHeaders(), downloader.data));
	}

	public IEnumerator PostJSONResponseJSONInternal(string subAddress, Dictionary<string, object> jsonContent, Action<WebManager.JsonWebResponse> response)
	{
		// Encode the JSON object into a string, and get the raw bytes
		string jsonString = Json.Serialize(jsonContent);
		byte[] rawBytes = Encoding.ASCII.GetBytes(jsonString);

		// Create a JSON upload handler
		UploadHandler uploader = new UploadHandlerRaw(rawBytes);
		uploader.contentType = "application/json";

		// Create a response downloader
		DownloadHandler downloader = new DownloadHandlerBuffer();

		// Post the JSON data to the web address
		UnityWebRequest request = new UnityWebRequest(WebAddress + subAddress, UnityWebRequest.kHttpVerbPOST, downloader, uploader);
		request.redirectLimit = 0;
		request.timeout = TimeoutSeconds;
		foreach (string key in m_headers.Keys)
			request.SetRequestHeader(key, m_headers[key]);
		yield return request.Send();
		while (!request.isDone)
			yield return new WaitForEndOfFrame();

		// If an error was found, return the error, otherwise return the result
		if (request.isNetworkError || request.isHttpError)
			response(new WebManager.JsonWebResponse(request.responseCode, request.error));
		else
			response(new WebManager.JsonWebResponse(request.GetResponseHeaders(), downloader.text));
	}
}
