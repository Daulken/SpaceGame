using System;
using SharpPDF;

namespace SharpPDF.Bookmarks {
	
	/// <summary>
	/// Class that represents a PDFDestination of FitH type.
	/// </summary>
	internal class PDFDestinationFitH : IPDFDestination
	{
		int _top;

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="top">Top margin</param>
		public PDFDestinationFitH(int top)
		{
			_top = top;
		}

		/// <summary>
		/// Method that returns the PDF codes to write the destination
		/// </summary>
		/// <returns>String that contains PDF codes</returns>
		public string GetDestinationValue()
		{
			return "/FitH " + _top.ToString();
		}

	}
}
