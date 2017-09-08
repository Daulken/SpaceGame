using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("SPACEJAM/GameState/Market/SharedUILayout")]
public class SharedUILayout : MonoBehaviour
{
	public GameObject		m_currency;
	public GameObject		m_page;
	public GameObject		m_pageUpButton;
	public GameObject		m_pageDownButton;

	private int				m_numPages = 1;
	public int				m_currentPage = 0;

	private System.Decimal	m_currentCurrencyAmountOwned = 0;

	public delegate void	PageChangedAction();
	public static event		PageChangedAction OnPageChanged;

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	// Update the common elements of market, inventory view etc, such as page up/down arrows
	public void RefreshCommonElements(int currentPage, int numPages )
	{
		if (DatabaseAccess.LoggedInPlayer != null)
		{
			m_currentCurrencyAmountOwned = ( System.Decimal )DatabaseAccess.LoggedInPlayer.CreditBalance;
		}
		m_currentPage = currentPage;
		m_numPages = numPages;

		if( m_currency != null )
		{
			UnityEngine.UI.Text currencyText = m_currency.GetComponent<UnityEngine.UI.Text>();
			currencyText.text = m_currentCurrencyAmountOwned.ToString() + " credits";
		}

		if( m_page != null )
		{
			int shownPageNumber = m_currentPage + 1;
			UnityEngine.UI.Text pageText = m_page.GetComponent<UnityEngine.UI.Text>();
			pageText.text = "Page " + shownPageNumber.ToString() + "/" + m_numPages.ToString();
		}
	}

	// Called when page up button is clicked
	public void OnPageUp()
	{
		if( m_currentPage > 0 )
		{
			--m_currentPage;
			OnPageChanged();
		}
	}

	// Called when page down button is clicked
	public void OnPageDown()
	{
		int maxPage = m_numPages - 1;
		if( m_currentPage < maxPage )
		{
			++m_currentPage;
			OnPageChanged();
		}
	}

	// Explicitly set the page
	public void SetPage( int pageNum )
	{
		m_currentPage = pageNum;
	}
}
