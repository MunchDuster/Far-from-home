using UnityEngine;
using UnityEngine.Events;

public class EnterableInteractable : Minigame
{
	public UnityEvent OnJoin;
	public UnityEvent OnLeave;
	
	protected override void Start()
	{
		base.Start();

		OnPlayerJoin += () => { if (OnJoin != null) OnJoin.Invoke(); };
		OnPlayerLeave += () => { if (OnLeave != null) OnLeave.Invoke(); };
	}
}