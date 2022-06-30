using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class ToString: MonoBehaviour
{
	public UnityEvent<string> Event;

	public void FloatToString(float value)
	{
		Event.Invoke(value.ToString());
	}
}