using System;
using SharpPDF;
using SharpPDF.Enumerators;
using SharpPDF.Exceptions;
using SharpPDF.Fonts.AFM;
using SharpPDF.Fonts.TTF;

namespace SharpPDF.Fonts {
	
	/// <summary>
	/// Class that represents a Font Factory
	/// </summary>
	internal abstract class PDFFontFactory
	{
		private readonly static string[] ms_predefinedFontNames = {
				"None",
				"Helvetica",
				"Helvetica-Bold",
				"Helvetica-Oblique",
				"Helvetica-BoldOblique",
				"Courier",
				"Courier-Bold",
				"Courier-Oblique",
				"Courier-BoldOblique",
				"Times-Roman",
				"Times-Bold",
				"Times-Italic",
				"Times-BoldItalic"
			};
		
		/// <summary>
		/// Method that returns an abstract font object[PDFAbstractFont Factory]
		/// </summary>
		/// <param name="fontReference">Font Name(for Predefined fonts) or Font TextAsset resource name for TrueType fonts</param>
		/// <param name="fontNumber">Number of the font inside the PDF</param>
		/// <param name="fontType">Type of the font</param>
		/// <returns>Abstract Font Object</returns>
		public static PDFAbstractFont GetFontObject(string fontReference, int fontNumber, DocumentFontType fontType)
		{
			PDFAbstractFont fontObject = null;			
			switch (fontType)
			{
			case DocumentFontType.Predefinedfont:
			default:
				fontObject = new PDFPredefinedFont((new AFMFontReader(fontReference)).GetFontDefinition(), fontNumber, DocumentFontEncoding.WinAnsiEncoding);
				break;
			case DocumentFontType.TrueTypeFont:
				fontObject = new PDFTrueTypeFont((new TTFFontReader(fontReference)).GetFontDefinition(), fontNumber, DocumentFontEncoding.IdentityH, fontReference);
				break;
			}
			return fontObject;
		}		

		/// <summary>
		/// Method that returns that font name for a Predefined font
		/// </summary>
		/// <param name="fontStyle">Font style</param>
		/// <returns>Predefined font name</returns>
		public static string GetPredefinedFontName(PredefinedFont fontStyle)
		{
			return PDFFontFactory.ms_predefinedFontNames[Convert.ToInt32(fontStyle)];
		}

		/// <summary>
		/// Method that returns if the font reference is a Predefined font
		/// </summary>
		/// <param name="fontReference">Font Reference</param>
		/// <returns>Boolean value that shows if the font reference is a Predefined font</returns>
		public static bool IsPredefinedFont(string fontReference)
		{
			foreach (string predefinedReference in ms_predefinedFontNames)
			{
				if (fontReference == predefinedReference)
					return true;
			}
			return false;
		}
	}
}
