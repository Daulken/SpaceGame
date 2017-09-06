using UnityEngine;
using System;
using System.Collections;

public static class DebugHelpers
{
	public static void Log(string message, params object[] parameters)
	{
		LogContext(message, null, parameters);
	}
	 
	public static void LogContext(string message, UnityEngine.Object context, params object[] parameters)
	{  
		if (!UnityEngine.Debug.isDebugBuild)
			return;
		string text = string.Format(message, parameters);
		if (context != null)
			UnityEngine.Debug.Log(text, context);
		else
			UnityEngine.Debug.Log(text);
	}
		   
	public static void LogWarning(string message, params object[] parameters)
	{  
		LogWarningContext(message, null, parameters);
	}
	 
	public static void LogWarningContext(string message, UnityEngine.Object context, params object[] parameters)
	{  
		if (!UnityEngine.Debug.isDebugBuild)
			return;
		string text = string.Format(message, parameters);
		if (context != null)
			UnityEngine.Debug.LogWarning(text, context);
		else
			UnityEngine.Debug.LogWarning(text);
	}
	
	public static void LogError(string message, params object[] parameters)
	{  
		LogErrorContext(message, null, parameters);
	}
	 
	public static void LogErrorContext(string message, UnityEngine.Object context, params object[] parameters)
	{  
		if (!UnityEngine.Debug.isDebugBuild)
			return;
		string text = string.Format(message, parameters);
		if (context != null)
			UnityEngine.Debug.LogError(text, context);
		else
			UnityEngine.Debug.LogError(text);
	}

	public static void LogException(Exception exception)
	{  
		LogExceptionContext(exception, null);
	}
	 
	public static void LogExceptionContext(Exception exception, UnityEngine.Object context)
	{  
		if (!UnityEngine.Debug.isDebugBuild)
			return;
		if (context != null)
			UnityEngine.Debug.LogException(exception, context);
		else
			UnityEngine.Debug.LogException(exception);
	}
}
