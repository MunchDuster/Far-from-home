using UnityEngine;

public class CardLock : Lock
{
	public override InteractionInfo Interact(Player player)
	{
		return InteractionInfo.Success();
	}
}
