using UnityEngine;
using UnityEngine.Events;

public abstract class Lock : Minigame
{
	public UnityEvent OnUnlock;
	public UnityEvent OnFail;

	// Start is called before the first frame update
	protected override void Start()
	{
		base.Start();

		requirementsToPlay.onCompleted += () => { Debug.Log("Unlocking door. "); OnUnlock.Invoke(); };

		OnPlayerJoin += StartPlaying;
		OnPlayerLeave += StopPlaying;
		OnGameUpdate += GameUpdate;
	}

	protected abstract void StartPlaying();
	protected abstract void StopPlaying();
	protected abstract void GameUpdate();
}
