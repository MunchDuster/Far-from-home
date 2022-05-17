using UnityEngine;
using UnityEngine.Events;

public class Detector : MonoBehaviour
{
	public UnityEvent OnDetected;

	// On Trigger Enter is called when the collider of another GameObject begins colliding with the collider of this GameObject (while this collider is trigger)
	private void OnTriggerEnter(Collider otherCollider)
	{
		if (otherCollider.gameObject.tag == "Player")
		{
			Trigger();
		}
	}

	public void Trigger()
	{
		if (OnDetected != null) OnDetected.Invoke();
	}
}
