using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
	public TextMeshProUGUI areaText;
	
	public void SetArea(string area)
	{
		areaText.text = area;
	}
}