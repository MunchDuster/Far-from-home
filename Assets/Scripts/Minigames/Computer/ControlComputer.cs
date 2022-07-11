using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class ControlComputer: LockedComputer
{
	public UnityEvent OnCalculateFlightPath;
	public UnityEvent OnUploadFlightPath;

	public TextMeshProUGUI calcText;
	public Slider loadSlider;

	public bool hasCalculatedFlightPath = false;

	protected ErrorBundle cantUploadBundle;
	public UnityEvent<bool> CantUploadError;

	// Awake is called when the gameObject is activated
	protected override void Awake()
	{
		cantUploadBundle = new ErrorBundle(CantUploadError, 3);
		base.Awake();
	}


	public void CalculateFlightPath()
	{
		float loadTime = 3;
		StartCoroutine(LoadBar(loadSlider, loadTime));
		StartCoroutine(LoadText(
			"Calculating flight path",
			loadTime,
			(string text) => {calcText.text = text;},
			() => {
				OnCalculateFlightPath.Invoke(); 
				hasCalculatedFlightPath = true;
			}
		));
	}

	public void UploadFlightPath()
	{

		if(!hasCalculatedFlightPath)
		{
			cantUploadBundle.Call();
			return;
		}

		
		float loadTime = 3;
		StartCoroutine(LoadBar(loadSlider, loadTime));
		StartCoroutine(LoadText(
			"Uploading flight path",
			loadTime,
			(string text) => {calcText.text = text;},
			() => {
				OnUploadFlightPath.Invoke(); 
			}
		));
	}

	private IEnumerator LoadBar(Slider slider, float loadTime)
	{
		float curTime = 0;
		while(curTime < loadTime)
		{
			slider.value = curTime / loadTime;
			yield return new WaitForEndOfFrame();
			curTime += Time.deltaTime;
		}
	}
}