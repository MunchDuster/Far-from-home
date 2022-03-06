using UnityEngine;

public abstract class InteractableWithRequirements : Interactable
{
	[Header("Interactable Requirements")]
	public Task[] requirements;

	//For children to quickly check whether all requirements are met
	protected bool requirementsMet;

	//Called before first frame update
	protected virtual void Start()
	{
		foreach (Task task in requirements)
		{
			task.OnCompleted.AddListener(UpdateRequirementsMet);
		}
		UpdateRequirementsMet();
	}

	//Checks whether requirements are met, if so then OnRequirementsMet is called
	private void UpdateRequirementsMet()
	{
		Debug.Log("UpdateRequirementsMet");
		Task incompleteTask = GetIncompleteTask();
		if (incompleteTask == null) OnRequirementsMet();
	}

	//Pass implementation down to child
	public override abstract InteractionInfo Interact(Player player);

	//Finds an incompleted task, returns null if all completed
	protected Task GetIncompleteTask()
	{
		if (requirements == null) return null;

		foreach (Task task in requirements)
		{
			if (!task.completed) return task;
		}

		return null;
	}

	//For child when all tasks are completed
	protected abstract void OnRequirementsMet();

	//Used by unityEvents to complete tasks
	public void CompleteTask(int index)
	{
		if (requirements == null || index >= requirements.Length)
		{
			Debug.LogError("InteractableWithRequirements Error: index to complete task does not exist.");
			return;
		}

		requirements[index].SetCompleted(true);
	}
}