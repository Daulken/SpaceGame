using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Diagnostics;


	
//------------------------------- Logging -------------------------------------
public static class RadicalLogger
{
	public static readonly bool DebugBuild;

	static RadicalLogger()
	{
		DebugBuild = UnityEngine.Debug.isDebugBuild;
	}
	
	// --------------------------------------------------------

	public static void LogNow(object message)
	{
		LogNow(message.ToString());
	}
	public static void LogNow(string message, params object[] parms)
	{
		if (!DebugBuild)
			return;
		UnityEngine.Debug.Log(string.Format(message, parms));
	}
	
	// --------------------------------------------------------

	public static void LogWarning(string message)
	{
		LogWarning ( message, null);
	}
	public static void LogWarning(string message, UnityEngine.Object context)
	{
		if (!DebugBuild)
			return;
		if (context != null)
		{
			UnityEngine.Debug.LogWarning(message, context);
		}
		else
		{
			UnityEngine.Debug.LogWarning(message);
		}
	}
	
	// --------------------------------------------------------

	public static void LogError(string message)
	{
		LogError(message, null);
	}
	public static void LogError(string message, UnityEngine.Object context)
	{
		if (!DebugBuild)
		{
			return;
		}
		if (context != null)
			UnityEngine.Debug.LogError(message, context);
		else
			UnityEngine.Debug.LogError(message);
	}

	public static void LogException(Exception exception)
	{
		LogException(exception, null);
	}
	public static void LogException(Exception exception, UnityEngine.Object context)
	{
		if (!DebugBuild)
		{
			return;
		}
		if (context != null)
			UnityEngine.Debug.LogException(exception, context);
		else
			UnityEngine.Debug.LogException(exception);
	}





	// -------------------------------------------------------------------------------------
	// -------------------------------------------------------------------------------------
	// -------------------------------------------------------------------------------------

	// Global disable flag for deferred logging
	public static bool AllowDeferredLogging = false;

	public static int _deferredLoggingEnabled = 0;
	private static int _deferredLogIndent=0;
	private static List<string> _deferredLogEntries = new List<string>();
	
	public class DeferredLogger : IDisposable
	{
		public DeferredLogger()
		{
			
			_deferredLoggingEnabled++;
		}
		
		public void Dispose()
		{
			_deferredLoggingEnabled--;
			if (_deferredLoggingEnabled == 0)
			{
				RadicalLogger.CommitDeferredLog();
			}
		}
	}
	
	public static bool DeferredLoggingEnabled
	{
		get
		{
			return _deferredLoggingEnabled > 0;
		}
	}
	
	public static bool IsDeferredLogging()
	{
		if (DebugBuild == false || !DeferredLoggingEnabled)
		{
			return false;
		}
		return true;
		
	}
	
	public static void DeferredLog(string message, params object[] parms)
	{
		if (DebugBuild == false || !DeferredLoggingEnabled || !AllowDeferredLogging)
		{
			return;
		}
		_deferredLogEntries.Add((new string(' ', 4 * _deferredLogIndent)) + string.Format(message, parms));
		if (_deferredLogEntries.Count > 50000)
		{
			_deferredLogEntries.RemoveAt(0);
		}
	}
	
	public static void IndentDeferredLog()
	{
		_deferredLogIndent++;
	}
	
	public static void OutdentDeferredLog()
	{
		_deferredLogIndent--;
	}
	
	public static void ClearDeferredLog()
	{
		_deferredLogEntries.Clear();
	}
	
	public static void CommitDeferredLog()
	{
		if (_deferredLogEntries.Count == 0)
		{
			return;
		}
		var sb = _deferredLogEntries.Aggregate((current, next) => current + "\n" + next);
		UnityEngine.Debug.Log(sb);
		_deferredLogEntries.Clear();
	}
	
}



