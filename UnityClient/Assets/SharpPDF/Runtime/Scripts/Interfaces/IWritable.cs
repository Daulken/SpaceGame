using System;

using SharpPDF.Exceptions;
using SharpPDF.Enumerators;

namespace SharpPDF
{
	/// <summary>
	/// Generic interface for pdf's objects
	/// </summary>	
	public interface IWritable
	{
		/// <summary>
		/// Method that returns the PDF codes to write the object in the document
		/// </summary>
		/// <returns>String that contains PDF codes</returns>
		string GetText();
	}
}
