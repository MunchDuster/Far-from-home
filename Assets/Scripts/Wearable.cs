using UnityEngine;
using UnityEngine.Events;

public class Wearable : Interactable
{
	public UnityEvent OnWear;
	public UnityEvent OnUnwear;

	public RequirementList wearRequirements;
	public RequirementList takeOffRequirements;

	private bool isBeingWorn = false;

	public bool canBeTakenOff { get { return takeOffRequirements == null || takeOffRequirements.completed; } }
	public bool canBeWorn { get { return wearRequirements == null || wearRequirements.completed; } }

	// Start is called before the first frame update
	private void Start()
	{
		wearRequirements.Start();
		takeOffRequirements.Start();
	}

	public override InteractionInfo Interact(Player player)
	{
		if (isBeingWorn)
		{
			if (canBeTakenOff)
			{
				if (OnUnwear != null) OnUnwear.Invoke();
			}
			else
			{
				return InteractionInfo.Fail(takeOffRequirements.GetIncompleteTask().description);
			}
		}
		else
		{
			if (canBeWorn)
			{
				if (OnWear != null) OnWear.Invoke();
			}
			else
			{
				return InteractionInfo.Fail(wearRequirements.GetIncompleteTask().description);
			}
		}
		return InteractionInfo.Success();
	}
}
