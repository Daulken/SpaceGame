using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu( "SPACEJAM/UI/SharedUILayout" )]
public class SharedUILayout : MonoBehaviour
{
	public GameObject		m_currency;
	public GameObject		m_page;
	public GameObject		m_pageUpButton;
	public GameObject		m_pageDownButton;

	protected int			m_numPages = 1;
	protected int			m_currentPage = 0;

	private int				m_currentCurrencyAmountOwned = 0;

	public delegate void	PageChangedAction();
	public static event		PageChangedAction OnPageChanged;

	// Use this for initialization
	void Start ()
	{
		RefreshCommonElements();
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	void RefreshCommonElements()
	{
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

	public void OnPageUp()
	{
		if( m_currentPage > 0 )
		{
			--m_currentPage;
		}
		OnPageChanged();
	}

	public void OnPageDown()
	{
		int maxPage = m_numPages - 1;
		if( m_currentPage < maxPage )
		{
			++m_currentPage;
			OnPageChanged();
		}
	}
}
