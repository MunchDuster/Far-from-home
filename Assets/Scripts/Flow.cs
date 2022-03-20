using UnityEngine;
using System.Collections.Generic;

public class Flow : MonoBehaviour
{
	[Header("Point settings")]
	public float lifeTime = 5;
	public float bounceFriction = 0.2f;
	public LayerMask layerMask;
	public float fuelPerPoint = 0.05f;

	private class Point
	{
		public static List<Point> points;

		public static float bounceFriction;
		public static LayerMask layerMask;

		public float timeLeft = 0;

		public Stream stream;
		public Vector3 position;
		public Vector3 velocity;

		public Point(Vector3 position, Vector3 velocity, float lifeTime, Stream stream)
		{
			points.Add(this);
			stream.points.Add(this);

			timeLeft = lifeTime;

			this.stream = stream;
			this.position = position;
			this.velocity = velocity;
		}
		public void UpdatePosition()
		{
			float dt = Time.fixedDeltaTime;
			velocity += dt * Physics.gravity;

			Vector3 nextPoint = position + velocity * dt;

			Vector3 direction = (nextPoint - position).normalized;
			float distance = (nextPoint - position).magnitude;

			if (Physics.Raycast(position, direction, out RaycastHit hit, distance, layerMask, QueryTriggerInteraction.Ignore))
			{
				nextPoint = hit.point;
				velocity = Vector3.Reflect(velocity, hit.normal) * bounceFriction;
			}

			position = nextPoint;
		}
	}

	private class Stream
	{
		public static List<Stream> streams;

		public List<Point> points = new List<Point>();

		public LineRenderer lineRenderer;

		public Stream(LineRenderer lineRenderer)
		{
			streams.Add(this);

			this.lineRenderer = lineRenderer;
		}
		public void UpdateLineRenderer()
		{
			lineRenderer.positionCount = points.Count;

			Vector3[] pointPositions = new Vector3[points.Count];

			for (int i = 0; i < points.Count; i++) pointPositions[i] = points[i].position;

			lineRenderer.SetPositions(pointPositions);
		}
		public void AddPoint(Vector3 position, Vector3 direction, float lifeTime)
		{
			Point point = new Point(position, direction, lifeTime, this);
		}
	}

	//MAIN CLASS CODE
	[Space(10)]
	public float closeness = 0.1f;

	[Space(10)]
	public Transform pourPoint;
	public Transform fuelPoint;
	public Engine engine;
	public Rigidbody rb;
	public LineRenderer baseLineRenderer;

	// Start is called before the first frame update
	private void Start()
	{
		//Init lists
		Init();
	}

	public void Init()
	{
		//Init lists
		Stream.streams = new List<Stream>();
		Point.points = new List<Point>();

		//Set vars
		Point.layerMask = layerMask;
		Point.bounceFriction = bounceFriction;
	}

	bool wasPouring = false;
	bool isPouring = false;
	private void UpdatePouring()
	{
		wasPouring = isPouring;
		isPouring = pourPoint.forward.y < 0;
	}

	// FixedUpdate is called every physics update
	private Stream curStream;
	private void FixedUpdate()
	{
		UpdatePouring();

		//Pour / Create new stream
		if (isPouring)
		{
			if (!wasPouring)
			{
				LineRenderer lineRenderer = CreateLineRenderer();

				Stream newStream = new Stream(lineRenderer);
				curStream = newStream;
			}

			curStream.AddPoint(pourPoint.position, rb.GetPointVelocity(pourPoint.position), lifeTime);
		}
		else
		{
			curStream = null;
		}

		UpdatePointsAlive();
		UpdateStreamsAlive();

		UpdatePointsPosition();
		UpdateStreamsRenderer();
	}

	private void UpdatePointsAlive()
	{
		for (int i = 0; i < Point.points.Count; i++)
		{
			Point.points[i].timeLeft -= Time.fixedDeltaTime;

			if (Point.points[i].timeLeft <= 0)
			{
				DestroyPointByIndex(i);
			}
			else if ((Point.points[i].position - fuelPoint.position).magnitude <= closeness)
			{
				engine.AddFuel(fuelPerPoint);
				DestroyPointByIndex(i);
			}
		}
	}
	private void DestroyPointByIndex(int i)
	{
		Point.points[i].stream.points.Remove(Point.points[i]);
		Point.points.RemoveAt(i);
	}
	private void UpdateStreamsAlive()
	{
		for (int i = 0; i < Stream.streams.Count; i++)
		{
			bool notCurStream = Stream.streams[i] != curStream;
			bool notEnoughPoints = Stream.streams[i].points.Count < 2;

			if (notEnoughPoints && notCurStream)
			{

				Destroy(Stream.streams[i].lineRenderer.gameObject);

				//Destroy last point
				if (Stream.streams[i].points.Count > 0)
				{
					Point point = Stream.streams[i].points[0];
					DestroyPointByIndex(Point.points.IndexOf(point));
				}

				Stream.streams.RemoveAt(i);

				i--;
			}
		}
	}
	private void UpdatePointsPosition()
	{
		for (int i = 0; i < Point.points.Count; i++)
		{
			Point.points[i].UpdatePosition();
		}
	}
	private void UpdateStreamsRenderer()
	{
		for (int i = 0; i < Stream.streams.Count; i++)
		{
			Stream.streams[i].UpdateLineRenderer();
		}
	}

	private LineRenderer CreateLineRenderer()
	{
		GameObject newGameObject = new GameObject("Stream " + Stream.streams.Count, new System.Type[] { typeof(LineRenderer) });

		LineRenderer lineRenderer = newGameObject.GetComponent<LineRenderer>();

		//Copy values from base
		lineRenderer.widthMultiplier = baseLineRenderer.widthMultiplier;
		lineRenderer.material = baseLineRenderer.material;
		lineRenderer.numCornerVertices = baseLineRenderer.numCornerVertices;
		lineRenderer.numCapVertices = baseLineRenderer.numCapVertices;

		return lineRenderer;
	}
}