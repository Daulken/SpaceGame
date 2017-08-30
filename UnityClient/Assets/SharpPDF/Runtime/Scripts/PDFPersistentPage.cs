using System;
using System.Collections;
using SharpPDF.Elements;
using SharpPDF.Enumerators;
using SharpPDF.Exceptions;
using SharpPDF.Fonts;

namespace SharpPDF
{
	/// <summary>
	/// Class that represents a persistent page.
	/// All its objects are inherited by all document's pages.
	/// </summary>
	public class PDFPersistentPage : PDFBasePage
	{

		/// <summary>
		/// Class's constructor
		/// </summary>
		public PDFPersistentPage(PDFDocument containerDoc):base(containerDoc)
		{

		}

		/// <summary>
		/// Class's distructor
		/// </summary>
		~PDFPersistentPage()
		{			
			_containerDoc = null;
			_elements = null;
		}

	}
}
