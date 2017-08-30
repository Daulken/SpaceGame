using System;

namespace SharpPDF.Exceptions {

	/// <summary>
	/// Exception that represents a generic error during the I/O.
	/// </summary>
	public class PDFGenericIOException : Exception
	{
		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="message">Message for the exception</param>
		/// <param name="ex">Inner exception</param>
		public PDFGenericIOException(string message, Exception ex):base(message,ex)
		{
		}
	}
}
