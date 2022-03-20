using UnityEngine;
using UnityEngine.Events;

public class EnterableInteractable : Minigame
{
	public RequirementList enterRequirements;

	public UnityEvent OnJoin;
	public UnityEvent OnLeave;

	protected void Start()
	{
		enterRequirements.Start();
		OnPlayerJoin += () => { if (OnJoin != null) OnJoin.Invoke(); };
		OnPlayerLeave += () => { if (OnLeave != null) OnLeave.Invoke(); };
	}

	protected override InteractionInfo CheckRequirements(Player player)
	{
		if (enterRequirements.completed) return InteractionInfo.Success();
		else return InteractionInfo.Fail(enterRequirements.GetIncompleteTask().description);
	}
}