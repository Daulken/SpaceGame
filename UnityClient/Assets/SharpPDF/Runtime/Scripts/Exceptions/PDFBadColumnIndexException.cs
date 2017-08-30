using System;

namespace SharpPDF.Exceptions {
	
	/// <summary>
	/// Exception that represents an error during an access on the PDFTableRow's columns with a bad index
	/// </summary>
	public class PDFBadColumnIndexException : PDFException
	{
		/// <summary>
		/// Class's constructor
		/// </summary>
		public PDFBadColumnIndexException():
			base ("The column index does not exist", null)
		{
		}
	}
}
