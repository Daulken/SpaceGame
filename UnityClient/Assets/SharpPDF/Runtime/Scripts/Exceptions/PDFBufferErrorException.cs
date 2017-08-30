using System;

namespace SharpPDF.Exceptions
{
	/// <summary>
	/// Exception that represents an error during the I/O on the buffer.
	/// </summary>
	public class PDFBufferErrorException : PDFException
	{
		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="message">Message for the exception</param>
		/// <param name="ex">Inner exception</param>
		public PDFBufferErrorException(string message, Exception ex):base(message, ex)
		{
			
		}
	}
}
