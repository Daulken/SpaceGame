using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using SharpPDF.Bookmarks;
using SharpPDF.Exceptions;
using SharpPDF.Enumerators;

namespace SharpPDF {

	/// <summary>
	/// A Class that implements a PDF Outlines.
	/// </summary>
	internal class PDFOutlines : IWritable
	{
		private int _objectIDOutlines;
		private int _childIDFirst = 0;
		private int _childIDLast = 0;
		private int _childCount = 0;
		private List<PDFBookmarkNode> m_rootBookmarks = new List<PDFBookmarkNode>();

		/// <summary>
		/// Outlines's ID
		/// </summary>
		public int objectIDOutlines
		{
			get
			{
				return _objectIDOutlines;
			}

			set
			{
				_objectIDOutlines = value;
			}
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		public PDFOutlines()
		{
			
		}

		/// <summary>
		/// Method that initialize
		/// </summary>
		/// <param name="counterID">Initial Object ID</param>
		/// <returns>Updated Object ID</returns>
		public int initializeOutlines(int counterID)
		{
			if (m_rootBookmarks.Count > 0)
			{		
				InitializeBookmarks(counterID, m_rootBookmarks, _objectIDOutlines);	
				_childIDFirst = m_rootBookmarks[0].ObjectID;
				_childIDLast = m_rootBookmarks[m_rootBookmarks.Count - 1].ObjectID;
				counterID += _childCount;
			}
			else
			{
				_childCount = 0;
				_childIDFirst = 0;
				_childIDLast = 0;
			}
			return counterID;
		}

		/// <summary>
		/// Method that adds a bookmark to the outlines object
		/// </summary>
		/// <param name="Bookmark">BookmarkNode Object</param>
		public void AddBookmark(PDFBookmarkNode Bookmark)
		{
			m_rootBookmarks.Add(Bookmark);
		}

		/// <summary>
		/// Method that initialize all bookmarks
		/// </summary>
		/// <param name="CounterID">Initial Object ID</param>
		/// <param name="bookmarks">List<PDFBookmarkNode> of BookmarkNodes of the same level</param>
		/// <param name="FatherID">Object ID of the father</param>
		/// <returns>Number of childs</returns>
		private int InitializeBookmarks(int CounterID, List<PDFBookmarkNode> bookmarks, int FatherID)
		{
			int currentNodes = 0;
			if (bookmarks.Count > 0)
			{
				PDFBookmarkNode bookmark;
				for (int i = 0; i < bookmarks.Count; i++)
				{
					bookmark = bookmarks[i];				
					bookmark.ObjectID = CounterID + _childCount;				
					bookmark.parent = FatherID;									
					currentNodes++;
					_childCount++;
					bookmark.childCount = InitializeBookmarks(CounterID, bookmark.children, bookmark.ObjectID);					
					if (bookmark.childCount > 0)
					{
						bookmark.first = bookmark.getFirstChild().ObjectID;
						bookmark.last = bookmark.getLastChild().ObjectID;	
					}
					if (bookmarks.Count > 1)
					{
						if (i > 0)
							bookmark.prev = bookmarks[i - 1].ObjectID;
						if (i < (bookmarks.Count - 1))
							bookmark.next = CounterID + _childCount;
					}
					currentNodes += bookmark.childCount;
				}
			}
			return currentNodes;
		}

		/// <summary>
		/// Method that returns the PDF codes to write the Outlines in the document
		/// </summary>
		/// <returns>String that contains PDF codes</returns>
		public string GetText()
		{
			StringBuilder strOutlines = new StringBuilder();	
			strOutlines.Append(_objectIDOutlines.ToString() + " 0 obj" + Convert.ToChar(13) + Convert.ToChar(10));
			strOutlines.Append("<<" + Convert.ToChar(13) + Convert.ToChar(10));
			strOutlines.Append("/Type /Outlines" + Convert.ToChar(13) + Convert.ToChar(10));
			if (_childCount != 0)
			{
				strOutlines.Append("/First " + _childIDFirst.ToString() + " 0 R" + Convert.ToChar(13) + Convert.ToChar(10));
				strOutlines.Append("/Last " + _childIDLast.ToString() + " 0 R" + Convert.ToChar(13) + Convert.ToChar(10));
				strOutlines.Append("/Count " + _childCount.ToString() + Convert.ToChar(13) + Convert.ToChar(10));
			}
			else
			{
				strOutlines.Append("/Count 0" + Convert.ToChar(13) + Convert.ToChar(10));
			}
			strOutlines.Append(">>" + Convert.ToChar(13) + Convert.ToChar(10));
			strOutlines.Append("endobj" + Convert.ToChar(13) + Convert.ToChar(10));			
			return strOutlines.ToString();
		}

		/// <summary>
		/// Method that returns all nodes from a start collection
		/// </summary>
		/// <param name="bookmarks">List<PDFBookmarkNode> with the start point of PDFBookmarkNodes</param>
		/// <returns>Collection of all PDFBookmarkNodes from the start point</returns>
		private List<PDFBookmarkNode> GetBookmarksIncludingChildren(List<PDFBookmarkNode> bookmarks)
		{
			List<PDFBookmarkNode> resultList = new List<PDFBookmarkNode>();
			if (bookmarks.Count > 0)
			{
				resultList.AddRange(bookmarks);
				foreach (PDFBookmarkNode bookmark in bookmarks)
				{
					resultList.AddRange(GetBookmarksIncludingChildren(bookmark.children));
				}
			}						
			return resultList;
		}

		/// <summary>
		/// Method that returns a sorted(by objectID) collection of PDFBookmarkNodes
		/// </summary>
		/// <returns>Sorted bookmark collection</returns>
		public List<PDFBookmarkNode> GetBookmarks()
		{
			List<PDFBookmarkNode> returnedList = GetBookmarksIncludingChildren(m_rootBookmarks);
			returnedList.Sort();
			return returnedList;
		}
		
	}
}
