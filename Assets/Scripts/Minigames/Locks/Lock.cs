using UnityEngine;
using UnityEngine.Events;

public abstract class Lock : Minigame
{
	public UnityEvent OnUnlock;
	public UnityEvent OnFail;
}
