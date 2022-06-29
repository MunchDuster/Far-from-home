using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Task
{
	public string name;
	public string description;
	[SerializeField] private bool _completed = false;

	public delegate void OnBoolEvent(bool aBool);
	public event OnBoolEvent OnCompleted;

	//Getter which calls OnCompleted when set true
	public bool completed { get { return _completed; } }

	//Used by unityEvents
	public void SetCompleted(bool isCompleted)
	{
		_completed = isCompleted;
		if (OnCompleted != null) OnCompleted.Invoke(_completed);
	}
}