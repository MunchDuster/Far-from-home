using UnityEngine;

public class NumberLock : Lock
{
	public override InteractionInfo Interact(Player player)
	{
		return InteractionInfo.Success();
	}
}
