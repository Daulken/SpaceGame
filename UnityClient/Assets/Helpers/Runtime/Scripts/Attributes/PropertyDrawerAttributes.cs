using UnityEngine;
using System;

// NOTE: These cannot be namespaced

// Turns a System.Enum property into a bit-mask in the Inspector
public class BitMaskAttribute : PropertyAttribute
{
	public System.Type m_propType;

	public BitMaskAttribute(System.Type propType)
	{
		m_propType = propType;
	}
}
