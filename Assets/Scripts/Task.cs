using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Task
{
	public string name;
	public string description;

	public UnityEvent OnCompleted;

	//Getter which calls OnCompleted when set true
	private bool _completed = false;
	public bool completed { get { return _completed; } }

	//Used by unityEvents
	public void SetCompleted(bool isCompleted)
	{
		_completed = isCompleted;

		if (_completed)
		{
			if (OnCompleted != null)
			{
				OnCompleted.Invoke();
			}
		}
	}
}