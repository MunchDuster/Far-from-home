using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class EnterableInteractable : Minigame
{
	public RequirementList enterRequirements;

	public UnityEvent<bool> OnJoin;

	protected void Start()
	{
		enterRequirements.Start();
		OnPlayerJoin += PlayerJoin;
	}
	protected void PlayerJoin(bool on)
	{
		if (OnJoin != null) OnJoin.Invoke(on);
	}

	public void CompleteRequirement(int no)
	{
		enterRequirements.requirements[no].SetCompleted(true);
	}

	public void UnCompleteRequirement(int no)
	{
		enterRequirements.requirements[no].SetCompleted(false);
	}

	protected override InteractionInfo CheckRequirements(Player player)
	{
		if (enterRequirements.completed) return InteractionInfo.Success();
		else return InteractionInfo.Fail(enterRequirements.GetIncompleteTask().description);
	}

	
}