using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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

	public enum ErrorCode
	{
		// 0xx Unity
		Unity_NoHost = 0,

		// 1xx Informational
		HTTP_Continue = 100,
		HTTP_SwitchingProtocols = 101,
		HTTP_WebDAV_Processing = 102,

		// 2xx Success
		HTTP_OK = 200,
		HTTP_Created = 201,
		HTTP_Accepted = 202,
		HTTP_NonAuthoritativeInfo = 203,
		HTTP_NoContent = 204,
		HTTP_ResetContent = 205,
		HTTP_PartialContent = 206,
		HTTP_MultiStatus = 207,
		HTTP_WebDAV_AlreadyReported = 208,
		HTTP_IMUsed = 225,

		// 3xx Redirection
		HTTP_MultipleChoices = 300,
		HTTP_MovedPermanently = 301,
		HTTP_Found = 302,
		HTTP_SeeOther = 303,
		HTTP_NotModified = 304,
		HTTP_UseProxy = 305,
		HTTP_Unused = 306,
		HTTP_TemporaryRedirect = 307,
		HTTP_PermanentRedirect = 308,

		// 4xx Client Error
		HTTP_BadRequest = 400,
		HTTP_Unauthorised = 401,
		HTTP_PaymentRequired = 402,
		HTTP_Forbidden = 403,
		HTTP_NotFound = 404,
		HTTP_MethodNotAllowed = 405,
		HTTP_NotAcceptable = 406,
		HTTP_ProxyAuthenticationRequired = 407,
		HTTP_RequestTimeout = 408,
		HTTP_Conflict = 409,
		HTTP_Gone = 410,
		HTTP_LengthRequired = 411,
		HTTP_PreconditionFailed = 412,
		HTTP_RequestEntityTooLarge = 413,
		HTTP_RequestURLTooLong = 414,
		HTTP_UnsupportedMediaType = 415,
		HTTP_RequestedRangeNotSatisfiable = 416,
		HTTP_ExpectationFailed = 417,
		HTTP_RFC2324_ImATeapot = 418,
		HTTP_Twitter_EnhanceYourCalm = 420,
		HTTP_WebDAV_UnprocessableEntity = 422,
		HTTP_WebDAV_Locked = 423,
		HTTP_WebDAV_FailedDependency = 424,
		HTTP_WebDAV_Reserved = 425,
		HTTP_UpgradeRequired = 426,
		HTTP_PreconditionRequired = 428,
		HTTP_TooManyRequests = 429,
		HTTP_RequestHeaderFieldsTooLarge = 431,
		HTTP_Nginx_NoResponse = 444,
		HTTP_Microsoft_RetryWith = 449,
		HTTP_Microsoft_BlockedByWindowsParentalControls = 450,
		HTTP_UnavailableForLegalPurposes = 451,
		HTTP_Nginx_ClientClosedRequest = 499,

		// 5xx Server Error
		HTTP_InternalServerError = 500,
		HTTP_NotImplemented = 501,
		HTTP_BadGateway = 502,
		HTTP_ServiceUnavailable = 503,
		HTTP_GatewayTimeout = 504,
		HTTP_HTTPVersionNotSupported = 505,
		HTTP_VariantAlsoNegotiates = 506,
		HTTP_WebDAV_InsufficientStorage = 507,
		HTTP_WebDAV_LoopDetected = 508,
		HTTP_Apache_BandwidthLimitReached = 509,
		HTTP_NotExtended = 510,
		HTTP_NetworkAuthenticationRequired = 511,
		HTTP_NetworkReadTimeoutError = 598,
		HTTP_NetworkConnectTimeoutError = 599,

		// 10xx Game Data Errors
		Game_PlayerNotFound = 1000,

		// 11xx Game Connection Errors
		Game_InvalidResponse = 1100,
		Game_InvalidCredentials = 1101,
		Game_InvalidGameVersion = 1102,
	};


	// Generic communication response
	public class WebResponse
	{
		protected ErrorCode m_errorCode;
		protected string m_errorString;
		protected Dictionary<string, string> m_responseHeaders;
		protected bool m_valid = false;

		public WebResponse(ErrorCode errorCode, string errorString, string errorDetail)
		{
			m_errorCode = errorCode;
			m_errorString = string.Format("{0}: {1}", errorString, errorDetail);
			m_responseHeaders = new Dictionary<string, string>();
		}

		public WebResponse(Dictionary<string, string> responseHeaders)
		{
			m_errorCode = ErrorCode.HTTP_OK;
			m_errorString = "";
			m_responseHeaders = responseHeaders;
			m_valid = true;
		}

		// Query whether the response is valid, or had an error
		public bool IsValid
		{
			get
			{
				return m_valid;
			}
		}

		// Get any error code, if the response isn't valid
		public ErrorCode ErrorCode
		{
			get
			{
				return m_errorCode;
			}
		}

		// Get any description of the error, if the response isn't valid
		public string ErrorDescription
		{
			get
			{
				return m_errorString;
			}
		}

		// Get the response headers, if the response is valid
		public Dictionary<string, string> ResponseHeaders
		{
			get
			{
				return m_valid ? m_responseHeaders : new Dictionary<string, string>();
			}
		}
	}

	// Binary specialised communication response
	public class BinaryWebResponse : WebResponse
	{
		private byte[] m_responseData;

		public BinaryWebResponse(ErrorCode errorCode, string errorString, string errorDetail)
			: base(errorCode, errorString, errorDetail)
		{
			m_responseData = new byte[0];
		}

		public BinaryWebResponse(Dictionary<string, string> responseHeaders, byte[] responseData)
			: base(responseHeaders)
		{
			m_responseData = responseData;
		}

		// Get the response data, if the response is valid
		public byte[] ResponseData
		{
			get
			{
				return m_responseData;
			}
		}
	}

	// Text specialised communication response
	public class TextWebResponse : WebResponse
	{
		private string m_responseText;
		private string m_responseType;
		private string m_responseVersion;

		private class ServiceWrapper
		{
			public bool ResponseSuccessful
			{
				get; set;
			}
			public int ResponseCode
			{
				get; set;
			}
			public string ResponseMessage
			{
				get; set;
			}
			public string ReturnedDataType
			{
				get; set;
			}
			public string ReturnedDataTypeVersion
			{
				get; set;
			}
			public string ReturnedJsonData
			{
				get; set;
			}
		}

		public TextWebResponse(ErrorCode errorCode, string errorString, string errorDetail)
			: base(errorCode, errorString, errorDetail)
		{
			m_responseText = "";
		}

		public TextWebResponse(Dictionary<string, string> responseHeaders, string responseText)
			: base(responseHeaders)
		{
			Match match = Regex.Match(responseText, ".*<string.*>(.*)</string>", RegexOptions.Multiline);
			if (match.Success)
			{
				ServiceWrapper response = JsonConvert.DeserializeObject<ServiceWrapper>(match.Groups[1].Value);
				if (response.ResponseSuccessful)
				{
					m_responseText = response.ReturnedJsonData;
					m_responseType = response.ReturnedDataType;
					m_responseVersion = response.ReturnedDataTypeVersion;
				}
				else
				{
					m_errorCode = (ErrorCode)response.ResponseCode;
					m_errorString = response.ResponseMessage;
					m_valid = false;
				}
			}
			else
			{
				m_errorCode = ErrorCode.Game_InvalidResponse;
				m_errorString = string.Format("Response is not a valid ServiceWrapper: {0}", responseText);
				m_valid = false;
			}
		}

		// Get the response JSON, if the response is valid
		public string ResponseText
		{
			get
			{
				return m_responseText;
			}
		}

		// Get the response type, if the response is valid
		public string ResponseType
		{
			get
			{
				return m_responseType;
			}
		}
		// Get the response version, if the response is valid
		public string ResponseVersion
		{
			get
			{
				return m_responseVersion;
			}
		}
	}

	// Log in using given credentials
	public static void LogIn(string username, string password, Action<WebResponse> response)
	{
		WebCommunicator.Instance.StartCoroutine(WebCommunicator.Instance.RequestLogInInternal(username, password, response));
	}

	// Query the currently logged in player ID
	public static int LoggedInPlayerID
	{
		get
		{
			return WebCommunicator.Instance.LoggedInPlayerID;
		}
	}

	// Register a common header to add to every request
	public static void AddHeader(string header, string value)
	{
		WebCommunicator.Instance.AddHeaderInternal(header, value);
	}

	// Unregister a common header from every request
	public static void RemoveHeader(string header)
	{
		WebCommunicator.Instance.RemoveHeaderInternal(header);
	}

	// Get the value of a registered common header added to every request
	public static bool GetHeader(string header, out string value)
	{
		return WebCommunicator.Instance.GetHeaderInternal(header, out value);
	}

	// Make a request, and get a binary response
	public static void RequestResponseBinary(RequestType requestType, string subAddress, Dictionary<string, string> formData, object jsonPutData, Action<BinaryWebResponse> response)
	{
		WebCommunicator.Instance.StartCoroutine(WebCommunicator.Instance.RequestResponseInternal(requestType, subAddress, formData, jsonPutData,
				(UnityWebRequest request) =>
				{
					// If an error was found, return the error, otherwise return the result
					if (request.isNetworkError || request.isHttpError)
						response(new WebManager.BinaryWebResponse((ErrorCode)request.responseCode, request.error, request.downloadHandler.text));
					else
						response(new WebManager.BinaryWebResponse(request.GetResponseHeaders(), request.downloadHandler.data));
				}
			));
	}

	// Make a request, and get a text response
	public static void RequestResponseText(RequestType requestType, string subAddress, Dictionary<string, string> formData, object jsonPutData, Action<TextWebResponse> response)
	{
		WebCommunicator.Instance.StartCoroutine(WebCommunicator.Instance.RequestResponseInternal(requestType, subAddress, formData, jsonPutData,
				(UnityWebRequest request) =>
				{
					// If an error was found, return the error, otherwise return the result
					if (request.isNetworkError || request.isHttpError)
						response(new WebManager.TextWebResponse((ErrorCode)request.responseCode, request.error, request.downloadHandler.text));
					else
						response(new WebManager.TextWebResponse(request.GetResponseHeaders(), request.downloadHandler.text));
				}
			));
	}

}


// Component used for parallel processing communications
[AddComponentMenu("")]
internal class WebCommunicator : Singleton<WebCommunicator>
{
	private const string WebAddress = "http://localhost:55271/";		// Server to connect to
	private const int TimeoutSeconds = 10;								// Connection timeout
	private const int GameVersion = 1;                                  // Current game data version

	private Dictionary<string, string> m_headers = new Dictionary<string, string>();

	private int m_loggedInPlayerID = -1;
	public int LoggedInPlayerID
	{
		get
		{
			return m_loggedInPlayerID;
		}
	}

	private class LoginResponse
	{
		public string AuthToken
		{
			get; set;
		}
		public int Version
		{
			get; set;
		}
		public int PlayerID
		{
			get; set;
		}
	};

	// Guarantee this will be always a singleton only - make the constructor protected!
	protected WebCommunicator()
	{
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

	public IEnumerator RequestLogInInternal(string username, string password, Action<WebManager.WebResponse> response)
	{
		// Attempt to log in to secure server using the calculated password hash.
		// A proper HTTPS connection should have been established for this, to prevent plain text password interception
		Dictionary<string, string> formData = new Dictionary<string, string>() { { "username", username }, { "password", password } };
		UnityWebRequest request = UnityWebRequest.Post(WebAddress + "SpaceService.asmx/Login", formData);
		request.timeout = TimeoutSeconds;
		yield return request.Send();

		// If an error was found, return the error, otherwise return the result
		if (request.isNetworkError || request.isHttpError)
		{
			// Give a response to anything that may want it
			response(new WebManager.WebResponse((WebManager.ErrorCode)request.responseCode, request.error, request.downloadHandler.text));
		}
		// Query was valid
		else
		{
			// Parse the response
			WebManager.TextWebResponse internalResponse = new WebManager.TextWebResponse(request.GetResponseHeaders(), request.downloadHandler.text);

			// Check response
			LoginResponse loginData = JsonConvert.DeserializeObject<LoginResponse>(internalResponse.ResponseText);
			if (loginData == null)
			{
				// Give a response to anything that may want it
				response(new WebManager.WebResponse(WebManager.ErrorCode.Game_InvalidResponse, "Login Error", "No login response given"));
			}
			// Check version
			else if (loginData.Version != GameVersion)
			{
				// Give a response to anything that may want it
				response(new WebManager.WebResponse(WebManager.ErrorCode.Game_InvalidGameVersion, "Login Error", "Game client running incorrect version"));
			}
			else
			{
				// Add authentication token for all other queries
				AddHeaderInternal("X-RWPVT", loginData.AuthToken);

				// Set logged in player ID
				m_loggedInPlayerID = loginData.PlayerID;

				// Response with success
				response(new WebManager.WebResponse(internalResponse.ResponseHeaders));
			}
		}
	}

	public IEnumerator RequestResponseInternal(WebManager.RequestType requestType, string subAddress, Dictionary<string, string> formData, object jsonPutData, Action<UnityWebRequest> response)
	{
		UnityWebRequest request;

		// Form data given. This is only compatible with POST
		if ((formData != null) && (requestType == WebManager.RequestType.Post))
		{
			// Construct the post request
			request = UnityWebRequest.Post(WebAddress + subAddress, formData);
		}
		// No form data given
		else
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

			// Construct a basic request for the verb type
			request = new UnityWebRequest(WebAddress + subAddress, requestVerb);

			// Construct a download handler, using a basic buffer
			request.downloadHandler = new DownloadHandlerBuffer();
		}

		// Create the request, with common headers
		foreach (string key in m_headers.Keys)
			request.SetRequestHeader(key, m_headers[key]);
		request.timeout = TimeoutSeconds;

		// If JSON PUT data has been specified
		if (jsonPutData != null)
		{
			// Encode the object into a JSON string
			string jsonString = JsonConvert.SerializeObject(jsonPutData);
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

		// Handle the request response
		response(request);
	}
}
