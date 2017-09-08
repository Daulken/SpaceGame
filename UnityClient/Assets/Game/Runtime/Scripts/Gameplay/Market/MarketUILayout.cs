using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("SPACEJAM/GameState/Market/MarketUILayout")]
public class MarketUILayout : MonoBehaviour
{
	public SharedUILayout		m_sharedLayout;
	public GameObject			m_rowContainer;
	public GameObject			m_templateRow;
	public GameObject			m_buyButton;
	public float				m_rowSpacing = 0.0f;
	private float				m_rowHeight = 0.0f;
	private int					m_maxVisibleRows = 0;
	private float				m_totalRowHeight = 0;
	private List<GameObject>	m_rows = new List<GameObject>();
	private int					m_selectedRow = 0;
	private bool				m_isAnyRowSelected = false;

	private List<SpaceLibrary.MarketOrder> m_orders;
	private int					m_numMarketEntries = 0;


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

		SharedUILayout.OnPageChanged += OnPageChanged;
		FetchMarketData();
		RefreshRows();
		RefreshControls();
		m_sharedLayout.RefreshCommonElements( 0, 1 );

		DatabaseAccess.OnPlayerUpdated += OnPlayerUpdated;
	}

	void OnDestroy()
	{
		DatabaseAccess.OnPlayerUpdated -= OnPlayerUpdated;
		SharedUILayout.OnPageChanged -= OnPageChanged;
	}

	// Update is called once per frame
	void Update ()
	{
		
	}

	// Populate the market view with the appropiate data
	int RefreshRows()
	{
		// Remove existing rows
		foreach( GameObject row in m_rows )
		{
			Destroy( row );
		}
		m_rows.Clear();

		// Get the row container transform
		RectTransform containerTransform = m_rowContainer.GetComponent<RectTransform>();

		// Duplicate rows for each item to buy
		int rowCount = 0;
		for( int rowIndex = 0; rowIndex < m_maxVisibleRows; ++rowIndex )
		{ 
			int entryIndex = rowIndex + m_sharedLayout.m_currentPage * m_maxVisibleRows;
			if( entryIndex < m_numMarketEntries )
			{
				// Create a new row
				GameObject newRow = Instantiate( m_templateRow ) as GameObject;
				m_rows.Add( newRow );

				// Set the index on the click handler, so when we get a clicked callback, we know which row it came from
				MarketRowButtonClickHandler newRowClickHandler = newRow.GetComponent<MarketRowButtonClickHandler>();
				if( newRowClickHandler != null )
				{
					newRowClickHandler.m_rowIndex = rowIndex;
				}

				// Fill in the row info
				MarketRow row = newRow.GetComponent<MarketRow>();
				if( row != null )
				{
					row.m_materialID = m_orders[ entryIndex ].MaterialId;
					row.m_price = (System.Decimal)m_orders[ entryIndex ].Price;
					row.m_quantity = (System.UInt32)m_orders[ entryIndex ].Quantity;
					row.PopulateUI();
				}

				RectTransform newRowTransform = newRow.GetComponent<RectTransform>();

				// Set the parent to be the container object
				newRowTransform.SetParent( containerTransform, false );
				float rowOffset = m_totalRowHeight * ( float )rowIndex;
				Vector2 newPos = new Vector2( newRowTransform.anchoredPosition.x, newRowTransform.anchoredPosition.y - rowOffset );
				newRowTransform.anchoredPosition = newPos;

				++rowCount;
			}
		}

		return rowCount;
	}

	// Update the state of the other controls like the buy button
	void RefreshControls()
	{
		// If a row is selected, enable the buy button
		if( m_buyButton != null )
		{
			UnityEngine.UI.Button buttonComponent = m_buyButton.GetComponent<UnityEngine.UI.Button>();
			buttonComponent.interactable = m_isAnyRowSelected;
		}
	}

	public void RowClicked( int rowIndex )
	{
		// RowSelected seems to do what we want here - not sure we need the click detection?
	}

	// Called when a row is selected
	public void RowSelected( int rowIndex )
	{
		m_isAnyRowSelected = true;
		m_selectedRow = rowIndex;

		RefreshControls();
	}

	// Call when a row is deselected
	public void RowDeselected()
	{
		m_isAnyRowSelected = false;

		RefreshControls();
	}

	// Called when 'buy' is clicked
	public void BuyClicked()
	{
		RefreshRows();
	}

	// Called when 'refresh' is clicked
	public void RefreshClicked()
	{
		FetchMarketData();
	}

	// Called when the page is changed by a button click
	public void OnPageChanged()
	{
	}

	// Called whenever the player is updated
	private void OnPlayerUpdated(SpaceLibrary.Player player)
	{
		m_sharedLayout.RefreshCommonElements(0, 1);
		FetchMarketData();
	}

	// Obtain the market data
	private void FetchMarketData()
	{
		// This will call FetchPlayerResult when the response has come back
		DatabaseAccess.GetMarketOrders( 1, MarketDataReturned );
	}

	// Callback for when the market data has been obtained
	private void MarketDataReturned( bool success, string error, List<SpaceLibrary.MarketOrder> marketOrders )
	{
		if( success )
		{
			// Reset the market view
			m_numMarketEntries = marketOrders.Count;
			m_orders = marketOrders;
		}
		else
		{
			// Empty the market
			m_numMarketEntries = 0;
			m_orders.Clear();

			// Display error dialog
			InfoDialog.Instance.Show( "LOGIN_ERROR_TITLE", error, null );
		}
		m_sharedLayout.SetPage( 0 );
		RefreshRows();
	}
}
