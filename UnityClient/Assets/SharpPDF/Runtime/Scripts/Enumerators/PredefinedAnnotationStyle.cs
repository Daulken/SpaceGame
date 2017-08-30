namespace SharpPDF.Enumerators {
	
	/// <summary>
	/// Enumerator that implements annotation styles.
	/// inserted by smeyer82 (2004)
	/// </summary>	
	public enum PredefinedAnnotationStyle : short
	{
		/// <summary>
		/// None
		/// </summary>
		None = 0,
		/// <summary>
		/// Annotation comment
		/// </summary>
		Comment = 1,
		/// <summary>
		/// Annotation key
		/// </summary>
		Key = 2,
		/// <summary>
		/// Annotation note
		/// </summary>
		Note = 3,
		/// <summary>
		/// Annotation help
		/// </summary>
		Help = 4,
		/// <summary>
		/// Annotation new paragraph
		/// </summary>
		NewParagraph = 5,
		/// <summary>
		/// Annotation paragraph
		/// </summary>
		Paragraph = 6,
		/// <summary>
		/// Annotation insert
		/// </summary>
		Insert = 7
	}
}