using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Engine : Minigame
{
	public float stiffness = 3;
	public float nozzleStiffness = 3;
	public float maxFuel = 40;

	[Space(10)]
	public Pickupable fuelCan;
	public Flow fuelFlow;
	public Color sliderStartColor;
	public Color sliderStopColor;

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

	private Plane plane;

	private float fuel;


	// Start is called before the first frame update
	protected void Start()
	{
		OnPlayerJoin += StartGame;
		OnPlayerLeave += StopGame;
		OnGameUpdate += UpdateInput;
		OnGameFixedUpdate += GameUpdate;

		plane = new Plane(itemPlane.forward, itemPlane.position);
	}

	protected override InteractionInfo CheckRequirements(Player player)
	{
		if (player.pickuper.item != fuelCan) return InteractionInfo.Fail("You must be holding fuel can.");
		else return InteractionInfo.Success();
	}

	private Rigidbody rb;

	private void StartGame()
	{
		OnStart.Invoke();

		rb = player.pickuper.item.GetComponentInChildren<Rigidbody>();

		rb.isKinematic = false;

		targetPos = GetItemTargetPosition(new Vector2(Screen.width / 2, Screen.height / 2));

		fuelFlow.fuelPoint = fuelPoint;

		//Reset pickup position
		fuelCan.transform.position = targetPos;

		fullnessSliderImage.color = sliderStartColor;
	}

	private Vector3 targetPos;

	private void UpdateInput()
	{
		if (Input.GetMouseButton(0))
		{
			targetPos = GetItemTargetPosition(Input.mousePosition);
		}
	}
	private void GameUpdate()
	{

		//Go to mouse position
		Vector3 handleDirection = targetPos - fuelHandle.position;
		Debug.DrawRay(fuelHandle.position, handleDirection, Color.red);
		rb.AddForceAtPosition(handleDirection.normalized * stiffness * Time.fixedDeltaTime * handleDirection.magnitude, fuelHandle.position);

		//Point towards feulPoint
		Vector3 nozzleDirection = fuelPoint.position - fuelNozzle.position;
		Debug.DrawRay(fuelNozzle.position, nozzleDirection, Color.green);
		rb.AddForceAtPosition(nozzleDirection.normalized * nozzleStiffness * Time.fixedDeltaTime, fuelNozzle.position);

	}
	private void StopGame()
	{
		OnStop.Invoke();

		rb.isKinematic = true;

		fullnessSliderImage.color = sliderStopColor;


		//Reset pickup position
		fuelCan.transform.localPosition = Vector3.zero;
		fuelCan.transform.localRotation = Quaternion.identity;
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
			fullnessSlider.value = 1;
			OnFuelled.Invoke();
		}
		else
		{
			fullnessSlider.value = fuel / maxFuel;
		}
	}
}