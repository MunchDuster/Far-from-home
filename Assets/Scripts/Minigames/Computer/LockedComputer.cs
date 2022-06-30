using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class LockedComputer : Computer
{
	public UnityEvent OnUnlock;
	
	public override void PowerOn(bool on)
	{
		if(on)
		{
			StartCoroutine(PowerUp());
		}
		else
		{
			OnPowerOn.Invoke(false);
		}
	}

	protected override IEnumerator PowerUp()
	{
		OnPowerOnStart.Invoke();
		yield return new WaitForSeconds(turnOnTime);
		PoweredOn();
	}

	protected override void PoweredOn()
	{
		OnPowerOn.Invoke(true);
	}

	public virtual void Unlock() 
	{
		OnUnlock.Invoke();
	}
}