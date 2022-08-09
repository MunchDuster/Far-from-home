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

	public UnityEvent<bool> CantUploadError;

	public Texture2D customCursorTexture;
	public Vector2 customCursorOffset;

	public UnityEvent<bool> OnDataEnterError;

	protected ErrorBundle cantUploadBundle;
	protected ErrorBundle dataInputErrorBundle;

	public string targetCoordinates;
	public string rocketModel;
	public string rocketMass;

	public bool enteredTC, enteredRMa, enteredRMo;

	// Awake is called when the gameObject is activated
	protected override void Awake()
	{
		cantUploadBundle = gameObject.AddComponent<ErrorBundle>();
		cantUploadBundle.Setup(CantUploadError, 3);
		dataInputErrorBundle = gameObject.AddComponent<ErrorBundle>();
		dataInputErrorBundle.Setup(OnDataEnterError, 3);

		base.Awake();
	}

	public void UseCustomCursor(bool useCustomCursor)
	{
		if(useCustomCursor)
			Cursor.SetCursor(customCursorTexture, customCursorOffset, CursorMode.Auto);
		else
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
	}
	public GameObject calculatingPanel;
	public void CalculateFlightPath()
	{
		if(!enteredTC || !enteredRMo || !enteredRMa)
		{
			dataInputErrorBundle.Call();
			return;
		}

		calculatingPanel.SetActive(true);
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

	public void OnEnterTargetCoordinates(string text)
	{
		if(text != targetCoordinates)
		{
			dataInputErrorBundle.Call();
		}
		else
		{
			enteredTC = true;
		}
	}

	public void OnEnterRocketModel(string text)
	{
		if(text != rocketModel)
		{
			dataInputErrorBundle.Call();
		}
		else
		{
			enteredRMo = true;
		}
	}

	public void OnEterRocketMass(string text)
	{
		if(text != rocketMass)
		{
			dataInputErrorBundle.Call();
		}
		else
		{
			enteredRMa = true;
		}
	}

	public void UploadFlightPath()
	{

		if(!hasCalculatedFlightPath)
		{
			cantUploadBundle.Call();
			return;
		}
		
		calculatingPanel.SetActive(true);
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