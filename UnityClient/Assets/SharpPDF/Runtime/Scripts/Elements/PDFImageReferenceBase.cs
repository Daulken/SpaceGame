using System;
using System.IO;
using System.Text;
using UnityEngine;
using SharpPDF.Exceptions;

namespace SharpPDF.Elements {

	/// <summary>
	/// Class that represents an image reference inside the document.
	/// </summary>
	public abstract class PDFImageReferenceBase : IWritable
	{
		protected int m_objectID;
		protected int m_height;
		protected int m_width;
		protected byte[] m_imageByteData;

		/// <summary>
		/// Object's ID
		/// </summary>
		internal int objectID
		{
			get
			{
				return m_objectID;
			}
			set
			{
				m_objectID = value;
			}
		}

		/// <summary>
		/// Image's Height
		/// </summary>
		public int height
		{
			get
			{
				return m_height;
			}
		}

		/// <summary>
		/// Image's Width
		/// </summary>
		public int width
		{
			get
			{
				return m_width;
			}
		}

		/// <summary>
		/// Image's bytes
		/// </summary>
		internal byte[] content
		{
			get
			{
				return m_imageByteData;
			}
		}

		/// <summary>
		/// Queries whether construction of reference has finished
		/// </summary>
		/// <returns>Whether construction has finished</returns>
		public abstract bool Constructed();
		
		/// <summary>
		/// Method that returns the PDF codes to write the generic element in the document. It must be implemented by the derived class
		/// </summary>
		/// <returns>String that contains PDF codes</returns>
		public abstract string GetText();
		
	}
}
