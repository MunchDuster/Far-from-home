using UnityEngine;
using UnityEngine.Events;

public class Door : InteractableWithRequirements
{
	//Main vars, controlled internally
	private bool open;
	public bool unlocked { get { return requirementsMet; } }

	//UnityEvents triggering chain events
	public UnityEvent OnOpen;
	public UnityEvent OnClose;
	public UnityEvent OnUnlock;

	//Reference to animator
	public Animator animator;

	//Start is called before first update.
	protected override void Start()
	{
		animator = GetComponent<Animator>();

		//Call start method of parent class
		base.Start();
	}

	//Called by UnityEvents to chage hover info
	public void SetInfo(string info)
	{
		hoverInfoText = info;
	}

	//Attempt to toggle open/close if not locked
	public override InteractionInfo Interact(Player player)
	{
		Task incompleteTask = GetIncompleteTask();

		if (incompleteTask != null) return InteractionInfo.Fail(incompleteTask.description);

		open = !open;

		animator.SetBool("open", open);

		if (open)
		{
			if (OnOpen != null) OnOpen.Invoke();
		}
		else
		{
			if (OnClose != null) OnClose.Invoke();
		}

		return InteractionInfo.Success();
	}

	//Call UnLock event when all requirements are met
	protected override void OnRequirementsMet()
	{
		if (OnUnlock != null) OnUnlock.Invoke();
	}
}