using System;

namespace SharpPDF.Exceptions {
	
	/// <summary>
	/// Exception that represents a nonexistent font
	/// </summary>
	public class PDFFontIsPredefinedException : PDFException
	{
		/// <summary>
		/// Class's constructor
		/// </summary>
		public PDFFontIsPredefinedException():
			base("The given TrueType font reference is already bound to a predefined font!", null)
		{
		}
	}
}
