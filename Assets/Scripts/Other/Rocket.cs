using UnityEngine;

public class Rocket : MonoBehaviour
{
	private class ShakePart
	{
		public Transform part;
		public Vector3 localPosition;

		public ShakePart(Transform part)
		{
			this.part = part;
			localPosition = part.localPosition;
		}
	}

	[Header("Takeoff settings")]
	public float acceleration = 1f;
	public float targetSpeed = 10;

	[Space(10)]
	public float shaking = 0.1f;
	public float shakeSpeed = 0.2f;
	public AnimationCurve speedMultiplier = AnimationCurve.Linear(0, 0, 1, 1);

	[Space(10)]
	public float skyboxSpinSpeed = 1;



	[Header("Refs")]
	public Animator animator;
	public Transform[] shakePartTransforms;

	private float speed, y;
	private Vector3 normalPos;
	private bool takingOff = false;
	private ShakePart[] shakeParts;

	// Start is called before the first frame update
	private void Start()
	{
		speed = 0;
		y = transform.position.y;
		normalPos = transform.position;

		//Init shakeParts
		shakeParts = new ShakePart[shakePartTransforms.Length];
		for (int i = 0; i < shakePartTransforms.Length; i++)
		{
			shakeParts[i] = new ShakePart(shakePartTransforms[i]);
		}
	}


	public void Launch()
	{
		animator.SetBool("launching", true);
		takingOff = true;
	}

	// Update is called every frame
	private void Update()
	{
		if (takingOff)
		{
			if (speed < targetSpeed)
			{
				float deltaSpeed = acceleration * Time.deltaTime;

				//Clamp to targetSpeed (incase of low framerate)
				speed = Mathf.Min(speed + deltaSpeed, targetSpeed);
			}

			y += speed * Time.deltaTime;
			normalPos = new Vector3(normalPos.x, y, normalPos.z);

			Vector3 shakedPos = AddShake(normalPos);

			transform.position = shakedPos;

			foreach (ShakePart shakePart in shakeParts)
			{
				shakePart.part.localPosition = AddShake(shakePart.localPosition);
			}

		}
	}

	private Vector3 AddShake(Vector3 position)
	{
		float shakeAmount = shaking * speedMultiplier.Evaluate(speed / targetSpeed) * Time.deltaTime;

		Vector3 offset = Random.onUnitSphere * shakeAmount;

		return position + offset;
	}
}