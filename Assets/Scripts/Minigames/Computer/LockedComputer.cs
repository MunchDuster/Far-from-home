using TMPro;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class LockedComputer : Computer
{
	public string password;
	public UnityEvent OnUnlock;
	public TextMeshProUGUI passwordText;

	protected override void PoweredOn()
	{
		OnPowerOnFinish.Invoke();
		OnUpdate += CheckInput;
	}	

	public virtual void Unlock() 
	{
		OnUnlock.Invoke();
	}

	protected override void OnCommandEntered()
	{
		passwordText.text = line;
	}
}