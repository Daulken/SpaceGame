using System;
using System.IO;
using Serialization;



public static class Storage
{
	// Serialize an object to a compressed format in a BASE64 string
	public static string SerializeToString(object obj)
	{
		return Convert.ToBase64String(UnitySerializer.Serialize(obj));	
	}
	
	// Deserialize a compressed object from a string
	public static object Deserialize(string data)
	{
		return UnitySerializer.Deserialize(Convert.FromBase64String(data));
	}

	// Typed deserialization
	public static T Deserialize<T>(string data) where T : class
	{
		return Deserialize(data) as T;
	}
}

