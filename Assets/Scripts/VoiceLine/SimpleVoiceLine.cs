using UnityEngine;

public class SimpleVoiceLine : VoiceLine
{
	public AudioClip clip;
	public override void Play()
	{
		source.PlayOneShot(clip);
	}
}