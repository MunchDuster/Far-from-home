using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;
using UnityEngine.Events;

public class ErrorBundle: MonoBehaviour
{
	UnityEvent<bool> unityEvent;
	float time;

	public void Setup(UnityEvent<bool> unityEvent, float time)
	{
		this.unityEvent = unityEvent;
		this.time = time;
	}

	IEnumerator ShowError()
	{
		unityEvent.Invoke(true);
		yield return new WaitForSeconds(time);
		unityEvent.Invoke(false);
	}

	private Coroutine coroutine;
	public void Call()
	{
		if(coroutine != null) StopCoroutine(coroutine);
		coroutine = StartCoroutine(ShowError());
	}
}