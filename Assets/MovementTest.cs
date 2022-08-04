using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTest : MonoBehaviour
{
    public Transform target;
    public Rigidbody rb;

    public float lerp = 1;
    public float damping = 1;

	public float rotLerp = 1;
	public float rotDamp = 1;

    void FixedUpdate()
    {
        Vector3 force = (target.position - rb.position) * lerp - rb.velocity * damping;
        rb.AddForce(force, ForceMode.VelocityChange);
		
		Vector3 turnOffset = GetTorqueAngle();
		curveX.AddKey(Time.timeSinceLevelLoad / 5f, turnOffset.x);
		curveY.AddKey(Time.timeSinceLevelLoad / 5f, turnOffset.y);
		curveZ.AddKey(Time.timeSinceLevelLoad / 5f, turnOffset.z);
		Vector3 torque = turnOffset * rotLerp - rb.angularVelocity * rotDamp;
		rb.AddTorque(torque * rotLerp);
    }
	public AnimationCurve curveX;
	public AnimationCurve curveY;
	public AnimationCurve curveZ;

	private Vector3 GetTorqueAngle()
    {
		//Quaternion offset = Quaternion.FromToRotation(rb.transform.forward, target.forward);

		float t0 = Time.realtimeSinceStartup;
		float x = 0, y = 0, z = 0;
		

			//12.45s
			// x = Vector3.SignedAngle(rb.transform.up, target.up, Vector3.right);
			// y = Vector3.SignedAngle(rb.transform.forward, target.forward, Vector3.up);
			// z = Vector3.SignedAngle(rb.transform.right, target.right, Vector3.forward);
			//04.90s
			Vector3 deltaEulers = Quaternion.FromToRotation(rb.transform.forward, target.forward).eulerAngles;
			x = deltaEulers.x > 180f ? deltaEulers.x - 360f : deltaEulers.x;
            y = deltaEulers.y > 180f ? deltaEulers.y - 360f : deltaEulers.y;
            z = deltaEulers.z > 180f ? deltaEulers.z - 360f : deltaEulers.z;
		
		float t1 = Time.realtimeSinceStartup;

		Debug.Log("Takes " + (t1 - t0) + " seconds");
 
        return new Vector3(x, y, z);
    }

    // OnDrawGizmos is called every editor update
    private void OnDrawGizmos()
    {
		Gizmos.color = Color.red;
		Gizmos.DrawRay(rb.position, rb.transform.forward);
		Gizmos.color = Color.green;
		Gizmos.DrawRay(target.position, target.forward);
		Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(target.position, 0.1f);
    }
}
