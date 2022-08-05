using UnityEngine;
using TMPro;

public class HoverInfo : MonoBehaviour
{
	public TextMeshPro hoverNameText;
	public TextMeshPro hoverInfoText;

	public Transform target;
	public Transform positionPoint;

	public void SetInfo(string name, string info)
	{
		hoverNameText.text = name;
		hoverInfoText.text = info;
	}

	//Update is called every frame.
	private void Update()
	{
		if(positionPoint  == null) return;
		transform.position = positionPoint.position;
		transform.LookAt(target);
	}
}