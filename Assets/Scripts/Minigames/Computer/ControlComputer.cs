using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ControlComputer: LockedComputer
{
	public Slider startupSlider;
	
	protected override IEnumerator PowerUp()
	{
		OnPowerOnStart.Invoke();
		
		float timeSoFar = 0;
		while(timeSoFar < turnOnTime)
		{
			startupSlider.value = timeSoFar / turnOnTime;
			yield return new WaitForEndOfFrame();
		}
		
		PoweredOn();
	}	
}