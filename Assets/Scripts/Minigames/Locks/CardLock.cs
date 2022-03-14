using UnityEngine;
using TMPro;

public class CardLock : Lock
{

	public Pickupable card;
	public new Camera camera;
	public TextMeshProUGUI text;
	public Vector3 cardRotationOffset = Vector3.zero;

	[Space(10)]
	public Transform slideStart;
	public Transform slideEnd;

	[Space(10)]
	public float targetSpeed = 4;
	public float targetSpeedRange = 0.2f;

	//Holds y value of item
	private float y;
	private float lastY;

	//Holds dist from cam to lock
	private float dist;

	//Shortcuts
	private float maxSpeed { get { return targetSpeed + (targetSpeedRange / 2f); } }
	private float minSpeed { get { return targetSpeed - (targetSpeedRange / 2f); } }

	protected override void StartPlaying()
	{
		if (player.pickuper.item == null) return;

		//Init position and rotation
		player.pickuper.item.transform.position = slideStart.position;
		player.pickuper.item.transform.rotation = slideStart.rotation * Quaternion.Euler(cardRotationOffset);

		//Init y position
		y = slideStart.position.y;
		lastY = y;

		//Init dist
		dist = (player.camera.position - transform.position).magnitude;
	}
	protected override void StopPlaying()
	{
		if (player.pickuper.item.transform != null)
		{
			player.pickuper.item.transform.localPosition = Vector3.zero;
			player.pickuper.item.transform.localRotation = Quaternion.identity;
		}
	}

	// Gets angle of mouse
	private float GetMouseAngleX()
	{
		Vector3 pixel = new Vector3(Screen.width / 2f, Input.mousePosition.y, 0);
		Ray ray = camera.ScreenPointToRay(pixel);
		float angle = Vector3.SignedAngle(Vector3.forward, ray.direction, Vector3.right);
		return angle;
	}

	//Checks whether to do scan
	private bool wasBelow = false;
	private void CheckScan()
	{
		if (y < transform.position.y)
		{
			if (!wasBelow)
			{
				Scan();
				wasBelow = true;
			}
		}
		else
		{
			wasBelow = false;
		}
	}

	//Main update while user is playing
	protected override void GameUpdate()
	{
		Debug.Log("Game update.");

		if (player.pickuper.item == null) return;

		//If the user is holding left mouse down
		if (Input.GetMouseButton(0))
		{
			//Drag the keycard to the mouse
			float angle = GetMouseAngleX();

			//Update y value
			y = dist * Mathf.Tan(angle * Mathf.Deg2Rad) + transform.position.y;
			y = Mathf.Clamp(y, slideEnd.position.y, slideStart.position.y);

			//Checks whether to call sacen function
			CheckScan();

			//Update transform position
			player.pickuper.item.transform.position = new Vector3(player.pickuper.item.transform.position.x, y, player.pickuper.item.transform.position.z);

			//Update lastY
			lastY = y;
		}
	}

	//The actual scan code
	private void Scan()
	{
		if (player.pickuper.item == card)
		{
			float speed = Mathf.Abs((y - lastY) / Time.deltaTime);

			Debug.Log("speed: " + speed);
			if (speed > maxSpeed)
			{
				text.text = "<color=\"red\">Error: Too fast.</color>";
				if (OnFail != null) OnFail.Invoke();
			}
			else if (speed < minSpeed)
			{
				text.text = "<color=\"red\">Error: Too slow.</color>";
				if (OnFail != null) OnFail.Invoke();
			}
			else
			{
				if (OnUnlock != null) OnUnlock.Invoke();
				text.text = "Validation success.";
			}
		}
		else
		{
			text.text = "<color=\"red\">Error: Uniditentified item.</color>";
			if (OnFail != null) OnFail.Invoke();
		}
	}
}
