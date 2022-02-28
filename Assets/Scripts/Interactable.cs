using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
	public UnityEvent<Player player> OnInteract;
	public Transform hoverInfoPoint;
	public string hoverName;
	public string hoverInfoText;

	private Outline outline;

	// Start is called before the first frame update
	private void Start()
	{
		CreateOutline();
	}

	private void CreateOutline()
	{
		outline = AddComponent<Outline>();
	}

	public InteractionInfo Interact(Player player)
	{
		if (OnInteract != null) return OnInteract.Invoke(player);
		return InteractionInfo.Success();
	}

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
