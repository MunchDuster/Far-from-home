using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class Timer : MonoBehaviour
{
	public float startTimeLeft = 60 * 5;
	public bool running = true;

	public TextMeshProUGUI text;
	public UnityEvent OnRunOut;

	//Main var
	private float timeLeft = 0;

	//Start is called before first update.
	private void Start()
	{
		timeLeft = startTimeLeft;
		text.text = GetTimeLeftText();
	}

	//Update is called every frame.
	private void Update()
	{
		if (!running) return;
		timeLeft -= Time.deltaTime;
		text.text = GetTimeLeftText();
	}

	//Used bin Update to get the timeleft in "m minutes s seconds" notation.
	private string GetTimeLeftText()
	{
		if (timeLeft >= 60)
		{
			int mins = (int)Mathf.Ceil(timeLeft / 60f);
			int secs = (int)Mathf.Ceil(timeLeft % 60);
			return mins + "m " + secs + "s";
		}
		else
		{
			if (timeLeft < 10)
			{
				if (timeLeft <= 0)
				{
					if (OnRunOut != null) OnRunOut.Invoke();
					this.enabled = false;
					return "";
				}
				float secs = timeLeft % 60;
				return secs.ToString("0.0") + "s";
			}
			else
			{
				int secs = Mathf.RoundToInt(timeLeft % 60);
				return secs + "s";
			}
		}
	}

	//Add more time left
	public void AddTime(float time)
	{
		timeLeft += time;
		Debug.Log("Time added: " + time);
	}

	public void Stop()
	{
		running = false;
	}
}