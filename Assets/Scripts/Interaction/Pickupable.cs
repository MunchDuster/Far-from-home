using UnityEngine;
using UnityEngine.Events;

public class Pickupable : Interactable
{
	public UnityEvent OnPickup;
	public Vector3 offset; 
	public override InteractionInfo Interact(Player player)
	{
		if (player.pickuper.item == null)
		{
			player.pickuper.Pickup(this);

			if(OnPickup != null) OnPickup.Invoke();
			return InteractionInfo.Success();
			
		}
		else return InteractionInfo.Fail("Already holding item, right click to drop.");
	}
	public void MakeUndetectable(Transform target = null)
	{
		HideDescendants(transform);
	}

	private void HideDescendants(Transform target)
	{
		target.gameObject.layer = 7;//Wearing layer

		foreach (Transform child in target)
		{
			HideDescendants(child);
		}
	}
}