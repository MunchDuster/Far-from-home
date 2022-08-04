using UnityEngine;
using UnityEngine.Events;

public class SplitEvent: MonoBehaviour
{
	public UnityEvent OnTrue;
	public UnityEvent OnFalse;

	public void Call(bool state)
	{
		if (state) OnTrue.Invoke();
		else OnFalse.Invoke();
	}
}