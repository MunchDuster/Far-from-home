using TMPro;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class LockedComputer : Computer
{
	public string password;
	public UnityEvent OnUnlock;
	public UnityEvent<bool> OnFailUnlock;
	public TextMeshProUGUI passwordText;


	protected override void PoweredOn()
	{
		OnPowerOnFinish.Invoke();
	}	

	public virtual void Unlock() 
	{
		OnUnlock.Invoke();
	}

	private Coroutine failUnlock;

	protected override void OnCommandEntered()
	{
		if(line == password)
		{
			Unlock();
		}
		else
		{
			if(failUnlock != null) StopCoroutine(failUnlock);
			failUnlock = StartCoroutine(FailUnlock());
		}
	}

	protected virtual IEnumerator FailUnlock()
	{
		OnFailUnlock.Invoke(true);
		yield return new WaitForSeconds(2);
		OnFailUnlock.Invoke(false);
	}

	protected override void OnCharEntered()
	{
		passwordText.text = line;
	}
}