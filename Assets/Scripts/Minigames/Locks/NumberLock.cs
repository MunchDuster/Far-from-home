using UnityEngine;
using System.Collections;
using TMPro;

public class NumberLock : Lock
{
	public TextMeshProUGUI text;
	public int noOfDigits;
	public string builder;
	public int answer;

	[Space(10)]
	public float flashTick = 0.8f;
	public float flashTime = 3;

	private int index;

	private int[] digits;

	// Start is called before the first frame update
	protected override void Start()
	{
		base.Start();

		digits = new int[noOfDigits];
	}

	protected override void StartPlaying() { }
	protected override void StopPlaying() { }
	protected override void GameUpdate() { }

	private Coroutine flashingText;

	public void EnterDigit(int digit)
	{
		digits[index] = digit;
		index = (index + 1) % noOfDigits;

		UpdateText();
	}

	//Checks whether entered correct number
	public void CheckDigits()
	{
		int enteredNum = 0;
		string str = "";
		for (int i = 0; i < noOfDigits; i++)
		{
			str += digits[i];
		}
		enteredNum = System.Convert.ToInt32(str);
		Debug.Log("eneteered num: " + enteredNum);

		if (enteredNum == answer)
		{
			if (OnUnlock != null) OnUnlock.Invoke();
			flashingText = StartCoroutine(FlashText("<color=\"green\">Correct</color>"));
		}
		else
		{
			if (OnFail != null) OnFail.Invoke();
			flashingText = StartCoroutine(FlashText("<color=\"red\">Incorrect</color>"));
		}
	}

	private IEnumerator FlashText(string text)
	{
		for (float t = 0; t < flashTime; t += flashTick)
		{
			this.text.text = ((t / flashTick) % 2 > 0.5f) ? "" : text;
			yield return new WaitForSeconds(flashTick);
		}
		Debug.Log("Finished flashing.");
		this.text.text = text;
	}

	private void UpdateText()
	{
		if (flashingText != null)
		{
			StopCoroutine(flashingText);
			flashingText = null;
		}

		string str = builder;
		for (int i = 0; i < noOfDigits; i++)
		{
			//replace num(i) with replacement string (digit entered), underlines selected digit
			string replacement = (i == index) ? "<u>" + digits[i].ToString() + "</u>" : digits[i].ToString();
			string replacee = "num" + (i + 1).ToString();

			str = str.Replace(replacee, replacement);
		}
		text.text = str;
	}
}
