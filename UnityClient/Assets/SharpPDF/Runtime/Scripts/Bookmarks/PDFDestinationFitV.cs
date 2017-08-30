using System;
using SharpPDF;

namespace SharpPDF.Bookmarks {
	
	/// <summary>
	/// Class that represents a pdfDestination of FitV type.
	/// </summary>
	internal class PDFDestinationFitV : IPDFDestination
	{
		int _left;

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="left">Left margin</param>
		public PDFDestinationFitV(int left)
		{
			_left = left;
		}

		/// <summary>
		/// Method that returns the PDF codes to write the destination
		/// </summary>
		/// <returns>String that contains PDF codes</returns>
		public string GetDestinationValue()
		{
			return "/FitV " + _left.ToString();
		}
	}
}
