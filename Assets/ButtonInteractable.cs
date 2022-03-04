using UnityEngine.Events;

public class ButtonInteractable : Interactable
{
	public UnityEvent OnClick;
    
	public override InteractionInfo Interact(Player player)
    {
		OnClick.Invoke();
		return InteractionInfo.Success();
	}
}
