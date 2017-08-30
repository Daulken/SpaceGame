using System;
using SharpPDF;
using SharpPDF.Enumerators;
using SharpPDF.Exceptions;

namespace SharpPDF.Fonts {
	
	/// <summary>
	/// Abstract Class for a generic Font Readers
	/// </summary>
	internal abstract class FontReader : IDisposable
	{
		/// <summary>
		/// Method that returs the definition of a font
		/// </summary>
		/// <returns>Font definition object</returns>
		public abstract PDFFontDefinition GetFontDefinition();

		/// <summary>
		/// Method that releases all the resources of the FontReader
		/// </summary>
		public abstract void Dispose();		

	}
}
