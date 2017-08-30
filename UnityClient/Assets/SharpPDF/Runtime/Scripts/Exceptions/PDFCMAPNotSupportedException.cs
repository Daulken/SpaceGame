using System;

namespace SharpPDF.Exceptions {
	
	/// <summary>
	/// Exception that represents an incompatible Character Mapping Table[The Font MUST be Windows/Unicode Or MAC].
	/// </summary>
	public class PDFCMAPNotSupportedException : PDFException
	{
		/// <summary>
		/// Class's constructor
		/// </summary>
		public PDFCMAPNotSupportedException():
			base("The CMAP of the font file is not supported", null)
		{
		}
	}
}
