using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;
using UnityEngine.Events;

public class CoroutineBundle: MonoBehaviour
{
	private Coroutine coroutine;
	public delegate IEnumerator Numerator();
	protected Numerator numerator;

	public CoroutineBundle(Numerator numerator)
	{
		this.numerator = numerator;
	}
	public CoroutineBundle() {}
	public void Call()
	{
		if(coroutine != null) StopCoroutine(coroutine);
		coroutine = StartCoroutine(numerator());
	}
}

public class ErrorBundle: CoroutineBundle
{
	UnityEvent<bool> unityEvent;
	float time;

	public ErrorBundle(UnityEvent<bool> unityEvent, float time)
	{
		this.unityEvent = unityEvent;
		this.time = time;
		numerator = ShowError;
	}

	IEnumerator ShowError()
	{
		unityEvent.Invoke(true);
		yield return new WaitForSeconds(time);
		unityEvent.Invoke(false);
	}
}