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
			OnPoweredOn.Invoke(false);
		}
	}

	protected override IEnumerator PowerUp()
	{
		//do stuff
		PoweredOn();
	}

	protected override void PoweredOn()
	{
		//Show lock screen
		OnPoweredOn.Invoke(true);


	}

	protected virtual void OnUnlocked() {}
}