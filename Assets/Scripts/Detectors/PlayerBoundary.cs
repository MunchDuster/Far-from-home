using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;


public class PlayerBoundary : MonoBehaviour
{
	public void SetActive(bool active)
	{
		_active = true;
	}
	private bool _active = false;

	public float countdownTime;
	public TextMeshProUGUI countdownText;
	public UnityEvent OnPlayerLeaveBoundary;
	public UnityEvent OnPlayerEnterBoundary;
	public UnityEvent OnPlayerDieOutsideBoundary;

	// Start is called before the first frame update
	private void Start()
	{
		countdown = Countdown();
	}

    private void OnTriggerEnter(Collider collider)
	{
		if(!_active || collider.gameObject.tag != "Player") return;


		Player player = collider.GetComponentInParent<Player>();

		if(player != null)
		{
			StopCoroutine(countdown);
			OnPlayerEnterBoundary.Invoke();
		}
	}
	private void OnTriggerExit(Collider collider)
	{
		if(!_active || collider.gameObject.tag != "Player") return;


		Player player = collider.GetComponentInParent<Player>();

		if(player != null)
		{
			OnPlayerLeaveBoundary.Invoke();
			 timeLeft = countdownTime;

			StartCoroutine(countdown);
		}
	}

	private IEnumerator  countdown;
	private float timeLeft;

	private IEnumerator Countdown()
	{
		while(timeLeft > 0)
		{
			timeLeft -= 0.1f;
			yield return new WaitForSeconds(0.1f);
			countdownText.text = timeLeft.ToString("0.0");
		}

		OnPlayerDieOutsideBoundary.Invoke();
	}
}
