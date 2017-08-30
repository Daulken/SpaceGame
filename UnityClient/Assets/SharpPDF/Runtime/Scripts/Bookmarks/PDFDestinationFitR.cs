using System;
using SharpPDF;

namespace SharpPDF.Bookmarks {
	
	/// <summary>
	/// Class that represents a pdfDestination of FitR type.
	/// </summary>
	internal class PDFDestinationFitR : IPDFDestination
	{
		int _left;
		int _top;
		int _bottom;
		int _right;		

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="left">Left margin</param>
		/// <param name="top">Top margin</param>
		/// <param name="bottom">Bottom margin</param>
		/// <param name="right">Right margin</param>
		public PDFDestinationFitR(int left, int top, int bottom, int right)
		{
			_left = left;
			_top = top;
			_bottom = bottom;
			_right = right;
		}

		/// <summary>
		/// Method that returns the PDF codes to write the destination
		/// </summary>
		/// <returns>String that contains PDF codes</returns>
		public string GetDestinationValue()
		{
			return "/FitR " + _left.ToString() + " " + _bottom.ToString() + " " + _right.ToString() + " " + _top.ToString();
		}
	}
}
