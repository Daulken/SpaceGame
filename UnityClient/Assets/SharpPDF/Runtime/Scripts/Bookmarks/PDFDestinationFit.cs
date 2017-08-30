using System;
using SharpPDF;

namespace SharpPDF.Bookmarks {
	
	/// <summary>
	/// Class that represents a PDFDestination of Fit type.
	/// </summary>
	internal class PDFDestinationFit : IPDFDestination
	{
	
		/// <summary>
		/// Class's constructor
		/// </summary>
		public PDFDestinationFit()
		{

		}

		/// <summary>
		/// Method that returns the PDF codes to write the destination
		/// </summary>
		/// <returns>String that contains PDF codes</returns>
		public string GetDestinationValue()
		{
			return "/Fit";
		}

	}
}
