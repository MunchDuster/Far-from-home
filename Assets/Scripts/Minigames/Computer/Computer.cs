using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public abstract class Computer : MonoBehaviour
{
	public float turnOnTime = 2;
	
	public UnityEvent<bool> OnUserEnter;
	public UnityEvent<bool> OnPowerOn;
	public UnityEvent OnPowerOnFinish;

	protected delegate void OnEvent();
	protected event OnEvent OnUpdate;
	
	protected delegate void OnBoolEvent(bool aBool);
	protected OnBoolEvent SetOn;
	
	protected delegate void OnStringEvent(string aString);
	protected OnStringEvent OnKeyPressed;
	
	public virtual void PowerOn(bool on)
	{
		if(on)
		{
			StartCoroutine(PowerUp());
		}
		else
		{
			OnPowerOn.Invoke(false);
			OnUpdate -= CheckInput;
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
		}
		else if(input == '\b')
		{
			ApplyBackspace(ref line);
		}
		else if(input != '\0')
		{
			OnCharEntered();
			line += input;
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
}