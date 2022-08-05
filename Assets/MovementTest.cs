using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTest : MonoBehaviour
{
    public Rigidbody rb;

    public float lerp = 1;
    public float damping = 1;

	public float rotLerp = 1;
	public float rotDamp = 1;

    void FixedUpdate()
    {
        Vector3 force = (transform.position - rb.position) * lerp - rb.velocity * damping;
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
		Vector3 deltaEulers = Quaternion.FromToRotation(rb.transform.forward, transform.forward).eulerAngles;
		float x = deltaEulers.x > 180f ? deltaEulers.x - 360f : deltaEulers.x;
        float y = deltaEulers.y > 180f ? deltaEulers.y - 360f : deltaEulers.y;
        float z = deltaEulers.z > 180f ? deltaEulers.z - 360f : deltaEulers.z;
 
        return new Vector3(x, y, z);
    }

    // OnDrawGizmos is called every editor update
    private void OnDrawGizmos()
    {
		Gizmos.color = Color.red;
		Gizmos.DrawRay(rb.position, rb.transform.forward);
		Gizmos.color = Color.green;
		Gizmos.DrawRay(transform.position, transform.forward);
		Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, 0.1f);
    }
}
