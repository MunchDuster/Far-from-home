using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPosition : MonoBehaviour
{
    public Transform follow;

	public Vector3 positionMatch = Vector3.one;

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = new Vector3(
			Mathf.Lerp(transform.position.x, follow.position.x, positionMatch.x),
			Mathf.Lerp(transform.position.y, follow.position.y, positionMatch.y),
			Mathf.Lerp(transform.position.z, follow.position.z, positionMatch.z)
		);
    }
}
