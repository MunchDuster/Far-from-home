using UnityEngine;
using System.Collections;

public class LockedComputer : Computer
{
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
		yield return new WaitForSeconds(1);
		//do stuff
		PoweredOn();
	}

	protected override void PoweredOn()
	{
		//Show lock screen
		OnPowerOn.Invoke(true);
	}

	protected virtual void OnUnlocked() {}
}