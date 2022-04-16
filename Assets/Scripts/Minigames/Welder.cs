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
		if (Input.GetMouseButton(0))
		{
			if (!isHeating)
			{
				StartHeat();
			}
			else
			{
				ApplyHeat();
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

		OnTurnOn.Invoke();
	}

	private void EndHeat()
	{
		isHeating = false;
		OnTurnOff.Invoke();
	}

	private void ApplyHeat()
	{
		Ray ray = new Ray(gunPoint.position, gunPoint.forward);

		plate.plane.Raycast(ray, out float intersect);
		Vector3 point = ray.GetPoint(intersect);

		plate.AddHeat(point, heatPerSecond * Time.deltaTime);
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
}