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

	protected int			m_numPages = 0;
	protected int			m_currentPage = 0;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}
