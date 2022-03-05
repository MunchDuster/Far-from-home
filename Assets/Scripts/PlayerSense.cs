using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerSense : MonoBehaviour
{
	public float raycastDist = 5f;
	public LayerMask layerMask;
	public Transform raycastPoint;

	[Space(10)]
	public Player player;
	public HoverInfo hoverInfo;
	public PlayerUI playerUI;


	private Interactable lastHover = null;

	//Update is called every frame.
	private void Update()
	{
		UpdateHover();
	}

	private void UpdateHover()
	{
		Interactable curHover = GetCurHover();

		//If hover changed
		if (curHover != lastHover)
		{
			//If hovering overinteractable
			if (curHover != null)
			{
				curHover.StartHover(hoverInfo);

			}
			else lastHover.EndHover(hoverInfo);
		}


		//Interact if key pressed
		if (Input.GetMouseButtonDown(0) && curHover != null)
		{
			InteractionInfo info = curHover.Interact(player);

			//Display error if fail
			if (!info.success) playerUI.ShowError(info.info);
		}
		lastHover = curHover;
	}


	private Interactable GetCurHover()
	{
		Interactable curHover = null;

		if (Physics.Raycast(raycastPoint.position, raycastPoint.forward, out RaycastHit hit, raycastDist, layerMask))
		{
			curHover = hit.collider.gameObject.GetComponentInParent<Interactable>();
		}

		return curHover;
	}
}