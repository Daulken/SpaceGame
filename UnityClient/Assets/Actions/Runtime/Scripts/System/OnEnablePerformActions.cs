using UnityEngine;
using System.Collections;

namespace Actions {


	[AddComponentMenu("SPACEJAM/Actions/Triggers/OnEnablePerformActions")]
	public class OnEnablePerformActions : MonoBehaviour
	{
		public GameObject m_actionListObject = null;
		public bool m_realTime = true;
		
		// This is non-zero if there are actions executing
		private System.UInt64 m_executingActions = 0;
		private bool m_playEnable = false;
		private bool m_shuttingDown = false;

		// This is called when enabled
		protected virtual void OnEnable()
		{
			// Cancel any current actions playing
			ActionListManager.Cancel(m_executingActions);
			m_executingActions = 0;

			// Play the enable actions
			m_playEnable = true;
		}
		
		// This is called whenever the node is updated
		protected virtual void Update()
		{
			// If playing the enable actions
			if (m_playEnable)
			{
				// If there is an action list specified, execute it over time
				m_executingActions = ActionListManager.Execute(m_actionListObject, false, m_realTime);
		
				// Now played
				m_playEnable = false;
			}
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
			// Cancel any current actions playing
			if (!m_shuttingDown)
			{
				ActionListManager.Cancel(m_executingActions);
				m_executingActions = 0;
			}
		}
	}


}	// namespace Actions
