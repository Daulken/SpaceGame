using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceLibrary
{
	/// <summary>
	/// Wrapper for a JSON response in a web service communication
	/// </summary>
	public class ServiceWrapper
    {
		/// <summary>
		/// Whether the web service call was successful or not
		/// </summary>
		public bool ResponseSuccessful
		{
			get; set;
		}

		/// <summary>
		/// If the call failed, this is the error code returned
		/// </summary>
		public int ResponseCode
		{
			get; set;
		}

		/// <summary>
		/// If the call failed, this is a description of the error (not localised)
		/// </summary>
		public string ResponseMessage
		{
			get; set;
		}

		/// <summary>
		/// If the call succeeded, this is the serialised type returned in the JSON response
		/// </summary>
		public string ReturnedDataType
		{
			get; set;
		}

		/// <summary>
		/// If the call succeeded, this is the version of the serialised type returned in the JSON response
		/// </summary>
		public string ReturnedDataTypeVersion
		{
			get; set;
		}

		/// <summary>
		/// If the call succeeded, this is a serialised JSON response containing relevant returned data
		/// </summary>
		public string ReturnedJsonData
		{
			get; set;
		}
    }
}
