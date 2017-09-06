using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("SPACEJAM/Streaming/DontDestroyOnSceneLoad")]
public class DontDestroyOnSceneLoad : MonoBehaviour
{
	// Use this for initialization
	protected void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}
}
