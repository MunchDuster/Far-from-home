using UnityEngine;
using TMPro;
using System.Collections;

public class CardLock : Lock
{

	public Pickupable card;
	public new Camera camera;
	public TMP_Text text;
	public Vector3 cardRotationOffset = Vector3.zero;
	public int maxSwipes = 20;

	[Space(10)]
	public Transform slideStart;
	public Transform slideEnd;

	[Space(10)]
	public float targetSpeed = 4;
	public float targetSpeedRange = 0.2f;

	public float lowFrameRateBoundary = 20;
	public float lowFrameRatetargetSpeed = 4;
	public float lowFrameRatetargetSpeedRange = 0.2f;

	//Holds y value of item
	private float y;
	private float lastY;

	//Holds dist from cam to lock
	private float dist;

	//Shortcuts
	private float maxSpeed { get { return targetSpeed + (targetSpeedRange / 2f); } }
	private float minSpeed { get { return targetSpeed - (targetSpeedRange / 2f); } }

	private int scans = 0;

	// Start is called before the first frame update
	private void Start()
	{
		OnPlayerJoin += PlayerJoin;
		OnGameUpdate += GameUpdate;
	}

	private void PlayerJoin(bool on)
	{
		if(on)
		{
			//Set item position and rotation
			player.pickuper.item.transform.position = slideStart.position;
			player.pickuper.item.transform.rotation = slideStart.rotation * Quaternion.Euler(cardRotationOffset);

			//Init vars
			y = slideStart.position.y;
			lastY = y;
			dist = (player.camera.transform.position - transform.position).magnitude;
		}
		else
		{
			if (player.pickuper.item != null)
			{
				player.pickuper.item.transform.localPosition = Vector3.zero;
				player.pickuper.item.transform.localRotation = Quaternion.identity;
			}
		}
	}

	protected override InteractionInfo CheckRequirements(Player player)
	{
		if (player.pickuper.item == null) return InteractionInfo.Fail("Needs an item to use.");
		else if (player.pickuper.item != card) return InteractionInfo.Fail("Can't use this item.");
		else return InteractionInfo.Success();
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
	private void GameUpdate()
	{
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
	private Coroutine textDisplayCoroutine;

	//The actual scan code
	private void Scan()
	{
		if (player.pickuper.item == card)
		{
			scans++;

			if(scans >= maxSwipes)
			{
				textDisplayCoroutine = StartCoroutine(ShowText("Validate Success", 2));
				if (OnUnlock != null) OnUnlock.Invoke();
				return;
			}

			float speed = Mathf.Abs((y - lastY) / Time.deltaTime);

			if (textDisplayCoroutine != null) StopCoroutine(textDisplayCoroutine);

			if (speed > maxSpeed)
			{
				textDisplayCoroutine = StartCoroutine(ShowText("<color=\"red\">Error: Too fast.</color>", 2));
				if (OnFail != null) OnFail.Invoke();
			}
			else if (speed < minSpeed)
			{
				textDisplayCoroutine = StartCoroutine(ShowText("<color=\"red\">Error: Too slow.</color>", 2));
				if (OnFail != null) OnFail.Invoke();
			}
			else
			{
				textDisplayCoroutine = StartCoroutine(ShowText("<color=\"green\">Validate Success</color>", 2));
				if (OnUnlock != null) OnUnlock.Invoke();
			}
		}
		else
		{
			textDisplayCoroutine = StartCoroutine(ShowText("<color=\"red\">Error: Uniditentified item</color>", 2));
			if (OnFail != null) OnFail.Invoke();
		}
	}

	private IEnumerator ShowText(string str, float time)
	{
		text.text = str;
		yield return new WaitForSeconds(time);
		text.text = "";
	}
}
