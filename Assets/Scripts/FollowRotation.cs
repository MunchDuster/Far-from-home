using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowRotation : MonoBehaviour
{
    public Transform follow;

    // Update is called once per frame
    void LateUpdate()
    {
		Vector3 currentEulers = transform.rotation.eulerAngles;
		Vector3 targetEulers = follow.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(
			0,0,
			-targetEulers.y
		);
    }
}
