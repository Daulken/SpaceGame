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
		SharedUILayout.OnPlayerDataReturned += OnPlayerDataReturned;
		m_sharedLayout.FetchPlayerData();
		RefreshRows();
		RefreshControls();
		m_sharedLayout.RefreshCommonElements( 0, 1 );
	}

	void OnDestroy()
	{
		SharedUILayout.OnPlayerDataReturned -= OnPlayerDataReturned;
		SharedUILayout.OnPageChanged -= OnPageChanged;
	}

	// Update is called once per frame
	void Update ()
	{
		
	}

	// Populate the view with the appropiate data
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
			GameObject newRow = Instantiate( m_templateRow ) as GameObject;
			m_rows.Add( newRow );

			// Set the index on the click handler, so when we get a clicked callback, we know which row it came from
			MarketRowButtonClickHandler newRowClickHandler = newRow.GetComponent<MarketRowButtonClickHandler>();
			if( newRowClickHandler != null )
			{
				newRowClickHandler.m_rowIndex = rowIndex;
			}

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

	public void RowSelected( int rowIndex )
	{
		m_isAnyRowSelected = true;
		m_selectedRow = rowIndex;

		RefreshControls();
	}

	public void RowDeselected()
	{
		m_isAnyRowSelected = false;

		RefreshControls();
	}

	public void BuyClicked()
	{
		RefreshRows();
	}

	public void OnPageChanged()
	{
	}

	public void OnPlayerDataReturned()
	{
		//FetchMarketData();
	}

	private void FetchMarketData()
	{
		DatabaseAccess.GetMarketOrders( m_sharedLayout.m_storedPlayer.PlayerId, ( success, error, marketOrders ) =>
				{
					if( success )
					{
						//marketOrders.Count
					}
					else
					{
						// Display error dialog
						InfoDialog.Instance.Show( "LOGIN_ERROR_TITLE", error, null );
					}
				}
			);
	}
}
