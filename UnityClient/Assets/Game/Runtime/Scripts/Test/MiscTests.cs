using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[AddComponentMenu("SPACEJAM/Tests/MiscTests")]
public class MiscTests : MonoBehaviour
{
	[LocalizationKey]
	public string m_stringID;

	public enum BitMasks
	{
		A = 0x01,
		B = 0x02,
		C = 0x04,
		D = 0x08
	};

	[BitMask(typeof(BitMasks))]
	public BitMasks m_bitfield = BitMasks.A | BitMasks.C;

}
