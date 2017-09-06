using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUIMessageHandler : UnityEngine.EventSystems.IEventSystemHandler
{
	void OnPageChanged();
}
