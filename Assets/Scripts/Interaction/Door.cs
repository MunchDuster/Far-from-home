using UnityEngine;
using UnityEngine.Events;

public class Door : Interactable
{
	public RequirementList openRequirements;

	//Main vars, controlled internally
	private bool open;
	public bool unlocked { get { return openRequirements == null || openRequirements.completed; } }

	//UnityEvents triggering chain events
	public UnityEvent OnOpen;
	public UnityEvent OnClose;
	public UnityEvent OnUnlock;
	public UnityEvent OnLock;

	//Reference to animator
	public Animator animator;

	//Start is called before first update.
	private void Start()
	{
		animator = GetComponent<Animator>();

		openRequirements.Start();

		openRequirements.onCompleted += UpdateLocked;
		openRequirements.onUncompleted += UpdateLocked;
	}

	//Called by UnityEvents to chage hover info
	public void SetInfo(string info)
	{
		hoverInfoText = info;
	}

	//Attempt to toggle open/close if not locked
	public override InteractionInfo Interact(Player player)
	{
		Task incompleteTask = openRequirements.GetIncompleteTask();

		if (incompleteTask != null) return InteractionInfo.Fail(incompleteTask.description);


		SetOpen(!open);

		return InteractionInfo.Success();
	}

	//Main opening/closing door function
	private void SetOpen(bool open)
	{
		this.open = open;
		animator.SetBool("open", open);

		if (open)
		{
			if (OnOpen != null) OnOpen.Invoke();
		}
		else
		{
			if (OnClose != null) OnClose.Invoke();
		}

	}

	//Call UnLock event when all requirements are met
	private void UpdateLocked()
	{
		Debug.Log("Updating locked: " + unlocked);
		if (unlocked)
		{
			if (OnUnlock != null) OnUnlock.Invoke();
		}
		else
		{
			if (OnLock != null) OnLock.Invoke();
			SetOpen(false);
		}
	}

	//Allow for unityEvents to set task completed
	public void CompleteOpenRequirement(string name)
	{
		openRequirements.SetTaskCompleted(name, true);
	}
	public void UncompleteOpenRequirement(string name)
	{
		openRequirements.SetTaskCompleted(name, false);
	}
	public void CloseNow()
	{
		SetOpen(false);
	}
	public void OpenNow()
	{
		SetOpen(true);
	}
}