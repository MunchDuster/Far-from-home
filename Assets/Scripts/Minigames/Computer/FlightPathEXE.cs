using UnityEngine;
using UnityEngine.Events;

public class FlightPathEXE : ComputerApp
{
	public UnityEvent OnCalculateFlightPath;
	
	public void CalculateFlightPath()
	{
		OnCalculateFlightPath.Invoke();
	}
}