using System;
using SharpPDF;
using SharpPDF.Enumerators;
using SharpPDF.Exceptions;

namespace SharpPDF.Bookmarks {
	
	/// <summary>
	/// Interface for a pdfDestination
	/// </summary>
	public interface IPDFDestination
	{
		/// <summary>
		/// Method that returns the PDF codes to write the destination
		/// </summary>
		/// <returns>String that contains PDF codes</returns>
		string GetDestinationValue();
	}
}
