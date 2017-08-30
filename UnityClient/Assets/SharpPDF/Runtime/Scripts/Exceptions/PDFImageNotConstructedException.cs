using System;

namespace SharpPDF.Exceptions {
	
	/// <summary>
	/// Exception that represents an image reference that hasn't finished compressing
	/// </summary>
	public class PDFImageNotConstructedException : PDFException
	{
		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="message">Message for the exception</param>
		public PDFImageNotConstructedException(string message): base(message, null)
		{
		}
	}
}
