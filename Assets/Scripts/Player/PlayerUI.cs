using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerUI : MonoBehaviour
{
	public TextMeshProUGUI areaText;
	public TextMeshProUGUI errorText;
	public TextMeshProUGUI tasksText;

	public float errorWaitTimePerChar = 0.15f;
	public float errorWriteTimePerChar = 0.05f;

	public void SetArea(string area)
	{
		areaText.text = area;
	}

	private Coroutine errorCoroutine;

	public void ShowError(string errorMsg)
	{
		if (errorCoroutine != null)
		{
			StopCoroutine(errorCoroutine);
			errorText.text = "";
		}

		errorCoroutine = StartCoroutine(WriteError(errorMsg));
	}

	private IEnumerator WriteError(string errorMsg)
	{

		for (int i = 0; i < errorMsg.Length; i++)
		{
			errorText.text = errorMsg.Substring(0, i + 1);
			yield return new WaitForSeconds(errorWriteTimePerChar);
		}

		errorCoroutine = StartCoroutine(ClearError(errorMsg));
	}

	private IEnumerator ClearError(string errorMsg)
	{
		yield return new WaitForSeconds(errorWaitTimePerChar * errorMsg.Length);
		errorText.text = "";
		errorCoroutine = null;
	}
}