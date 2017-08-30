using System;

namespace SharpPDF.Exceptions {
	
	/// <summary>
	/// Exception that represents an error during the writing of the PDF document.
	/// </summary>
	public class PDFWritingErrorException : PDFException
	{
		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="message">Message for the exception</param>
		/// <param name="ex">Inner exception</param>
		public PDFWritingErrorException(string message, Exception ex): base(message,ex)
		{
		}
	}
}
