using UnityEngine;
using UnityEngine.Events;

public class MovementListener : MonoBehaviour
{
	public UnityEvent OnMoveStart;
	public UnityEvent OnMoveStop;

	private bool wasMoving = false;

	// Update is called once per frame
	void Update()
	{
		bool isMoving = CheckIsMoving();

		if (isMoving && !wasMoving && OnMoveStart != null) OnMoveStart.Invoke();
		if (!isMoving && wasMoving && OnMoveStop != null) OnMoveStop.Invoke();

		wasMoving = isMoving;
	}

	bool CheckIsMoving()
	{
		bool movingHorizontal = Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f;
		bool movingVertical = Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f;
		return movingHorizontal || movingVertical;
	}
}
