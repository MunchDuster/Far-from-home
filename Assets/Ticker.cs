using System.Collections;
using UnityEngine;
using TMPro;

public class Ticker : MonoBehaviour
{
	public int startValue = 234;
	public float timeStep = 1f;

	public string beforeText = "";
	public string afterText = "";

	public TMP_Text text;

    void Start()
    {
        StartCoroutine(Tick());
    }
	public void Stop()
	{
		StopAllCoroutines();
	}

	int value;
	IEnumerator Tick()
	{
		value = startValue - 1;
		while (true)
		{
			value++;
			text.text = beforeText + value + afterText;
			yield return new WaitForSeconds(timeStep);
		}
	}
}
