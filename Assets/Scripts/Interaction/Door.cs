using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Door : Interactable
{
	public RequirementList openRequirements;

	public bool unlocked { get { return openRequirements == null || openRequirements.completed; } }

	//UnityEvents triggering chain events
	public UnityEvent<bool> OnOpen;
	public UnityEvent<bool> OnUnlock;

	[SerializeField] private bool open;


	//Reference to animator
	public Animator animator;

	//Start is called before first update.
	private void Start()
	{
		animator = GetComponent<Animator>();

		openRequirements.Start();

		openRequirements.onCompleted += UpdateLocked;

		//SetOpen(open);
		UpdateLocked(unlocked);
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

		if (!unlocked) return InteractionInfo.Fail(incompleteTask.description);


		SetOpen(!open);

		return InteractionInfo.Success();
	}

	//Main opening/closing door function
	public void SetOpen(bool open)
	{
		this.open = open;
		animator.SetBool("open", open);

		if (OnOpen != null) OnOpen.Invoke(open);
	}

	//Call UnLock event when all requirements are met
	private void UpdateLocked(bool unlocked)
	{
		if (OnUnlock != null) OnUnlock.Invoke(unlocked);
	}

	//Allow for unityEvents to set task completed
	public void CompleteOpenRequirement(bool complete, string name)
	{
		openRequirements.SetTaskCompleted(name, complete);
	}

	public void OpenAfter(float time)
	{
		StartCoroutine(SetOpenAfter(time, true));
	}

	private IEnumerator SetOpenAfter(float time, bool open)
	{
		yield return new WaitForSeconds(time);
		SetOpen(open);
	}
}