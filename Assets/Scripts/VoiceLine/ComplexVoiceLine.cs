using UnityEngine;
using System.Collections;

public class ComplexVoiceLine : VoiceLine
{
	[System.Serializable]
	public class LinePart
	{
		public AudioClip clip;
		public AudioSource source;
		public float timeIn;
	}

	public LinePart[] parts;

	public override void Play()
	{
		StartCoroutine(PlayClips());
	}

	private IEnumerator PlayClips()
	{
		for (int i = 0; i < parts.Length - 1; i++)
		{
			PlayPart(i);
			yield return new WaitForSeconds(parts[i].timeIn - parts[i + 1].timeIn);
		}

		int last = parts.Length - 1;
		PlayPart(last);
	}

	private void PlayPart(int index)
	{
		parts[index].source.PlayOneShot(parts[index].clip);
	}
}