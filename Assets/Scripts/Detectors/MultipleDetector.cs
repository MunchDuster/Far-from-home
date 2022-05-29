using UnityEngine;
using UnityEngine.Events;

public class MultipleDetector : MonoBehaviour
{
	[System.Serializable]
	public class Detection
	{
		public string name;
		public UnityEvent OnDetected;
	}

	public Detection[] detections;

	public void TriggerDetect(string name)
	{
		for (int i = 0; i < detections.Length; i++)
		{
			if(detections[i].name == name)
			{
				detections[i].OnDetected.Invoke();
			}
		}
	}
}
