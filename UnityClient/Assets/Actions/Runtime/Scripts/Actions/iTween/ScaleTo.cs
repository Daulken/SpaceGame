using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Actions {
		
	
	[AddComponentMenu("SPACEJAM/Actions/Actions/iTween/ScaleTo")]
	public class ScaleTo : BaseAction
	{
		public Transform m_transform = null;
		public Vector3 m_scale = new Vector3(1.0f, 1.0f, 1.0f);
		public float m_duration = 0.0f;
		public iTween.EaseType m_easeType = iTween.EaseType.linear;
		public iTween.LoopType m_loopType = iTween.LoopType.none;
		public int m_loopCount = -1;
		public float m_iTweenDelay = 0.0f;

		private iTween.ArgumentData m_iTweenArgs = null;
		
		// Called whenever values are changed in the editor, or at startup, to cache values that can then be used
		// multiple times. For example, to create lists or arrays that would be expensive to create each perform
		protected override void RecacheValues()
		{
			// Create iTween arguments as required
			if (m_iTweenArgs == null)
				m_iTweenArgs = new iTween.ArgumentData();
			m_iTweenArgs.Set("scale", m_scale);
			m_iTweenArgs.Set("duration", m_duration);
			m_iTweenArgs.Set("easetype", m_easeType);
			m_iTweenArgs.Set("looptype", m_loopType);
			m_iTweenArgs.Set("loopcount", m_loopCount);
			m_iTweenArgs.Set("delay", m_iTweenDelay);
			m_iTweenArgs.Set("islocal", true);
		}

		// Called when the action is executed. The given flags indicate whether the action is being run as part of an immediate or realtime
		// action list, and a list of action list handles that this action should add to if it executes any. Returns action duration in seconds.
		protected override float PerformAction(bool immediate, bool realTime, ref List<System.UInt64> dependantActionHandles)
		{
			// Check for transform
			if (m_transform == null)
			{
				DebugHelpers.LogWarningContext("ScaleTo action on \"{0}\" has no Transform specified", this, gameObject.GetNameWithPath());
				return 0.0f;
			}
			
			// Check for instant change
			if ((m_duration <= 0.0f) || immediate || !m_transform.gameObject.activeInHierarchy)
			{
				// Cancel all Scale iTween transitions on this game object. Only one Scale can execute at a time
				iTween.StopByType(m_transform.gameObject, "Scale", false, false);
				
				m_transform.localScale = m_scale;
				return 0.0f;
			}

			// Set the realtime flag
			m_iTweenArgs.Set("ignoretimescale", realTime);

			// Execute the iTween
			if (!iTween.ScaleTo(m_transform.gameObject, m_iTweenArgs))
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
			// Cancel all Scale iTween transitions on this game object. Only one Scale can execute at a time
			if (actionStillRunning && (m_transform != null))
				iTween.StopByType(m_transform.gameObject, "Scale", false, false);
		}
	}
	

}	// namespace Actions
