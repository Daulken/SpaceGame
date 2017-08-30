using System;

namespace SharpPDF.Exceptions {
	
	/// <summary>
	/// Exception that represents a row longer than maximum table space
	/// </summary>
	public class PDFBadRowHeightException : PDFException
	{
		/// <summary>
		/// Class's Costructor
		/// </summary>
		public PDFBadRowHeightException():
			base ("The height of the row exceed the maximum height", null)
		{
		}
	}
}
