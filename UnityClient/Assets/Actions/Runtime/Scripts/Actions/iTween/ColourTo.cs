using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Actions {

	/// <summary>
	/// Global colour channel type
	/// </summary>
	public enum ColourChannels
	{
		Red = 0x01,
		Green = 0x02,
		Blue = 0x04,
		Alpha = 0x08
	}
	
	[AddComponentMenu("SPACEJAM/Actions/Actions/iTween/ColourTo")]
	public class ColourTo : BaseAction
	{
		public GameObject m_gameObject = null;
		public Color m_colour = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		public ColourChannels m_channels = ColourChannels.Red | ColourChannels.Green | ColourChannels.Blue | ColourChannels.Alpha;
		public bool m_recursive = true;
		public float m_duration = 0.0f;
		public iTween.EaseType m_easeType = iTween.EaseType.linear;
		public iTween.LoopType m_loopType = iTween.LoopType.none;
		public int m_loopCount = -1;
		public float m_iTweenDelay = 0.0f;
	
		private bool m_hasRed = true;
		private bool m_hasGreen = true;
		private bool m_hasBlue = true;
		private bool m_hasAlpha = true;
		private iTween.ArgumentData m_iTweenArgs = null;
	
	
		// Called whenever values are changed in the editor, or at startup, to cache values that can then be used
		// multiple times. For example, to create lists or arrays that would be expensive to create each perform
		protected override void RecacheValues()
		{
			// Test the active colour channels
			m_hasRed = (m_channels & ColourChannels.Red) != 0;
			m_hasGreen = (m_channels & ColourChannels.Green) != 0;
			m_hasBlue = (m_channels & ColourChannels.Blue) != 0;
			m_hasAlpha = (m_channels & ColourChannels.Alpha) != 0;
			
			// Create iTween arguments as required
			if (m_iTweenArgs == null)
				m_iTweenArgs = new iTween.ArgumentData();
			m_iTweenArgs.Set("duration", m_duration);
			m_iTweenArgs.Set("easetype", m_easeType);
			m_iTweenArgs.Set("looptype", m_loopType);
			m_iTweenArgs.Set("loopcount", m_loopCount);
			m_iTweenArgs.Set("delay", m_iTweenDelay);
			m_iTweenArgs.Set("includechildren", m_recursive);
			List<string> tags = new List<string>();
			if (m_hasRed)
			{
				m_iTweenArgs.Set("r", m_colour.r);
				tags.Add("colorto_red");
			}
			if (m_hasGreen)
			{
				m_iTweenArgs.Set("g", m_colour.g);
				tags.Add("colorto_green");
			}
			if (m_hasBlue)
			{
				m_iTweenArgs.Set("b", m_colour.b);
				tags.Add("colorto_blue");
			}
			if (m_hasAlpha)
			{
				m_iTweenArgs.Set("a", m_colour.a);
				tags.Add("colorto_alpha");
			}
			m_iTweenArgs.Set("tags", tags);
		}

		// Called when the action is executed. The given flags indicate whether the action is being run as part of an immediate or realtime
		// action list, and a list of action list handles that this action should add to if it executes any. Returns action duration in seconds.
		protected override float PerformAction(bool immediate, bool realTime, ref List<System.UInt64> dependantActionHandles)
		{
			// Check for game object
			if (m_gameObject == null)
			{
				DebugHelpers.LogWarningContext("ColourTo action on \"{0}\" has no GameObject specified", this, gameObject.GetNameWithPath());
				return 0.0f;
			}
				
			// Check for instant change
			if ((m_duration <= 0.0f) || immediate || !m_gameObject.activeInHierarchy)
			{
				// Cancel all ColorTo iTween transitions on this game object that have the same channels. Only one ColorTo on a channel can execute at a time
				if (m_hasRed)
					iTween.StopByTag(m_gameObject, "colorto_red", m_recursive, true);
				if (m_hasGreen)
					iTween.StopByTag(m_gameObject, "colorto_green", m_recursive, true);
				if (m_hasBlue)
					iTween.StopByTag(m_gameObject, "colorto_blue", m_recursive, true);
				if (m_hasAlpha)
					iTween.StopByTag(m_gameObject, "colorto_alpha", m_recursive, true);
	
				// Apply colour instantly
				if (m_recursive)
					HelperFunctions.RecursiveApplyColour(m_gameObject.transform, m_colour, m_hasRed, m_hasGreen, m_hasBlue, m_hasAlpha);
				else
					HelperFunctions.ApplyColour(m_gameObject.transform, m_colour, m_hasRed, m_hasGreen, m_hasBlue, m_hasAlpha);
				return 0.0f;
			}

			// Set the realtime flag
			m_iTweenArgs.Set("ignoretimescale", realTime);
			
			// Execute the iTween
			if (!iTween.ColorTo(m_gameObject, m_iTweenArgs))
				return 0.0f;
	
			// Return the time the action lasts for
			int loopCount = (m_loopType == iTween.LoopType.none)?0:m_loopCount;
			return (loopCount < 0)?Mathf.Infinity:((m_iTweenDelay + m_duration) * (m_loopCount + 1));
		}
	
		// Called when an actionlist that this belongs to is cancelled, and this action has been executed. This can happen even if
		// already finished, so long as the actionlist it belongs to is running. The given flags indicate whether the action is still
		// running within the duration returned from PerformAction (and therefore active), whether it is waiting on a dependant action
		// action to finish, and whether the action list itself has finished, and is only waiting on one or more dependant actions to
		// finish. If the first 2 flags are unset, the action has finished and has no dependants, but later actions are still running.
		// Dependant executing action lists are cancelled automatically, so this does not need to handle it.
		protected override void CancelAction(bool actionStillRunning, bool actionHasActiveDependants, bool actionListFinished)
		{
			// Cancel all ColorTo iTween transitions on this game object that have the same channels. Only one ColorTo on a channel can execute at a time
			if (actionStillRunning && (m_gameObject != null))
			{
				if (m_hasRed)
					iTween.StopByTag(m_gameObject, "colorto_red", m_recursive, false);
				if (m_hasGreen)
					iTween.StopByTag(m_gameObject, "colorto_green", m_recursive, false);
				if (m_hasBlue)
					iTween.StopByTag(m_gameObject, "colorto_blue", m_recursive, false);
				if (m_hasAlpha)
					iTween.StopByTag(m_gameObject, "colorto_alpha", m_recursive, false);
			}
		}
	}


}	// namespace Actions
