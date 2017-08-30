using System;

namespace SharpPDF.Exceptions {
	
	/// <summary>
	/// Generic PDF Exception.
	/// </summary>
	public class PDFException : Exception
	{
		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="message">Message for the exception</param>
		/// <param name="ex">Inner exception</param>
		public PDFException(string message, Exception ex):
			base(message, ex)
		{
		}
	}
}
