using UnityEngine;
using UnityEngine.Events;

public class Welder : MonoBehaviour
{
	public Transform gun;
	public Transform gunPoint;
	public WeldPlate plate;

	[Space(10)]
	public float weldDist = 0.2f;
	public float heatPerSecond = 1;
	public float gunDistanceFromPlane = 0.5f;
	public LayerMask weldLayerMask;

	[Space(10)]
	public UnityEvent OnTurnOn;
	public UnityEvent OnTurnOff;
	private bool isHeating;

	//Update is called every frame.
	public void GameUpdate()
	{
		Debug.Log("Welder game update");
		if (Input.GetMouseButton(0))
		{
			if (!isHeating)
			{
				StartHeat();
			}

			UpdateGunTargetPos();
		}
		else
		{
			if (isHeating)
			{
				EndHeat();
			}
		}

		UpdateGunPos();
	}

	private void StartHeat()
	{
		isHeating = true;
		plate.StartWelding();
		OnTurnOn.Invoke();
	}

	private void EndHeat()
	{
		isHeating = false;
		plate.StopWelding();
		OnTurnOff.Invoke();
	}

	private void ApplyHeat()
	{
		Ray ray = new Ray(gunPoint.position, gunPoint.forward);

		Vector3 point = plate.

		if (Physics.Raycast(gun.position, gun.forward, out RaycastHit hit, weldDist, weldLayerMask))
		{

			bool isPlateGameObject = plate.transform.Find(hit.collider.gameObject.name);
			if (isPlateGameObject == plate)
			{
				Debug.Log("Applying heat");
				plate.AddHeat(hit.point, heatPerSecond * Time.deltaTime);
			}
			else
			{
				Debug.Log("Not overing over plate");
			}
		}
	}

	Vector3 gunTargetPos;
	private void UpdateGunTargetPos()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		plate.plane.Raycast(ray, out float distance);

		gunTargetPos = ray.GetPoint(distance) + plate.plane.normal * gunDistanceFromPlane;
	}

	private void UpdateGunPos()
	{
		gun.rotation = Quaternion.LookRotation(plate.plane.normal) * Quaternion.Euler(-90, 0, 0);
		gun.position = gunTargetPos;
	}

	// OnDrawGizmos is called every editor update
	private void OnDrawGizmos()
	{
		if (gunTargetPos != null)
		{
			Gizmos.DrawSphere(gunTargetPos, 0.5f);
		}
	}
}