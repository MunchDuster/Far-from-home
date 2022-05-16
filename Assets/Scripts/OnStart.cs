using UnityEngine;
using UnityEngine.Events;
using System.Collections;
public class OnStart : MonoBehaviour
{
	public float delay;
	public UnityEvent onStart;
    
	// Start is called before the first frame update
	void Start()
    {
		StartCoroutine(Delay());
	}
	
	private IEnumerator Delay()
	{
		yield return new WaitForSeconds(delay);
		onStart.Invoke();
	}
}
