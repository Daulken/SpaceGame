using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("SPACEJAM/GameState/Market/InventoryUILayout")]
public class InventoryUILayout : MonoBehaviour
{
	public SharedUILayout m_sharedLayout;

	// Use this for initialization
	void Start ()
	{
		SharedUILayout.OnPageChanged += OnPageChanged;
	}

	void OnDestroy()
	{
		SharedUILayout.OnPageChanged -= OnPageChanged;
	}

	// Update is called once per frame
	void Update ()
	{

	}

	public void OnPageChanged()
	{
	}
}
