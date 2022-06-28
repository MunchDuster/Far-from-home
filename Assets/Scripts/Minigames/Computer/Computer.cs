using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public abstract class Computer : MonoBehaviour
{
	public UnityEvent<bool> OnUserEnter;
	public UnityEvent<bool> OnPowerOn;
	
	protected OnBoolEvent SetOn;
	protected delegate void OnBoolEvent(bool aBool);
	
	protected delegate void OnStringEvent(string aString);
	protected OnStringEvent OnKeyPressed;
	
	public abstract void PowerOn(bool turnOn);
	
	public string GetKeyInput()
	{
		string input = "";
		Event e = Event.current;
		if (e.isKey && e.type == EventType.KeyDown)
		{
			if(e.keyCode == KeyCode.None)
			{
				return (e.shift) ? e.character.ToString().ToUpper() : e.character.ToString();
			}
			else if(e.keyCode - KeyCode.Backspace)
			{
				return '\b';
			}
		}
	}
}