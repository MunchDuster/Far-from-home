using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
	public UnityEvent OnInteract;
	public Transform hoverInfoPoint;
	public string hoverName;
	public string hoverInfo;

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

	public void Interact(Player player)
	{
		if (OnInteract != null) OnInteract();
	}

	public void StartHover(Player player)
	{
		//Show outline
		outline.enabled = true;

		//Show info
		player.hoverInfo.enabled = true;
		player.hoverInfo.SetInfo(hoverName, hoverInfo);
		player.hoverInfo.positionTransform = hoverInfoPoint;
	}

	public void EndHover(Player player)
	{
		//Hide outline
		outline.enabled = false;

		//Hide info
		player.hoverInfo.enabled = false;
	}

}
