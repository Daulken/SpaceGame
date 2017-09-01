using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketRowButtonClickHandler : MonoBehaviour, UnityEngine.EventSystems.IPointerClickHandler, UnityEngine.EventSystems.IDeselectHandler, UnityEngine.EventSystems.ISelectHandler
{
	public int			m_rowIndex = 0;

	// TODO - looking up game objects at runtime is slow - ideally we should use the heirarchy to find a parent or something.

	public void OnPointerClick( UnityEngine.EventSystems.PointerEventData eventData )
	{
		GameObject marketRowLayoutObject = GameObject.Find( "MarketUILayout" );
		if( marketRowLayoutObject != null )
		{
			MarketUILayout marketUILayoutComponent = marketRowLayoutObject.GetComponent<MarketUILayout>();
			marketUILayoutComponent.RowClicked( m_rowIndex );
		}
	}

	public void OnDeselect( UnityEngine.EventSystems.BaseEventData eventData )
	{
		GameObject marketRowLayoutObject = GameObject.Find( "MarketUILayout" );
		if( marketRowLayoutObject != null )
		{
			MarketUILayout marketUILayoutComponent = marketRowLayoutObject.GetComponent<MarketUILayout>();
			marketUILayoutComponent.RowDeselected();
		}
	}

	public void OnSelect( UnityEngine.EventSystems.BaseEventData eventData )
	{
		GameObject marketRowLayoutObject = GameObject.Find( "MarketUILayout" );
		if( marketRowLayoutObject != null )
		{
			MarketUILayout marketUILayoutComponent = marketRowLayoutObject.GetComponent<MarketUILayout>();
			marketUILayoutComponent.RowSelected( m_rowIndex );
		}
	}
}
