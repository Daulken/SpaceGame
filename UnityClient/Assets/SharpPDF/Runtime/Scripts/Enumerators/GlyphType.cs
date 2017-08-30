using System;

namespace SharpPDF.Enumerators {
	
	/// <summary>
	/// Enumerator that implements the glyph type
	/// </summary>
	public enum GlyphType : short
	{		
		/// <summary>
		/// Composite Glyph with two arguments
		/// </summary>
		TwoArgs = 1,		
		/// <summary>
		/// This indicates that there is a simple scale for the glyph
		/// </summary>
		SingleScale = 8,
		/// <summary>
		/// Indicates at least one more glyph after this one
		/// </summary>
		MoreComponents = 32,
		/// <summary>
		/// This indicates that there is a double scale for the glyph
		/// </summary>
		DoubleScale = 64,
		/// <summary>
		/// There is a  2 by 2 transormation that will be used to scale the component
		/// </summary>
		TwoByTwo = 128
	}
}
