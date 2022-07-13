using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public abstract class Computer : MonoBehaviour
{
	public float turnOnTime = 2;
	public float maxInputLength = 15;
	
	public UnityEvent<bool> OnUserEnter;
	public UnityEvent<bool> OnPowerOn;
	public UnityEvent OnPowerOnFinish;
	public UnityEvent<bool> OnTooManyChars;

	protected delegate void OnEvent();
	protected event OnEvent OnUpdate;
	protected event OnEvent OnGUIUpdate;
	
	protected delegate void OnBoolEvent(bool aBool);
	protected OnBoolEvent SetOn;
	
	protected delegate void OnStringEvent(string aString);
	protected OnStringEvent OnKeyPressed;

	protected ErrorBundle tooManyCharsBundle;
	
	// Awake is called when the gameObject is activated
	protected virtual void Awake()
	{
		tooManyCharsBundle = gameObject.AddComponent<ErrorBundle>();
		tooManyCharsBundle.Setup(OnTooManyChars, 3);
	}
	
	public virtual void PowerOn(bool on)
	{
		if(on)
		{
			StartCoroutine(PowerUp());
			OnGUIUpdate += CheckInput;
		}
		else
		{
			OnPowerOn.Invoke(false);
			OnGUIUpdate -= CheckInput;
		}
	}

	protected abstract void PoweredOn();
	
	//Input
	protected char GetKeyInput()
	{
		Event e = Event.current;

		if (e != null && e.isKey && e.type == EventType.KeyDown)
		{
			if(e.keyCode == KeyCode.None)
			{
				return e.character;
			}
			else if(e.keyCode == KeyCode.Backspace)
			{
				return '\b';
			}
		}
		return '\0';
	}

	protected string line = "";
	
	protected void CheckInput()
	{
		char input = GetKeyInput();

		if(input == '\n')
		{
			OnCommandEntered();
			line = "";
			OnCharEntered();
		}
		else if(input == '\b')
		{
			ApplyBackspace(ref line);
			OnCharEntered();
		}
		else if(input != '\0')
		{
			if(line.Length < maxInputLength) line += input;
			else tooManyCharsBundle.Call();
			OnCharEntered();
		}
	}

	protected virtual void OnCommandEntered() {}
	protected virtual void OnCharEntered() {}

	protected void ApplyBackspace(ref string text)
	{
		if(text == null || text.Length == 0) return;
		text = text.Substring(0, text.Length - 1);
	}

	private void Update()
	{
		if(OnUpdate != null) OnUpdate.Invoke();
	}

	private void OnGUI()
	{
		if(OnGUIUpdate != null) OnGUIUpdate.Invoke();
	}

	//Booting
	public Slider startupSlider;
	
	protected virtual IEnumerator PowerUp()
	{
		OnPowerOn.Invoke(true);
		
		float timeSoFar = 0;
		while(timeSoFar < turnOnTime)
		{
			startupSlider.value = timeSoFar / turnOnTime;
			yield return new WaitForEndOfFrame();
			timeSoFar += Time.deltaTime;
		}
		
		OnPowerOnFinish.Invoke();
		PoweredOn();
	}

	//Loading
	private float dotsDelta = 0.25f;
	protected delegate void OnSetText(string text);
	protected IEnumerator LoadText(string text, float time, OnSetText setter, OnEvent callback = null)
	{
		int noOfDots = -1;

		for (float t = 0; t < time; t += dotsDelta)
		{
			//Loop from 0 to 3 dots
			noOfDots = ++noOfDots % 4;

			//Put that many dots onto string
			string dots = "";
			for (int i = 0; i < noOfDots; i++) dots += ".";

			setter(text + dots);

			yield return new WaitForSeconds(dotsDelta);
		}

		if(callback != null) callback();
	}
}