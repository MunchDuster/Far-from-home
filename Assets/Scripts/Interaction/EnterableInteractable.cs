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
		OnPlayerJoin += PlayerJoin;
	}
	protected void PlayerJoin(bool on)
	{
		if(on)
		{
			if (OnJoin != null) OnJoin.Invoke();
		}
		else
		{
			if (OnLeave != null) OnLeave.Invoke();
		}
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