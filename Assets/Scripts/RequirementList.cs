using UnityEngine;

[System.Serializable]
public class RequirementList
{
	public Task[] requirements;
	[HideInInspector] public bool completed;

	public delegate void OnEvent();
	public OnEvent onCompleted;
	public OnEvent onUncompleted;

	//Called before first frame update
	public void Start()
	{
		foreach (Task task in requirements)
		{
			task.OnCompleted.AddListener(UpdateCompleted);
		}
		UpdateCompleted();
	}

	//Checks whether requirements are met, if so then OnCompleted is called
	private void UpdateCompleted()
	{
		Debug.Log("UpdateCompleted");
		Task incompleteTask = GetIncompleteTask();
		if (incompleteTask == null)
		{

			Debug.Log("Completed true");
			bool wasCompleted = completed;
			completed = true;
			if (!wasCompleted && onCompleted != null) onCompleted();
		}
		else
		{
			Debug.Log("Completed false");
			bool wasCompleted = completed;
			completed = false;
			if (wasCompleted && onUncompleted != null) onUncompleted();
		}
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