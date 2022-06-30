using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public abstract class Computer : MonoBehaviour
{
	public float turnOnTime = 2;
	
	public UnityEvent<bool> OnUserEnter;
	public UnityEvent<bool> OnPowerOn;
	public UnityEvent OnPowerOnStart;

	protected delegate void OnEvent();
	
	protected delegate void OnBoolEvent(bool aBool);
	protected OnBoolEvent SetOn;
	
	protected delegate void OnStringEvent(string aString);
	protected OnStringEvent OnKeyPressed;
	
	public abstract void PowerOn(bool turnOn);

	protected abstract void PoweredOn();
	protected abstract IEnumerator PowerUp();
	
	protected char GetKeyInput()
	{
		Event e = Event.current;
		if (e.isKey && e.type == EventType.KeyDown)
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

	protected void ApplyBackspace(ref string text)
	{
		if(text == null || text.Length == 0) return;
		text = text.Substring(0, text.Length - 1);
	}
}