using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorTest : MonoBehaviour
{
	public Transform probe;
	public Transform probePlane;
	public Transform player;

	Plane plane;

	// Start is called before the first frame update
	private void Start()
	{
		Vector3 normal = probePlane.up;
		Vector3 point = probePlane.position;
		plane = new Plane(normal, point);
	}
    // Update is called once per frame
    void Update()
    {
        probe.position = plane.ClosestPointOnPlane(player.position);
    }
}
