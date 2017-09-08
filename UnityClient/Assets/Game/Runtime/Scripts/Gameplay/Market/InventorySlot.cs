using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
	public GameObject m_itemNameText;
	public GameObject m_quantityText;
	public System.UInt32 m_quantity = 0;
	public int m_materialID = 0;

	// Use this for initialization
	void Start ()
	{		
	}
	
	// Update is called once per frame
	void Update ()
	{
	}

	// Fill in the UI elements on the row - name, cost etc
	public void PopulateUI()
	{
		UnityEngine.UI.Text itemNameTextComponent = m_itemNameText.GetComponent<UnityEngine.UI.Text>();
		itemNameTextComponent.text = m_materialID.ToString();

		UnityEngine.UI.Text quantityTextComponent = m_quantityText.GetComponent<UnityEngine.UI.Text>();
		quantityTextComponent.text = m_quantity.ToString();
	}
}
