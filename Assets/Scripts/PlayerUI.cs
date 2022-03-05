using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerUI : MonoBehaviour
{
	public TextMeshProUGUI areaText;
	public TextMeshProUGUI errorText;
	public float errorTimePerChar = 0.3f;

	public void SetArea(string area)
	{
		areaText.text = area;
	}

	private int errorId;

	public void ShowError(string errorMsg)
	{
		errorId++;
		errorText.text = errorMsg;
		StartCoroutine(HideError(errorId));
	}
	IEnumerator HideError(int myId)
	{
		yield return new WaitForSeconds(errorTimePerChar * errorText.text.Length);

		if (myId == errorId)
		{
			errorText.text = "";
		}
	}
}