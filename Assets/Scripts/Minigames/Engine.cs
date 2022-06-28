using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Engine : Minigame
{
	public float bodyStiffness = 3;
	public float nozzleStiffness = 3;
	public float alignmentStiffness = 3;
	public float maxFuel = 40;

	[Space(10)]
	public Pickupable fuelCan;
	public Flow fuelFlow;
	public Color sliderStartColor;
	public Color sliderStopColor;

	[Space(10)]
	public Transform itemPlane;
	public Transform fuelPoint;
	public Transform fuelHandle;
	public Transform fuelNozzle;

	[Space(10)]
	public Slider fullnessSlider;
	public Image fullnessSliderImage;

	[Space(10)]
	public UnityEvent OnStart;
	public UnityEvent OnStop;
	public UnityEvent OnFuelled;

	private Vector3 targetPos;
	private Rigidbody rb;
	private Plane plane;
	private float fuel;

	// Start is called before the first frame update
	protected void Start()
	{
		OnPlayerJoin += PlayerJoin;
		OnGameUpdate += UpdateInput;
		OnGameFixedUpdate += GameUpdate;

		plane = new Plane(itemPlane.forward, itemPlane.position);
	}

	protected override InteractionInfo CheckRequirements(Player player)
	{
		if (player.pickuper.item != fuelCan) return InteractionInfo.Fail("You must be holding fuel can.");
		else return InteractionInfo.Success();
	}

	private void PlayerJoin(bool on)
	{
		if(on)
		{
			OnStart.Invoke();
			targetPos = GetItemTargetPosition(new Vector2(Screen.width / 2, Screen.height / 2));
			SetupPlayer();
			SetupFuelCan();
			SetupSlider();
		}
		else
		{
			OnStop.Invoke();
			rb.isKinematic = true;
			fullnessSliderImage.color = sliderStopColor;
			player.pickuper.isAllowedToDropItem = true;
			//Reset pickup position
			fuelCan.transform.localPosition = Vector3.zero;
			fuelCan.transform.localRotation = Quaternion.identity;
		}
	}
	private void SetupSlider()
	{
		fullnessSlider.value = fuel / maxFuel;
		fullnessSliderImage.color = (fuel < maxFuel) ? sliderStartColor : sliderStopColor;
	}
	private void SetupPlayer()
	{
		rb = player.pickuper.item.GetComponentInChildren<Rigidbody>();
		rb.isKinematic = false;
		player.pickuper.isAllowedToDropItem = false;
	}
	private void SetupFuelCan()
	{
		fuelFlow.fuelPoint = fuelPoint;
		fuelFlow.engine = this;
		
		fuelCan.GetComponent<Collider>().enabled = true;
		fuelCan.transform.position = targetPos;
	}

	private void UpdateInput()
	{
		if (Input.GetMouseButton(0))
		{
			targetPos = GetItemTargetPosition(Input.mousePosition);
		}
	}
	private void GameUpdate()
	{
		Vector3 fuelHandleOnPlane = plane.ClosestPointOnPlane(fuelHandle.position);


		//Go to mouse position
		Vector3 handleDirection = targetPos - fuelHandleOnPlane;
		Debug.DrawRay(fuelHandle.position, handleDirection, Color.red);
		rb.AddForceAtPosition(handleDirection.normalized * bodyStiffness * Time.fixedDeltaTime * handleDirection.magnitude, fuelHandle.position);

		//Point towards feulPoint
		Vector3 nozzleDirection = fuelPoint.position - fuelNozzle.position;
		Debug.DrawRay(fuelNozzle.position, nozzleDirection, Color.green);
		rb.AddForceAtPosition(nozzleDirection.normalized * nozzleStiffness * Time.fixedDeltaTime, fuelNozzle.position);

		Vector3 rbOnPlane = plane.ClosestPointOnPlane(rb.position);
		float distance = (rbOnPlane - rb.position).magnitude;
		Vector3 alignmentForce = (rbOnPlane - rb.position) * (distance) * alignmentStiffness;
		Debug.DrawRay(rb.position, alignmentForce, Color.blue);
		rb.AddForce(alignmentForce);
	}

	private Vector3 GetItemTargetPosition(Vector2 screenPos)
	{
		Ray ray = player.camera.ScreenPointToRay(screenPos);

		if (plane.Raycast(ray, out float enter))
		{
			Vector3 hitPoint = ray.GetPoint(enter);
			return hitPoint;
		}
		else
		{
			return Vector3.zero;
		}
	}

	public void AddFuel(float amount)
	{
		if (fuel >= maxFuel) return;

		fuel += amount;

		if (fuel >= maxFuel)
		{
			Debug.Log("Full!");
			fullnessSlider.value = 1;
			fullnessSliderImage.color = sliderStopColor;
			OnFuelled.Invoke();
		}
		else
		{
			fullnessSlider.value = fuel / maxFuel;
		}
	}
}