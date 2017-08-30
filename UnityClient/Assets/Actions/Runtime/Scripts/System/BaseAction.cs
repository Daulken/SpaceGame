using UnityEngine;
#if UNITY_EDITOR
	using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

namespace Actions {

		
	public abstract class BaseAction : MonoBehaviour
	{
		public float m_delaySeconds = 0.0f;

		private class PerExecuteInfo
		{
			public List<System.UInt64> m_dependantActionHandles = new List<System.UInt64>();
			public float m_startTimeSeconds = -1.0f;
			public float m_endTimeSeconds = -1.0f;
			public bool m_realTime = false;
		};

		private Dictionary<System.UInt64, PerExecuteInfo> m_perExecuteInfo = new Dictionary<System.UInt64, PerExecuteInfo>(1);
		private Dictionary<System.UInt64, float> m_perExecuteDelayEndTimeSeconds = new Dictionary<System.UInt64, float>(1);
		private bool m_shuttingDown = false;
		private float m_pauseTimeSeconds = -1.0f;
		private bool m_cached = false;
	
		// This is called whenever the node is enabled.
		// NOTE: ALL scripts require this function, even if not used, to avoid a bug with script execution order being ignored
		protected virtual void OnEnable()
		{
			// Having this here adds the enable tickbox to all actions, so we can add functionality to skip them when disabled
		}

		// Called when the application shuts down
		protected virtual void OnApplicationQuit()
		{
			// Set our flag to indicate we are shutting down
			m_shuttingDown = true;
		}
		
		// Called when this object is destroyed
		protected virtual void OnDestroy()
		{
			if (!m_shuttingDown)
			{
				// Cancel any current actions being played by this action
				if (m_perExecuteInfo.Count > 0)
				{
					foreach (System.UInt64 executeHandle in m_perExecuteInfo.Keys)
					{
						// Go through all execute handles. Use for, which, while slower for lists, avoids memory allocation,
						// which is important, since we need to call this function in GC.Collect sensitive areas.
						List<System.UInt64> dependantActionHandles = m_perExecuteInfo[executeHandle].m_dependantActionHandles;
						for (int dependantHandleIndex = 0, maxDependantHandles = dependantActionHandles.Count; dependantHandleIndex < maxDependantHandles; ++dependantHandleIndex)
						{
							System.UInt64 dependantActionHandle = dependantActionHandles[dependantHandleIndex];
							ActionListManager.Cancel(dependantActionHandle);
						}
					}
					m_perExecuteInfo.Clear();
				}

				// Clear any start delays
				m_perExecuteDelayEndTimeSeconds.Clear();
			}
		}

		// Called whenever the application is paused or unpaused due to alt-tab or other switching behaviour
		protected virtual void OnApplicationPause(bool pause)
		{
			if (pause)
			{
				m_pauseTimeSeconds = Time.realtimeSinceStartup;
			}
			else if (m_pauseTimeSeconds >= 0.0f)
			{
				float timePaused = Time.realtimeSinceStartup - m_pauseTimeSeconds;
				m_pauseTimeSeconds = -1.0f;

				foreach (PerExecuteInfo perExecuteInfo in m_perExecuteInfo.Values)
				{
					if (!perExecuteInfo.m_realTime)
						continue;
					perExecuteInfo.m_startTimeSeconds += timePaused;
					perExecuteInfo.m_endTimeSeconds += timePaused;
				}

				List<System.UInt64> keys = new List<System.UInt64>(m_perExecuteDelayEndTimeSeconds.Keys);
				foreach (System.UInt64 executeHandle in keys)
					m_perExecuteDelayEndTimeSeconds[executeHandle] += timePaused;
			}
		}
		
		
		#region Derived Class API

		// Called whenever values are changed in the editor, or at startup, to cache values that can then be used
		// multiple times. For example, to create lists or arrays that would be expensive to create each perform
		protected virtual void RecacheValues()
		{
		}

		// Called when the action is executed. The given flags indicate whether the action is being run as part of an immediate or realtime
		// action list, and a list of action list handles that this action should add to if it executes any. Returns action duration in seconds.
		protected virtual float PerformAction(bool immediate, bool realTime, ref List<System.UInt64> dependantActionHandles)
		{
			DebugHelpers.LogWarningContext("BaseAction::PerformAction on \"{0}\" has not been overridden by derived action", this, gameObject.GetNameWithPath());
			return 0.0f;
		}
	
		// Called when an actionlist that this belongs to is cancelled, and this action has been executed. This can happen even if
		// already finished, so long as the actionlist it belongs to is running. The given flags indicate whether the action is still
		// running within the duration returned from PerformAction (and therefore active), whether it is waiting on a dependant action
		// action to finish, and whether the action list itself has finished, and is only waiting on one or more dependant actions to
		// finish. If the first 2 flags are unset, the action has finished and has no dependants, but later actions are still running.
		// Dependant executing action lists are cancelled automatically, so this does not need to handle it.
		protected virtual void CancelAction(bool actionStillRunning, bool actionHasActiveDependants, bool actionListFinished)
		{
		}

		#endregion
	
		#region ActionListManager Interface

		// Set the this action starts its realtime delay
		public void SetRealTimeDelayStartTime(System.UInt64 executeHandle)
		{
			// Set the time for this execution that the action ends its real-time start delay
			m_perExecuteDelayEndTimeSeconds[executeHandle] = Time.realtimeSinceStartup + m_delaySeconds;
		}
		
		// Get whether this action has finished its realtime start delay
		public bool RealTimeDelayFinished(System.UInt64 executeHandle)
		{
			// Check whether this action has a start delay for the given action list handle.
			float delayEndTime;
			if (!m_perExecuteDelayEndTimeSeconds.TryGetValue(executeHandle, out delayEndTime))
				return true;

			// Return whether the time for this execution has finished
			return (Time.realtimeSinceStartup >= delayEndTime);
		}

		// Start an action running
		public void TriggerPerformAction(System.UInt64 executeHandle, bool immediate, bool realTime, ref List<System.UInt64> dependantActionHandles)
		{
			// Remove any start delay information
			m_perExecuteDelayEndTimeSeconds.Remove(executeHandle);

			// Add per-execute info for this handle
			PerExecuteInfo perExecuteInfo = new PerExecuteInfo();

			// Cache any values required for multiple runs
			if (!m_cached)
			{
				RecacheValues();
				m_cached = true;
			}
			
			// Perform the action, recording the duration, and any dependant action handles
			float duration = PerformAction(immediate, realTime, ref perExecuteInfo.m_dependantActionHandles);

			// Add any new action handles to the main dependant action handle list as well
			dependantActionHandles.AddRange(perExecuteInfo.m_dependantActionHandles);
	
			// Set the times between which this action is running
			perExecuteInfo.m_realTime = realTime;
			perExecuteInfo.m_startTimeSeconds = perExecuteInfo.m_realTime ? Time.realtimeSinceStartup : Time.time;
			perExecuteInfo.m_endTimeSeconds = perExecuteInfo.m_startTimeSeconds + duration;
			m_perExecuteInfo[executeHandle] = perExecuteInfo;
		}

		// Query the status of the action in an action list
		public void QueryActionStatus(System.UInt64 executeHandle, out bool actionHasExecuted, out bool actionStillRunning, out bool actionHasActiveDependants)
		{
			// Check whether this action has been executed for the given action list handle.
			// (will not be present if not yet executed, or if the entire action list has finished)
			PerExecuteInfo perExecuteInfo = null;
			if (!m_perExecuteInfo.TryGetValue(executeHandle, out perExecuteInfo))
			{
				actionHasExecuted = false;
				actionStillRunning = false;
				actionHasActiveDependants = false;
				return;
			}

			// If info found, we have executed, and the action list is still running, even if we aren't
			actionHasExecuted = true;

			// Calculate whether this action itself if still running - the list may have moved
			// on to a later action, or this one is waiting on a dependant action list to finish
			float currentTimeSeconds = perExecuteInfo.m_realTime ? Time.realtimeSinceStartup : Time.time;
			actionStillRunning = ((currentTimeSeconds >= perExecuteInfo.m_startTimeSeconds) && (currentTimeSeconds < perExecuteInfo.m_endTimeSeconds));

			// Check whether any dependant action handles are active
			actionHasActiveDependants = ActionListManager.IsExecuting(perExecuteInfo.m_dependantActionHandles);
		}

		// Cancel a running action list
		public void TriggerCancelAction(System.UInt64 executeHandle, bool actionListFinished)
		{
			// Remove any start delay information
			m_perExecuteDelayEndTimeSeconds.Remove(executeHandle);

			// Check whether this action has been executed for the given action list handle.
			// (will not be present if not yet executed, or if the entire action list has finished)
			PerExecuteInfo perExecuteInfo;
			if (m_perExecuteInfo.TryGetValue(executeHandle, out perExecuteInfo))
			{
				// Calculate whether this action itself if still running - it may have moved
				// on to a later one, or waiting on a dependant action list to finish
				float currentTimeSeconds = perExecuteInfo.m_realTime ? Time.realtimeSinceStartup : Time.time;
				bool actionStillRunning = ((currentTimeSeconds >= perExecuteInfo.m_startTimeSeconds) && (currentTimeSeconds < perExecuteInfo.m_endTimeSeconds));

				// Check whether any dependant action handles are active
				bool actionHasActiveDependants = ActionListManager.IsExecuting(perExecuteInfo.m_dependantActionHandles);
				if (actionHasActiveDependants)
				{
					// Cancel any dependant action handles for this execution
					// Go through all execute handles. Use for, which, while slower for lists, avoids memory allocation,
					// which is important, since we need to call this function in GC.Collect sensitive areas.
					List<System.UInt64> dependantActionHandles = m_perExecuteInfo[executeHandle].m_dependantActionHandles;
					for (int dependantHandleIndex = 0, maxDependantHandles = dependantActionHandles.Count; dependantHandleIndex < maxDependantHandles; ++dependantHandleIndex)
						ActionListManager.Cancel(dependantActionHandles[dependantHandleIndex]);
				}

				// Tell this action to cancel, regardless of whether active or not. It may need to undo something
				CancelAction(actionStillRunning, actionHasActiveDependants, actionListFinished);
			}

			// Remove any state information for this execution
			m_perExecuteInfo.Remove(executeHandle);
		}

		// Stop an action after its action list finishes naturally
		public void TriggerStopAction(System.UInt64 executeHandle)
		{
			// Remove any start delay information
			m_perExecuteDelayEndTimeSeconds.Remove(executeHandle);

			// Remove any state information for this execution
			m_perExecuteInfo.Remove(executeHandle);
		}

		#endregion

		#region Editor Interface

#if UNITY_EDITOR
		// Called whenever values are changed in the editor
		public void MarkDirty()
		{
			EditorUtility.SetDirty(this);
			m_cached = false;
			RecacheValues();
		}
#endif

		#endregion
		
	}
	

}	// namespace Actions
