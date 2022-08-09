using UnityEngine;
using UnityEngine.Events;

public class Pickupable : Interactable
{
	public UnityEvent OnPickup;
	public Vector3 offset;
	public Vector3 offsetRotation;
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

	public void MakeDetectable(bool detectable)
	{
		if(detectable)
		{
			foreach(Transform child in GetComponentsInChildren<Transform>())
			{
				child.gameObject.layer = 0; //Default layer
			}
		}
		else
		{
			foreach(Transform child in GetComponentsInChildren<Transform>())
			{
				child.gameObject.layer = 7; //Wearing layer
			}
		}
		
	}
}