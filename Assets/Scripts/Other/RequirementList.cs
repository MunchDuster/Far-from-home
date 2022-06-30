using UnityEngine;

[System.Serializable]
public class RequirementList
{
	public Task[] requirements;
	[HideInInspector] public bool completed;

	public delegate void OnBoolEvent(bool aBool);
	public OnBoolEvent onCompleted;

	//Called before first frame update
	public void Start()
	{
		if (requirements == null)
		{
			completed = true;
			return;
		}

		foreach (Task task in requirements)
		{
			task.OnCompleted += UpdateCompleted;
		}
		UpdateCompleted(true); //Init completed
	}

	//Checks whether requirements are met, if so then OnCompleted is called
	private void UpdateCompleted(bool completedTask)
	{
		//Task not complete, complete is false
		if(!completedTask)
		{
			completed = false;
			return;
		}

		//Task is true, check for ay other incomplete task
		Task incompleteTask = GetIncompleteTask();
		completed = incompleteTask == null;
		if (onCompleted != null) onCompleted(completed);
	}

	//Finds an incompleted task, returns null if all completed
	public Task GetIncompleteTask()
	{
		if (requirements == null) return null;

		Task task = System.Array.Find(requirements, task => { return !task.completed; });

		return task;
	}

	//Used by unityEvents to complete tasks
	public void SetTaskCompleted(string name, bool completed)
	{
		Task task = System.Array.Find(requirements, s => s.name == name);

		if (task != null)
		{
			task.SetCompleted(completed);
			Debug.Log("Task " + name + ", set completed " + completed);
		}
		else Debug.LogError("Requirement List CompleteTask by name, task not found: " + name);
	}
}