using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;


public class PlayerBoundary : MonoBehaviour
{
	public float countdownTime;
	public TextMeshProUGUI countdownText;
	public UnityEvent OnPlayerLeaveBoundary;
	public UnityEvent OnPlayerEnterBoundary;
	public UnityEvent OnPlayerDieOutsideBoundary;

    private void OnTriggerEnter(Collider collider)
	{
		Player player = collider.GetComponentInParent<Player>();

		if(player != null)
		{
			if(countdown != null) StopCoroutine(countdown);
			OnPlayerEnterBoundary.Invoke();
		}
	}
	private void OnTriggerLeave(Collider collider)
	{
		Player player = collider.GetComponentInParent<Player>();

		if(player != null)
		{
			countdown = StartCoroutine(Countdown());
			OnPlayerLeaveBoundary.Invoke();
		}
	}

	private Coroutine countdown;

	private IEnumerator Countdown()
	{
		float timeLeft = countdownTime;

		while(timeLeft > 0)
		{
			timeLeft -= 0.1f;
			yield return new WaitForSeconds(0.1f);
			countdownText.text = timeLeft.ToString("0.0");
		}

		OnPlayerDieOutsideBoundary.Invoke();
	}
}
