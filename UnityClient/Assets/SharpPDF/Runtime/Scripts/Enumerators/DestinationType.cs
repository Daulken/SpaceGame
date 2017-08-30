namespace SharpPDF.Enumerators {
	
	/// <summary>
	/// Enumerator that implements the type of pdf's destination.
	/// </summary>
	internal enum DestinationType
	{
		/// <summary>
		/// Null pdf's destination
		/// </summary>
		None,
		/// <summary>
		/// Pdf's destination with top, left and zoom value
		/// </summary>
		XYZ,
		/// <summary>
		/// Pdf's destination that shows the entire page
		/// within the window both horizontally and vertically.
		/// </summary>
		Fit,
		/// <summary>
		/// Pdf's destination that display the page with the vertical coordinate top positioned
		/// at the top edge of the window
		/// </summary>
		FitH,
		/// <summary>
		/// Pdf's destination that display the page with the horizontal coordinate left positioned
		/// at the left edge of the window
		/// </summary>
		FitV,
		/// <summary>
		/// Pdf's destination that display the page with its contents magnified just enough
		/// to fit the rectangle specified by the coordinates left, bottom, right, and top
		/// entirely within the window both horizontally and vertically.
		/// </summary>
		FitR
	}
}