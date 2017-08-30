using UnityEngine;
#if UNITY_EDITOR
	using UnityEditor;
#endif
using System;
using System.Collections;
using System.Collections.Generic;

namespace Actions {

	[AddComponentMenu("")]
	public class ActionListManager : MonoBehaviour
	{
		private struct ExecuteInfo
		{
			public GameObject m_actionList;
			public Actions.BaseAction[] m_actions;
		};

		private static ActionListManager ms_instance = null;
		private Dictionary<System.UInt64, ExecuteInfo> m_executingCoroutines = new Dictionary<System.UInt64, ExecuteInfo>();
		private System.UInt64 m_nextExecuteHandle = 1;

		// Awake is called at game startup, regardless of enable state
		protected void Awake()
		{
			// If no instance exists, use this one
			if (ms_instance == null)
				ms_instance = this;
		
			// If we're not the instance that things use, remove ourself, as we're duplicate
			if (ms_instance != this)
			{
				bool destroyGameObject = true;
				Component[] components = gameObject.GetComponents(typeof(Component));
				foreach (Component component in components)
				{
					if (component is Transform)
						continue;
					if (component is ActionListManager)
						continue;
					destroyGameObject = false;
					break;
				}

				if (destroyGameObject)
					Destroy(gameObject);
				else
					Destroy(this);
			}
			// This is our instance
			else
			{
				// Ensure we don't get destroyed between scenes
				DontDestroyOnLoad(gameObject);
			}
		}

		// Called when this object is destroyed
		protected void OnDestroy()
		{
			// Remove our global instance pointer
			if (ms_instance == this)
				ms_instance = null;
		}

		// Ensures that there is an instance available to work with
		private static void EnsureInstanceExists()
		{
			// If an instance exists already, do nothing
			if ((ms_instance != null) && (ms_instance.gameObject != null))
				return;	

			// Look for an instance of this type in the scene already
			ms_instance = (ActionListManager)GameObject.FindObjectOfType(typeof(ActionListManager));

			// Create one if not found
			if (ms_instance == null)
			{
#if UNITY_EDITOR
				GameObject go = UnityEditor.EditorUtility.CreateGameObjectWithHideFlags("_Global_ActionListManager", HideFlags.HideAndDontSave);
#else
				GameObject go = new GameObject("_Global_ActionListManager");
				go.hideFlags = HideFlags.HideAndDontSave;
#endif
				ms_instance = go.AddComponent<ActionListManager>();
			}
		}

		// Called to execute an action list
		public static System.UInt64 Execute(GameObject actionObject, bool immediate, bool realTime)
		{
			// If there is no game object given, do nothing
			if (actionObject == null)
				return 0;

			// Ensure that there is an instance of the singleton available
			EnsureInstanceExists();

			// Cancel any currently running version of this action list
			Cancel(actionObject);

			// Get all actions on this game object
			ExecuteInfo executeInfo = new ExecuteInfo();
			executeInfo.m_actionList = actionObject;
			executeInfo.m_actions = actionObject.GetComponents<Actions.BaseAction>();

			// Start executing these actions over time
			System.UInt64 executeHandle = ms_instance.m_nextExecuteHandle++;
			if (ms_instance.m_nextExecuteHandle == 0)
				ms_instance.m_nextExecuteHandle++;
			ms_instance.m_executingCoroutines.Add(executeHandle, executeInfo);
			ms_instance.StartCoroutine(ms_instance.ExecuteOverTime(executeInfo, executeHandle, immediate, realTime));
			return executeHandle;
		}

		// Called to query if a specific actionlist handle is still executing
		public static bool IsExecuting(System.UInt64 executeHandle)
		{
			// If the action list handle is not valid, cannot be running
			if (executeHandle == 0)
				return false;

			// If an ActionListManager singleton is not available, cannot be running
			if ((ms_instance == null) || (ms_instance.gameObject == null))
				return false;

			// Check if this execute handle is in the list of active coroutines
			return ms_instance.m_executingCoroutines.ContainsKey(executeHandle);
		}

		// Called to query if a list of specific actionlist handles are still executing
		public static bool IsExecuting(List<System.UInt64> executeHandles)
		{
			// If an ActionListManager singleton is not available, cannot be running
			if ((ms_instance == null) || (ms_instance.gameObject == null))
				return false;
			
			// Go through all execute handles. Use for, which, while slower for lists, avoids memory allocation,
			// which is important, since we need to call this function in GC.Collect sensitive areas.
			for (int handleIndex = 0, maxHandles = executeHandles.Count; handleIndex < maxHandles; ++handleIndex)
			{
				System.UInt64 executeHandle = executeHandles[handleIndex];

				// If the action list handle is not valid, cannot be running
				if (executeHandle == 0)
					continue;
	
				// Check if this execute handle is in the list of active coroutines
				if (ms_instance.m_executingCoroutines.ContainsKey(executeHandle))
					return true;
			}

			// None found running
			return false;
		}

		// Called to query if a specific actionlist gameobject is still executing
		public static bool IsExecuting(GameObject actionObject)
		{
			// If the action list object is not valid, cannot be running
			if (actionObject == null)
				return false;

			// If an ActionListManager singleton is not available, cannot be running
			if ((ms_instance == null) || (ms_instance.gameObject == null))
				return false;
			
			// Iterate all active coroutines to see if the gameobject is used
			foreach (ExecuteInfo executeInfo in ms_instance.m_executingCoroutines.Values)
			{
				if (executeInfo.m_actionList == actionObject)
					return true;
			}
			return false;
		}

		// Called to stop a specific actionlist handle
		public static void Cancel(System.UInt64 executeHandle)
		{
			// If the action list handle is not valid, do nothing
			if (executeHandle == 0)
				return;

			// Ensure that there is an instance of the singleton available
			EnsureInstanceExists();

			// Remove this execute handle from the list of active coroutines, if it exists
			ExecuteInfo executeInfo;
			if (ms_instance.m_executingCoroutines.TryGetValue(executeHandle, out executeInfo))
			{
				// Query whether this action list is still executing, purely because of dependant actions
				bool actionListFinished = true;
				bool actionsWaitingOnDependants = false;
				for (int itemIndex = 0, itemCount = executeInfo.m_actions.Length; itemIndex < itemCount; ++itemIndex)
				{
					Actions.BaseAction action = executeInfo.m_actions[itemIndex];

					bool actionHasExecuted;
					bool actionStillRunning;
					bool actionHasActiveDependants;
					action.QueryActionStatus(executeHandle, out actionHasExecuted, out actionStillRunning, out actionHasActiveDependants);
					if (!actionHasExecuted || actionStillRunning)
						actionListFinished = false;
					if (actionHasActiveDependants)
						actionsWaitingOnDependants = true;

					// If both flags have changed, no point querying further actions
					if (!actionListFinished && actionsWaitingOnDependants)
						break;
				}

				// Cancel all of these actions, in case they were over-time
				for (int itemIndex = 0, itemCount = executeInfo.m_actions.Length; itemIndex < itemCount; ++itemIndex)
				{
					Actions.BaseAction action = executeInfo.m_actions[itemIndex];
					action.TriggerCancelAction(executeHandle, actionListFinished);
				}

				ms_instance.m_executingCoroutines.Remove(executeHandle);
			}
		}

		// Called to stop a specific actionlist gameobject. Will stop multiple running instances of the same action list
		public static void Cancel(GameObject actionObject)
		{
			// Iterate all active coroutines to see if the gameobject is used
			List<System.UInt64> cancelHandles = new List<System.UInt64>();
			foreach (KeyValuePair<System.UInt64, ExecuteInfo> kvp in ms_instance.m_executingCoroutines)
			{
				// If this actionlist gameobject matches, cancel this co-routine
				if (kvp.Value.m_actionList == actionObject)
					cancelHandles.Add(kvp.Key);
			}

			// Now cancel the found keys. Done separately, because we cannot iterate executing coroutines
			// and cancel at the same time, because the cancel will modify the executing coroutines.
			// Go through all execute handles. Use for, which, while slower for lists, avoids memory allocation,
			// which is important, since we need to call this function in GC.Collect sensitive areas.
			for (int handleIndex = 0, maxHandles = cancelHandles.Count; handleIndex < maxHandles; ++handleIndex)
				Cancel(cancelHandles[handleIndex]);
		}
		
		// Called to execute actions over time
		private IEnumerator ExecuteOverTime(ExecuteInfo executeInfo, System.UInt64 executeHandle, bool immediate, bool realTime)
		{
			// Reset the maximum end time, and dependant action list handles
			List<System.UInt64> dependantActionHandles = new List<System.UInt64>();

			// Iterate through all of these actions
			for (int itemIndex = 0, itemCount = executeInfo.m_actions.Length; itemIndex < itemCount; ++itemIndex)
			{
				Actions.BaseAction action = executeInfo.m_actions[itemIndex];

				// If the action has been destroyed, or it is disabled, ignore the action
				if ((action == null) || (action.gameObject == null) || !action.enabled)
					continue;

				// Ensure that we wait until any delay has finished
				if (!immediate && (action.m_delaySeconds > 0.0f))
				{
					if (realTime)
					{
						action.SetRealTimeDelayStartTime(executeHandle);
						while (!action.RealTimeDelayFinished(executeHandle))
							yield return null;
					}
					else
					{
						yield return new WaitForSeconds(action.m_delaySeconds);
					}
				}

				// Check if this coroutine needs to abort
				if (!m_executingCoroutines.ContainsKey(executeHandle))
					yield break;

				// If the action has been destroyed while delaying, ignore the action
				if ((action == null) || (action.gameObject == null))
					continue;

				// Perform this action, recording the most future time of action finishing
				action.TriggerPerformAction(executeHandle, immediate, realTime, ref dependantActionHandles);
			}

			// Wait until all actions have finished before removing the execution handle.
			// Do this in blocks, so that we can still cancel even if waiting indefinitely
			// (for example, the duration of the action is infinite, such as a loop)
			while (true)
			{
				// Check if this coroutine needs to abort due to cancellation
				ExecuteInfo storedExecuteInfo;
				if (!m_executingCoroutines.TryGetValue(executeHandle, out storedExecuteInfo))
					break;
				
				// Query whether this action list is still executing, purely because of dependant actions
				bool stillWaiting = false;
				for (int itemIndex = 0, itemCount = storedExecuteInfo.m_actions.Length; itemIndex < itemCount; ++itemIndex)
				{
					Actions.BaseAction action = storedExecuteInfo.m_actions[itemIndex];

					// If the action has been destroyed while executing other actions, ignore the action
					if ((action == null) || (action.gameObject == null))
						continue;

					bool actionHasExecuted;
					bool actionStillRunning;
					bool actionHasActiveDependants;
					action.QueryActionStatus(executeHandle, out actionHasExecuted, out actionStillRunning, out actionHasActiveDependants);
					if (!actionHasExecuted || actionStillRunning || actionHasActiveDependants)
					{
						stillWaiting = true;
						break;
					}
				}

				// If the actions have all finished, and are not waiting on dependants, we have finished
				if (!stillWaiting)
					break;

				// Yield until next frame, when we check again
				yield return null;
			}

			// If this action list still exists (may have been cancelled)
			if (ms_instance.m_executingCoroutines.ContainsKey(executeHandle))
			{
				// Remove this execute handle from the list of active coroutines
				m_executingCoroutines.Remove(executeHandle);

				// Stop all of these actions, in case they were over-time
				foreach (Actions.BaseAction action in executeInfo.m_actions)
					action.TriggerStopAction(executeHandle);
			}
		}
	}


}	// namespace Actions
