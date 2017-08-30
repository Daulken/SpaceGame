using System;

namespace SharpPDF.Exceptions {

	/// <summary>
	/// Exception that represents an error during the I/O on the buffer.
	/// </summary>
	public class PDFMissingFontFileException : PDFException
	{
		/// <summary>
		/// Class's constructor
		/// </summary>
		public PDFMissingFontFileException(string fontName): base("Failed to load font resource '" + fontName + "'", null)
		{
		}
	}
}
