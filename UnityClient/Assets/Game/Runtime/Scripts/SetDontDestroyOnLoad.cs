using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("SPACEJAM/SetDontDestroyOnLoad")]
public class SetDontDestroyOnLoad : MonoBehaviour
{
	// Use this for initialization
	protected void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}
}
