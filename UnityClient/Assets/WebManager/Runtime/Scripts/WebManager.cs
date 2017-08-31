using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;


public static class WebManager
{
	public enum RequestType
	{
		Get,
		Post,
		Put,
		Delete,
	};

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

	// Text specialised communication response
	public class TextWebResponse : WebResponse
	{
		private string m_responseText;

		public TextWebResponse(long errorCode, string errorString)
			: base(errorCode, errorString)
		{
			m_responseText = "";
		}

		public TextWebResponse(Dictionary<string, string> responseHeaders, string responseText)
			: base(responseHeaders)
		{
			m_responseText = responseText;
		}

		// Get the response, if the response is valid
		public string GetResponse()
		{
			return m_responseText;
		}
	}

	// Register a common header to add to every request
	public static void AddHeader(string header, string value)
	{
		WebCommunicator.GetInstance().AddHeaderInternal(header, value);
	}

	// Unregister a common header from every request
	public static void RemoveHeader(string header)
	{
		WebCommunicator.GetInstance().RemoveHeaderInternal(header);
	}

	// Get the value of a registered common header added to every request
	public static bool GetHeader(string header, out string value)
	{
		return WebCommunicator.GetInstance().GetHeaderInternal(header, out value);
	}

	// Make a request, and get a binary response
	public static void RequestResponseBinary(RequestType requestType, string subAddress, object parameters, Action<BinaryWebResponse> response)
	{
		WebCommunicator.GetInstance().StartCoroutine(WebCommunicator.GetInstance().RequestResponseBinaryInternal(requestType, subAddress, parameters, response));
	}

	// Make a request, and get a text response
	public static void RequestResponseText(RequestType requestType, string subAddress, object parameters, Action<TextWebResponse> response)
	{
		WebCommunicator.GetInstance().StartCoroutine(WebCommunicator.GetInstance().RequestResponseTextInternal(requestType, subAddress, parameters, response));
	}

}


// Component used for parallel processing communications
[AddComponentMenu("")]
internal class WebCommunicator : MonoBehaviour
{
	private static string WebAddress = "http://localhost:63465/";
	private static int TimeoutSeconds = 10;

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

	// Get an instance of the web communicator
	public static WebCommunicator GetInstance()
	{
		// If an instance exists already, do nothing
		if ((ms_instance != null) && (ms_instance.gameObject != null))
			return ms_instance;

		// Look for an instance of this type in the scene already, in case a singleton was added
		ms_instance = (WebCommunicator)GameObject.FindObjectOfType(typeof(WebCommunicator));

		// Automatically create one if not found
		if (ms_instance == null)
		{
			GameObject go = new GameObject("_Global_WebCommunicator");
			go.hideFlags = HideFlags.HideAndDontSave;
			ms_instance = go.AddComponent<WebCommunicator>();
		}

		return ms_instance;
	}

	// Register a common header to add to every request
	public void AddHeaderInternal(string header, string value)
	{
		m_headers.Add(header, value);
	}

	// Unregister a common header from every request
	public void RemoveHeaderInternal(string header)
	{
		m_headers.Remove(header);
	}

	// Get the value of a registered common header added to every request
	public bool GetHeaderInternal(string header, out string value)
	{
		return m_headers.TryGetValue(header, out value);
	}

	public IEnumerator RequestResponseBinaryInternal(WebManager.RequestType requestType, string subAddress, object parameters, Action<WebManager.BinaryWebResponse> response)
	{
		// Get the verb type to request
		string requestVerb = "";
		switch (requestType)
		{
		case WebManager.RequestType.Get:
			requestVerb = UnityWebRequest.kHttpVerbGET;
			break;
		case WebManager.RequestType.Post:
			requestVerb = UnityWebRequest.kHttpVerbPOST;
			break;
		case WebManager.RequestType.Put:
			requestVerb = UnityWebRequest.kHttpVerbPUT;
			break;
		case WebManager.RequestType.Delete:
			requestVerb = UnityWebRequest.kHttpVerbDELETE;
			break;
		}

		// Create the request, with common headers
		UnityWebRequest request = new UnityWebRequest(WebAddress + subAddress, requestVerb);
		foreach (string key in m_headers.Keys)
			request.SetRequestHeader(key, m_headers[key]);
		request.timeout = TimeoutSeconds;

		// If parameters have been specified
		if (parameters != null)
		{
			// Encode the object into a JSON string
			string jsonString = JsonConvert.SerializeObject(parameters);
			byte[] rawBytes = Encoding.ASCII.GetBytes(jsonString);

			// Assign the upload handler
			UploadHandlerRaw uploader = new UploadHandlerRaw(rawBytes);
			uploader.contentType = "application/json"; //this is ignored?
			request.uploadHandler = uploader;
			request.SetRequestHeader("Content-Type", "application/json");
		}

		// Send the request
		yield return request.Send();

		// If an error was found, return the error, otherwise return the result
		if (request.isNetworkError || request.isHttpError)
			response(new WebManager.BinaryWebResponse(request.responseCode, request.error));
		else
			response(new WebManager.BinaryWebResponse(request.GetResponseHeaders(), request.downloadHandler.data));
	}

	public IEnumerator RequestResponseTextInternal(WebManager.RequestType requestType, string subAddress, object parameters, Action<WebManager.TextWebResponse> response)
	{
		// Get the verb type to request
		string requestVerb = "";
		switch (requestType)
		{
		case WebManager.RequestType.Get:
			requestVerb = UnityWebRequest.kHttpVerbGET;
			break;
		case WebManager.RequestType.Post:
			requestVerb = UnityWebRequest.kHttpVerbPOST;
			break;
		case WebManager.RequestType.Put:
			requestVerb = UnityWebRequest.kHttpVerbPUT;
			break;
		case WebManager.RequestType.Delete:
			requestVerb = UnityWebRequest.kHttpVerbDELETE;
			break;
		}

		// Create the request, with common headers
		UnityWebRequest request = new UnityWebRequest(WebAddress + subAddress, requestVerb);
		foreach (string key in m_headers.Keys)
			request.SetRequestHeader(key, m_headers[key]);
		request.timeout = TimeoutSeconds;

		// If parameters have been specified
		if (parameters != null)
		{
			// Encode the object into a JSON string
			string jsonString = JsonConvert.SerializeObject(parameters);
			if (jsonString.Length > 0)
			{
				byte[] rawBytes = Encoding.ASCII.GetBytes(jsonString);

				// Assign the upload handler
				UploadHandlerRaw uploader = new UploadHandlerRaw(rawBytes);
				uploader.contentType = "application/json"; //this is ignored?
				request.uploadHandler = uploader;
				request.SetRequestHeader("Content-Type", "application/json");
			}
		}

		// Send the request
		yield return request.Send();

		// If an error was found, return the error, otherwise return the result
		if (request.isNetworkError || request.isHttpError)
			response(new WebManager.TextWebResponse(request.responseCode, request.error));
		else
			response(new WebManager.TextWebResponse(request.GetResponseHeaders(), request.downloadHandler.text));
	}
}
