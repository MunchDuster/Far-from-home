using UnityEngine;

public class Explosion : MonoBehaviour
{
	public bool explodeOnStart;

	[Space(10)]
	[Header("Blast settings")]
	public float blastRadius;
	public float blastForce;

	// Start is called before the first frame update
	void Start()
	{
		if (explodeOnStart)
		{
			Explode();
		}
	}

	public void Explode()
	{
		Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);

		foreach (Collider collider in colliders)
		{
			Rigidbody body = collider.GetComponent<Rigidbody>();

			if (body != null) body.AddExplosionForce(blastForce, transform.position, blastRadius);
		}
	}
}
