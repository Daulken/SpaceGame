using System;
using System.Collections;
using System.Collections.Generic;
using SharpPDF.Elements;

namespace SharpPDF {

	/// <summary>
	/// Interface for all separable objects.
	/// </summary>
	public interface ISeparable
	{
		/// <summary>
		/// This Method returns the standard elements which the composite element is made of.
		/// </summary>
		/// <returns>Collection of basic elements</returns>
		List<PDFElement> GetBasicElements();
	}
}
