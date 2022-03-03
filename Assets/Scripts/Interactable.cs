using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
	public Transform hoverInfoPoint;
	public string hoverName;
	public string hoverInfoText;

	public Outline outline;

	public abstract InteractionInfo Interact(Player player);

	public void StartHover(HoverInfo hoverInfo)
	{
		//Show outline
		outline.enabled = true;

		//Show info
		hoverInfo.gameObject.SetActive(true);
		hoverInfo.SetInfo(hoverName, hoverInfoText);
		hoverInfo.positionPoint = hoverInfoPoint;
	}

	public void EndHover(HoverInfo hoverInfo)
	{
		//Hide outline
		outline.enabled = false;

		//Hide info
		hoverInfo.gameObject.SetActive(false);
	}

}
