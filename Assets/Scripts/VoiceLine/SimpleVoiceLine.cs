using UnityEngine;
using System.Collections;

public class SimpleVoiceLine : VoiceLine
{
	public AudioClip clip;
	public string clipText;
	public string talker;
	public float delay;
	public override void Play()
	{
		StartCoroutine(Delay());
	}

	private IEnumerator Delay()
	{
		yield return new WaitForSeconds(delay);
		source.PlayOneShot(clip);
		PlayerUI.ui.AddSubtitle(clipText, talker);
		yield return new WaitForSeconds(clip.length);
		OnPlayed.Invoke();
	}
}