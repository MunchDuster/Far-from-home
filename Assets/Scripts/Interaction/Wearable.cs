using UnityEngine;
using UnityEngine.Events;

public class Wearable : Interactable
{
	public string wearingHoverName;
	public string wearingHoverInfo;
	public UnityEvent OnWear;
	public UnityEvent OnUnwear;

	public RequirementList wearRequirements;
	public RequirementList takeOffRequirements;

	public bool canBeTakenOff { get { return takeOffRequirements == null || takeOffRequirements.completed; } }
	public bool canBeWorn { get { return wearRequirements == null || wearRequirements.completed; } }

	private bool isBeingWorn = false;
	private string notWearingHoverName;
	private string notWearingHoverInfo;

	// Start is called before the first frame update
	private void Start()
	{
		wearRequirements.Start();
		takeOffRequirements.Start();

		notWearingHoverName = hoverName;
		notWearingHoverInfo = hoverInfoText;
	}

	public override InteractionInfo Interact(Player player)
	{
		if (isBeingWorn)
		{
			if (canBeTakenOff)
			{
				if (OnUnwear != null) OnUnwear.Invoke();
				isBeingWorn = false;

				hoverName = notWearingHoverName;
				hoverInfoText = notWearingHoverInfo;
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
				isBeingWorn = true;

				hoverName = wearingHoverName;
				hoverInfoText = wearingHoverInfo;
			}
			else
			{
				return InteractionInfo.Fail(wearRequirements.GetIncompleteTask().description);
			}
		}
		return InteractionInfo.Success();
	}
}
