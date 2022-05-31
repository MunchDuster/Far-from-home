using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class PauseMenu : Menu
{
	public AudioMixer audioMixer;
	public GameObject subtitles;
	public Volume volume;

	private ColorAdjustments colorAdjustments;

	public void SetVolume(float volume)
	{
		audioMixer.SetFloat("volume", volume);
	}

	public void SetSubtitles(bool enabled)
	{
		subtitles.SetActive(enabled);
	}

	public void SetBrightness(float value)
	{
		Color color = Color.LerpUnclamped(Color.black, Color.white, value);
		ColorParameter colorParameter = colorAdjustments.colorFilter;
		colorParameter.value = color;
		colorAdjustments.colorFilter = colorParameter;
	}

	// Start is called before the first frame update
	private void Start()
	{
		volume.profile.TryGet(out colorAdjustments);
		Debug.Log(colorAdjustments);
	}

    // Update is called every frame
	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			ToggleShown();
		}
	}

	public GameObject pausePanel;
	public UnityEvent<bool> OnToggleShown;

	private bool paused = false;

	public void ToggleShown()
	{
		paused = !paused;
		
		if(OnToggleShown != null) OnToggleShown.Invoke(!paused);

		if(paused)
		{
			pausePanel.SetActive(true);
			Time.timeScale = 0;
		}
		else
		{
			pausePanel.SetActive(false);
			Time.timeScale = 1;
		}
	}
}