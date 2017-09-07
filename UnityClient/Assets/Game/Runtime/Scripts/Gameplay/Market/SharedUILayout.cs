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
	private int				m_currentPage = 0;

	private System.UInt64	m_currentCurrencyAmountOwned = 0;

	public delegate void	PageChangedAction();
	public static event		PageChangedAction OnPageChanged;

	public delegate void PlayerDataReturnedAction();
	public static event PlayerDataReturnedAction OnPlayerDataReturned;

	public SpaceLibrary.Player m_storedPlayer;

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public void RefreshCommonElements(int currentPage, int numPages )
	{
		if( m_storedPlayer != null )
		{
			m_currentCurrencyAmountOwned = ( System.UInt64 )m_storedPlayer.CreditBalance;
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

	public void OnPageUp()
	{
		if( m_currentPage > 0 )
		{
			--m_currentPage;
			OnPageChanged();
		}
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

	public void FetchPlayerData()
	{
		// This will call FetchPlayerResult when the response has come back
		DatabaseAccess.GetPlayer( ReturnedPlayerData );
	}

	private void ReturnedPlayerData( bool success, string error, SpaceLibrary.Player player )
	{
		if( success )
		{
			m_storedPlayer = player;
			OnPlayerDataReturned();
		}
		else
		{
			// Display error dialog
			InfoDialog.Instance.Show( "LOGIN_ERROR_TITLE", error, null );
		}
	}
}
