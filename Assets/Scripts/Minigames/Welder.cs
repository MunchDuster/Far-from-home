using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class Welder : MonoBehaviour
{
	public Transform gun;
	public Transform welderBase;
	public Transform gunPoint;
	public WeldPlate plate;

	[Space(10)]
	public float weldDist = 0.2f;
	public float heatPerSecond = 1;
	public float gunDistanceFromPlane = 0.5f;
	public LayerMask weldLayerMask;

	[Space(10)]
	public UnityEvent<bool> OnTurnOn;
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

		ApplyHeat(); //TEMP

		UpdateGunPos();
	}

	private void StartHeat()
	{
		isHeating = true;
		OnTurnOn.Invoke(true);
	}

	private void EndHeat()
	{
		isHeating = false;
		OnTurnOn.Invoke(false);
	}

	Vector3 heatPoint;
	Ray heatRay;

	private void ApplyHeat()
	{
		heatRay = new Ray(gunPoint.position, gunPoint.forward);

		plate.plane.Raycast(heatRay, out float intersect);
		heatPoint = heatRay.GetPoint(intersect);
		plate.AddHeat(heatPoint, heatPerSecond * Time.deltaTime);
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
		gun.position = gunTargetPos;
		gun.rotation = Quaternion.LookRotation(plate.plane.normal) * Quaternion.Euler(-90, 0, 0);
	}

	// OnDrawGizmos is called every editor update
	private void OnDrawGizmos()
	{
		if(gunTargetPos != null)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(gunTargetPos, 0.1f);

			if(heatPoint != null)
			{
				Gizmos.color = Color.cyan;
				Gizmos.DrawWireSphere(heatPoint, 0.1f);
				Gizmos.color = Color.red;
				Gizmos.DrawRay(heatRay.origin, heatRay.direction);
				Gizmos.color = Color.yellow;
				Gizmos.DrawRay(gunPoint.position, gunPoint.forward);
			}
			Gizmos.color = Color.blue;
			Gizmos.DrawRay(plate.normal.position, plate.normal.forward);
		}

	}
}