using System;

namespace SharpPDF.Exceptions {
	
	/// <summary>
	/// Exception that represents an error during the I/O on the buffer.
	/// </summary>
	public class PDFBadFontFileException : PDFException
	{
		/// <summary>
		/// Class's constructor
		/// </summary>
		public PDFBadFontFileException():
			base("The font file is badly formatted", null)
		{
		}
	}
}
