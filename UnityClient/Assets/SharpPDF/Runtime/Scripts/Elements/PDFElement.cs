using System;
using SharpPDF.Exceptions;
using SharpPDF.Enumerators;

namespace SharpPDF.Elements {
	
	/// <summary>
	/// A Class that implements a generic PDF element.
	/// </summary>
	public abstract class PDFElement : PDFPositionableObject, IWritable, ICloneable
	{

		/// <summary>
		/// Element's color
		/// </summary>
		protected PDFColor _strokeColor = PDFColor.NoColor;
		/// <summary>
		/// Border's color
		/// </summary>
		protected PDFColor _fillColor = PDFColor.NoColor;
		/// <summary>
		/// Element's ID
		/// </summary>
		protected int _objectID;
		/// <summary>
		/// Height of the element
		/// </summary>
		protected int _height;
		/// <summary>
		/// Width of the element
		/// </summary>
		protected int _width;

		/// <summary>
		/// Border's color
		/// </summary>
		public PDFColor strokeColor
		{
			get
			{
				return _strokeColor;
			}

			set
			{
				_strokeColor = value;
			}
		}

		/// <summary>
		/// Element's Color
		/// </summary>
		public PDFColor fillColor
		{
			get
			{
				return _fillColor;
			}

			set
			{
				_fillColor = value;
			}
		}

		/// <summary>
		/// Height of the element
		/// </summary>
		public int height
		{
			get
			{
				return _height;
			}
		}

		/// <summary>
		/// Width of the element
		/// </summary>
		public int width
		{
			get
			{
				return _width;
			}
		}

		/// <summary>
		/// Element's ID
		/// </summary>
		internal int objectID
		{
			get
			{
				return _objectID;
			}
			set
			{
				_objectID = value;
			}
		}		 

		/// <summary>
		/// Method that returns the PDF codes to write the generic element in the document. It must be implemented by the derived class
		/// </summary>
		/// <returns>String that contains PDF codes</returns>
		public abstract string GetText();
		
		#region ICloneable

		/// <summary>
		/// Method that clones the element
		/// </summary>
		/// <returns>Cloned object</returns>
		public abstract object Clone();			

		#endregion
	}
}
