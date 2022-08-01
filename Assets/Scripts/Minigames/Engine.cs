using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

public class Engine : Minigame
{
	public float bodyStiffness = 3;
	public float nozzleStiffness = 3;
	public float alignmentStiffness = 3;
	public float maxFuel = 40;

	[Space(10)]
	public FuelCan fuelCan;
	public Color sliderStartColor;
	public Color sliderStopColor;

	[Space(10)]
	public Transform itemPlane;
	public Transform fuelPoint;

	[Space(10)]
	public UnityEvent<bool> OnPlayerJoined;
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

		plane = new Plane(itemPlane.right, itemPlane.position);
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
			OnPlayerJoined.Invoke(true);
			targetPos = GetItemTargetPosition(new Vector2(Screen.width / 2, Screen.height / 2));
			SetupPlayer();
			SetupFuelCan();
			SetupSlider();
		}
		else
		{
			OnPlayerJoined.Invoke(false);
			rb.isKinematic = true;
			fuelCan.fullnessSliderFillImage.color = sliderStopColor;
			player.pickuper.isAllowedToDropItem = true;
			//Reset pickup position
			fuelCan.transform.localPosition = Vector3.zero;
			fuelCan.transform.localRotation = Quaternion.identity;
		}
	}
	private void SetupSlider()
	{
		fuelCan.fullnessSlider.value = fuel / maxFuel;
		fuelCan.fullnessSliderFillImage.color = (fuel < maxFuel) ? sliderStartColor : sliderStopColor;
	}
	private void SetupPlayer()
	{
		rb = player.pickuper.item.GetComponentInChildren<Rigidbody>();
		rb.isKinematic = false;
		player.pickuper.isAllowedToDropItem = false;
	}
	private void SetupFuelCan()
	{
		fuelCan.flow.fuelPoint = fuelPoint;
		fuelCan.flow.engine = this;
		
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
	private Vector3 GetSpringForce(Vector3 damping, Vector3 force)
	{
		return (force - damping) * Time.fixedDeltaTime;
	}

	[Space(10)]
	public float handleDampingStiffness = 1;
	public float nozzleDampingStiffness = 1;
	public float alignmentDampingStiffness = 1;
	private void GameUpdate()
	{
		//Go to mouse position
		Vector3 handleForce = (targetPos - plane.ClosestPointOnPlane(fuelCan.handle.position)) * bodyStiffness;
		Vector3 handleDamping = handleDampingStiffness * rb.GetPointVelocity(fuelCan.handle.position);
		Debug.DrawRay(fuelCan.handle.position, handleForce, Color.red);
		rb.AddForceAtPosition(GetSpringForce(handleDamping, handleForce), fuelCan.handle.position);

		//Point towards feulPoint
		Vector3 nozzleForce = (fuelPoint.position - fuelCan.nozzle.position).normalized * nozzleStiffness;
		Vector3 nozzleDamping = nozzleDampingStiffness * rb.GetPointVelocity(fuelCan.nozzle.position);
		Debug.DrawRay(fuelCan.nozzle.position, nozzleForce, Color.green);
		rb.AddForceAtPosition(GetSpringForce(nozzleDamping, nozzleForce), fuelCan.nozzle.position);

		//Keep on plane
		Vector3 alignmentForce = (plane.ClosestPointOnPlane(rb.position) - rb.position) * alignmentStiffness;
		Vector3 alignmentDamping = alignmentDampingStiffness * Vector3.Project(rb.velocity, plane.normal);
		Debug.DrawRay(rb.position, alignmentForce, Color.blue);
		rb.AddForce(GetSpringForce(alignmentDamping, alignmentForce));
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
			fuelCan.fullnessSlider.value = 1;
			fuelCan.fullnessSliderFillImage.color = sliderStopColor;
			OnFuelled.Invoke();
		}
		else
		{
			fuelCan.fullnessSlider.value = fuel / maxFuel;
		}
	}
}