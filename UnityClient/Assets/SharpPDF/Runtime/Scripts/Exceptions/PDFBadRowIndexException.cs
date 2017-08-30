using System;

namespace SharpPDF.Exceptions {
	
	/// <summary>
	/// Exception that represents an error during an access on the PDFTable's rows with a bad index
	/// </summary>
	public class PDFBadRowIndexException : PDFException
	{
		/// <summary>
		/// Class's constructor
		/// </summary>
		public PDFBadRowIndexException():
			base ("The row index does not exist", null)
		{
		}
	}
}
