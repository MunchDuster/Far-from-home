using UnityEngine;

public class PlayerSense : MonoBehaviour
{
	public float raycastDist;
	public LayerMask layerMask;
	public Transform raycastPoint;

	[Space(10)]
	public Player player;

	private Interactable lastHover = null;

	//Update is called every frame.
	private void Update()
	{
		UpdateHover();
		UpdateInteraction();
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
				curHover.StartHover(player);

				//Interact if key pressed
				if (Input.GetMouseButtonDown(0)) curHover.Interact(player);
			}
			else lastHover.EndHover(player);
		}
		lastHover = curHover;
	}
	private Interactable GetCurHover()
	{
		Interactable curHover = null;

		if (Physics.Raycast(raycastPoint.position, raycastPoint.forward, out RaycastHit hit, raycastDist, layerMask))
		{
			curHover = hit.gameObject.GetComponentInParent<Interactable>();
		}

		return curHover;
	}
}