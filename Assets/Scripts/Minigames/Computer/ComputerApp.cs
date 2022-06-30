using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public abstract class ComputerApp : MonoBehaviour
{	
	public UnityEvent<bool> OnOpen;
	
	public void Open(bool open)
	{
		if(OnOpen != null) OnOpen.Invoke(open);
	}
}