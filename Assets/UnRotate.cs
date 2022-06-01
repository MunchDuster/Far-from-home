using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnRotate : MonoBehaviour
{
    public Transform matchRotation;

	public Transform[] matchers;

    // Update is called once per frame
    void LateUpdate()
    {
        float rotation = matchRotation.rotation.eulerAngles.y;

		for(int i = 0; i < matchers.Length; i++)
		{
			matchers[i].localRotation = Quaternion.Euler(0, 0, -rotation);
		}
    }
}
