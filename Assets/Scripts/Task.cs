using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Task
{
	public string name;
	public string description;
	[SerializeField] private bool _completed = false;

	public delegate void OnEvent();
	public event OnEvent OnCompleted;
	public event OnEvent OnUncompleted;

	//Getter which calls OnCompleted when set true
	public bool completed { get { return _completed; } }

	//Used by unityEvents
	public void SetCompleted(bool isCompleted)
	{
		_completed = isCompleted;

		if (_completed)
		{
			if (OnCompleted != null) OnCompleted.Invoke();
		}
		else
		{
			if (OnUncompleted != null) OnUncompleted.Invoke();
		}
	}
}