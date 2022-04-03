using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
	[Header("Hover Info")]
	public string hoverName;
	public string hoverInfoText;

	[Header("Interactable References")]
	public Transform hoverInfoPoint;
	public Outline outline;

	//On click
	public abstract InteractionInfo Interact(Player player);

	//Shows hover info
	public void StartHover(HoverInfo hoverInfo)
	{
		//Show outline
		outline.enabled = true;

		//Show info
		hoverInfo.gameObject.SetActive(true);
		hoverInfo.SetInfo(hoverName, hoverInfoText);
		hoverInfo.positionPoint = hoverInfoPoint;
	}

	//Hides hover info
	public void EndHover(HoverInfo hoverInfo)
	{
		outline.enabled = false;
		hoverInfo.gameObject.SetActive(false);
	}
}
