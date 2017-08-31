using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketUILayout : MonoBehaviour
{
	public GameObject			m_rowContainer;
	public GameObject			m_templateRow;
	public float				m_rowSpacing = 0.0f;
	private float				m_rowHeight = 0.0f;
	private int					m_maxVisibleRows = 0;
	private float				m_totalRowHeight = 0;


	// Use this for initialization
	void Start ()
	{
		// Get the template row Transform
		RectTransform templateRowTransform = m_templateRow.GetComponent<RectTransform>();
		m_rowHeight = templateRowTransform.rect.height;

		RectTransform containerTransform = m_rowContainer.GetComponent<RectTransform>();
		float containerHeight = containerTransform.rect.height;

		m_totalRowHeight = m_rowHeight + m_rowSpacing;

		m_maxVisibleRows = (int)( containerHeight / m_totalRowHeight );

		RefreshView();
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	// Populate the view with the appropiate data
	int RefreshView()
	{
		// Get the row container transform
		RectTransform containerTransform = m_rowContainer.GetComponent<RectTransform>();

		// Duplicate rows for each item to buy
		int rowCount = 0;
		for( int rowIndex = 0; rowIndex < m_maxVisibleRows; ++rowIndex )
		{
			GameObject newRow = Instantiate( m_templateRow ) as GameObject;
			RectTransform newRowTransform = newRow.GetComponent<RectTransform>();
			
			// Set the parent to be the container object
			newRowTransform.SetParent( containerTransform, false );
			float rowOffset = m_totalRowHeight * ( float )rowIndex;
			Vector2 newPos = new Vector2( newRowTransform.anchoredPosition.x, newRowTransform.anchoredPosition.y - rowOffset );
			newRowTransform.anchoredPosition = newPos;

			++rowCount;
		}

		return rowCount;
	}
}
