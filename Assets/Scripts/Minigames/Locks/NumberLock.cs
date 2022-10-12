using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using TMPro;

public class NumberLock : Lock
{
	public TMP_Text text;
	public int noOfDigits;
	public string builder;
	public int answer;

	[Space(10)]
	public float flashTick = 0.8f;
	public float flashTime = 3;

	public UnityEvent<bool> OnJoin;

	private int index;

	private int[] digits;

	// Start is called before the first frame update
	protected void Start()
	{
		digits = new int[noOfDigits];
		OnPlayerJoin += (bool joined) => {OnJoin.Invoke(joined); };
	}

	private Coroutine flashingText;

	public void EnterDigit(int digit)
	{
		//Set digit
		digits[index] = digit;

		//Update index to next digit
		index = (index + 1) % noOfDigits;

		//Init text
		UpdateText();
	}

	//Checks whether entered correct number
	public void CheckDigits()
	{
		//convert array of entered digits to string
		string str = "";
		for (int i = 0; i < noOfDigits; i++)
		{
			str += digits[i];
		}
		//Parse string as integer
		int enteredNum = enteredNum = System.Convert.ToInt32(str);

		//Check answer
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
		//Flash given text
		for (float t = 0; t < flashTime; t += flashTick)
		{
			this.text.text = ((t / flashTick) % 2 > 0.5f) ? "" : text;
			yield return new WaitForSeconds(flashTick);
		}

		//Show digits again after flashing
		UpdateText();
	}

	private void UpdateText()
	{
		//Stop flashing if flashing
		if (flashingText != null)
		{
			StopCoroutine(flashingText);
			flashingText = null;
		}

		//Show digits, underline selected digit
		string str = builder;
		for (int i = 0; i < noOfDigits; i++)
		{
			//replace num(i) with replacement string (digit entered), underlines selected digit
			string replacement = (i == index) ? "<u>" + digits[i].ToString() + "</u>" : digits[i].ToString();
			string replacee = "num" + (i + 1).ToString();

			str = str.Replace(replacee, replacement);
		}

		//Set text
		text.text = str;
	}

	protected override InteractionInfo CheckRequirements(Player player)
	{
		return InteractionInfo.Success();
	}
}
