using UnityEngine;
using UnityEngine.Events;

public abstract class Lock : Interactable
{
	public UnityEvent OnUnlock;

	public override abstract InteractionInfo Interact(Player player);
}
