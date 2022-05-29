using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class GameSettings : MonoBehaviour
{
    public static GameSettings settings;

	public TMP_Dropdown resolutionDropDown;

	Resolution[] resolutions;	

	public void SetQuality (int qualityIndex)
	{
		QualitySettings.SetQualityLevel(qualityIndex);
	}

	public void SetFullscreen (bool fullscreen)
	{
		Screen.fullScreen = fullscreen;
	}

	public void SetResolution (int resolutionIndex)
	{
		Resolution resolution = resolutions[resolutionIndex];
		Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
	}

	// Start is called before the first frame update
	private void Start()
	{
		SetupSingleton();
		SetupResolutionDropdown();
	}

	private void SetupSingleton()
	{
		if(settings == null)
		{
			settings = this;
		}
		else
		{
			Debug.LogWarning("Multiple game settins detected, there should be only one.");
		}
	}

	private void SetupResolutionDropdown()
	{
		resolutions = Screen.resolutions;
		resolutionDropDown.ClearOptions();

		List<string> options = new List<string>();

		int currentResolutionIndex = 0;

		for(int i = 0; i < resolutions.Length; i++)
		{
			string option = resolutions[i].width + " x " + resolutions[i].height;
			options.Add(option);

			if(resolutions[i].height == Screen.currentResolution.height && resolutions[i].width == Screen.currentResolution.width)
			{
				currentResolutionIndex = i;
			}
		}
		resolutionDropDown.AddOptions(options);
		resolutionDropDown.value = currentResolutionIndex;
		resolutionDropDown.RefreshShownValue();
	}
}