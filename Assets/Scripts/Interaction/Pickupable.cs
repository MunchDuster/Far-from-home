using UnityEngine;

public class Pickupable : Interactable
{
	public override InteractionInfo Interact(Player player)
	{
		if (player.pickuper.item == null)
		{
			Debug.Log("Picking up item.");
			player.pickuper.Pickup(this);
			return InteractionInfo.Success();
		}
		else return InteractionInfo.Fail("Already holding item, right click to drop.");
	}
}