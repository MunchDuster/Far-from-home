using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public abstract class VoiceLine : MonoBehaviour
{
	public UnityEvent OnPlayed;
	public abstract void Play();

	protected AudioSource source;

	// Start is called before the first frame update
	protected void Start()
	{
		source = GetComponent<AudioSource>();
	}
}