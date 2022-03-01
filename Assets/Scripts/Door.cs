using UnityEngine;
using UnityEngine.Events;

public class Door : Interactable
{
	public bool open;
	public bool unlocked;
	public string lockedReason;

	public UnityEvent OnOpen;
	public UnityEvent OnClose;

	public Animator animator;


	//Start is called before first update.
	private void Start()
	{
		animator = GetComponent<Animator>();
	}


	public override InteractionInfo Interact(Player player)
	{
		if (!unlocked) return InteractionInfo.Fail(lockedReason);

		open = !open;

		animator.SetBool("open", open);

		if (open) { if (OnOpen != null) OnOpen.Invoke(); }
		else if (OnClose != null) OnClose.Invoke();

		return InteractionInfo.Success();
	}
}