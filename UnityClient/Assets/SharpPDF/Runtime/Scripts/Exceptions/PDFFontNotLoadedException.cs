using System;

namespace SharpPDF.Exceptions {
	
	/// <summary>
	/// Exception that represents a nonexistent font
	/// </summary>
	public class PDFFontNotLoadedException : PDFException
	{
		/// <summary>
		/// Class's constructor
		/// </summary>
		public PDFFontNotLoadedException():
			base("The font reference is not found inside the document!",null)
		{
		}
	}
}
