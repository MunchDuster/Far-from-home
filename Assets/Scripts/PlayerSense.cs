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

	[Space(10)]
	public float errorTime = 1f;
	public GameObject errorParent;
	public TextMeshProUGUI errorText;

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
			Debug.Log("Clicking!");
			InteractionInfo info = curHover.Interact(player);

			//Display error if fail
			if (!info.success) DisplayError(info.info);
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
	private void DisplayError(string errorMessage)
	{
		errorParent.SetActive(true);
		errorText.text = errorMessage;
		StartCoroutine(HideError());
	}
	private IEnumerator HideError()
	{
		yield return new WaitForSeconds(errorTime);
		errorParent.SetActive(false);
	}
}