using System.Collections;
using UnityEngine;

public class WeldPlate : Pickupable
{
	[Header("Settings")]
	public float burnGridDensity = 30;
	public float heatDispersionSpeed = 0.1f;
	public float heatDeltaTime = 0.2f;

	[Space(10)]
	public float overHeat = 20;
	public float weldHeat = 15;

	[Space(10)]
	public Vector2Int gridSize = Vector2Int.one * 20;

	[Space(10)]
	public Color32 tooBrightColor = new Color32(255, 255, 255, 255);
	public Color32 brightColor = new Color32(255, 255, 0, 255);
	public Color32 darkColor = new Color32(0, 0, 0, 255);

	[Header("Refs")]
	public Transform normal;
	public new Renderer renderer;

	[HideInInspector] public Plane plane;

	private Texture2D texture;
	private float[,] heatGrid;
	private Coroutine heatUpdateCoroutine;


	public void StartWelding()
	{
		heatUpdateCoroutine = StartCoroutine(HeatUpdate());
	}

	public void StopWelding()
	{
		StopCoroutine(heatUpdateCoroutine);
	}

	public void AddHeat(Vector3 position, float heat)
	{
		Vector2Int index = GetIndex(position);

		Debug.Log("Adding heat to " + index);

		if (index.x < 0 || index.x >= gridSize.x || index.y < 0 || index.y >= gridSize.y) return;

		heatGrid[index.x, index.y] += heat;

		Debug.Log("Now: " + heatGrid[index.x, index.y]);
	}

	private IEnumerator HeatUpdate()
	{
		while (true)
		{
			UpdateHeatMap();
			yield return new WaitForSeconds(heatDeltaTime / 2);

			UpdateTexture();
			yield return new WaitForSeconds(heatDeltaTime / 2);
		}
	}

	//Gets index of cell from position on plane
	private Vector2Int GetIndex(Vector3 position)
	{
		float vertical = gridSize.y / burnGridDensity;
		float horizontal = gridSize.x / burnGridDensity;

		Vector3 top = normal.up * vertical;
		Vector3 bottom = -normal.up * vertical;
		Vector3 right = normal.right * horizontal;
		Vector3 left = -normal.right * horizontal;

		Vector3 horizontalProjection = Vector3.Project(position - normal.position, right - left);
		float x = InverseLerp(left, right, horizontalProjection);


		Vector3 verticalProjection = Vector3.Project(position - normal.position, bottom - top);
		float y = InverseLerp(top, bottom, verticalProjection);

		Debug.DrawRay(normal.position + bottom + left, horizontalProjection, Color.yellow);
		Debug.DrawRay(normal.position + bottom + left, verticalProjection, Color.green);


		Vector2Int index = new Vector2Int(
			(int)(x * ((float)gridSize.x - 1f)),
			(int)(y * ((float)gridSize.y - 1f))
		);
		// Vector3 direction = position - normal.position;

		// Vector3 localDirection = normal.InverseTransformDirection(direction);

		// Vector2 planeDirection = new Vector2(localDirection.x, localDirection.y);

		// Vector2 index = new Vector2(
		// 	(planeDirection.x) * burnGridDensity + (float)gridSize.x / 2,
		// 	(planeDirection.y) * burnGridDensity + (float)gridSize.y / 2
		// );


		return index;
	}

	private float InverseLerp(Vector3 A, Vector3 B, Vector3 C)
	{
		//Assumes all vectors are on a line and that C is between A and B

		float C2B = (B - A).magnitude;
		float C2A = (C - A).magnitude;
		return C2B / C2A;
	}

	//Average out values on heat map, even out heat
	private void UpdateHeatMap()
	{
		for (int i = 1; i < gridSize.x - 1; i++)
		{
			for (int j = 1; j < gridSize.y - 1; j++)
			{
				float[] sides = new float[] {
					heatGrid[i + 0, j + 1], //Up
					heatGrid[i + 0, j - 1], //Down
					heatGrid[i + 1, j + 0], //Left
					heatGrid[i - 1, j + 0], //Right
					heatGrid[i + 1, j + 1], //Up - Left
					heatGrid[i + 1, j - 1], //Up - Right
					heatGrid[i - 1, j + 1], //Down - Left
					heatGrid[i - 1, j - 1]  //Down - Right
				};

				float sum = 0;
				System.Array.ForEach<float>(sides, delegate (float i) { sum += i; });

				float avg = sum / 8f;

				heatGrid[i, j] = Mathf.Lerp(heatGrid[i, j], avg, heatDeltaTime * heatDispersionSpeed);
			}
		}
	}

	//Update the texture to match heat map
	private void UpdateTexture()
	{
		Color32[] pixels = new Color32[gridSize.x * gridSize.y];

		//Convert heatMap to pixel colors
		for (int i = 0; i < gridSize.x; i++)
		{
			for (int j = 0; j < gridSize.y; j++)
			{
				int index = j * gridSize.x + i;

				if (heatGrid[i, j] > overHeat)
				{
					Debug.Log("Bright!");
					float bright2TooBright = (heatGrid[i, j] - weldHeat) / (overHeat - weldHeat);
					pixels[index] = Color32.Lerp(darkColor, brightColor, bright2TooBright);
				}
				else
				{
					float dark2Bright = heatGrid[i, j] / weldHeat;
					pixels[index] = Color32.Lerp(darkColor, brightColor, dark2Bright);
				}
			}
		}

		texture.SetPixels32(pixels);
		texture.Apply();
	}

	// Start is called before the first frame update
	private void Start()
	{
		plane = new Plane(normal.forward, normal.position);

		heatGrid = new float[gridSize.x, gridSize.y];

		texture = new Texture2D(gridSize.x, gridSize.y);

		renderer.material.mainTexture = texture;
	}

	// Update is called every frame
	private void Update()
	{
		plane.SetNormalAndPosition(normal.forward, normal.position);
	}

	// OnDrawGizmosSelect is called every editor update when the gameObject is selected
	private void OnDrawGizmosSelected()
	{
		DrawDebugGrid();
		// DrawDebugHeatGrid();
	}

	// private void DrawDebugHeatGrid()
	// {
	// 	float vertical = gridSize.y / burnGridDensity;
	// 	float horizontal = gridSize.x / burnGridDensity;

	// 	Vector3 top = normal.up * vertical;
	// 	Vector3 bottom = -normal.up * vertical;
	// 	Vector3 right = normal.right * horizontal;
	// 	Vector3 left = -normal.right * horizontal;

	// 	for (int i = 0; i < gridSize.x; i++)
	// 	{
	// 		for (int j = 0; j < gridSize.y; j++)
	// 		{
	// 			Gizmos.DrawSphere(point, 0.05f, texture.GetPixels());
	// 		}
	// 	}
	// }
	private void DrawDebugGrid()
	{
		float vertical = gridSize.y / burnGridDensity;
		float horizontal = gridSize.x / burnGridDensity;

		Vector3 top = normal.up * vertical;
		Vector3 bottom = -normal.up * vertical;
		Vector3 right = normal.right * horizontal;
		Vector3 left = -normal.right * horizontal;

		for (int i = 0; i <= gridSize.x; i++)
		{
			float lerp = (float)i / (float)gridSize.x;
			Vector3 topLerp = normal.position + Vector3.Lerp(top + left, top + right, lerp);
			Vector3 bottomLerp = normal.position + Vector3.Lerp(bottom + left, bottom + right, lerp);
			Debug.DrawLine(topLerp, bottomLerp, Color.red);
		}
		for (int j = 0; j <= gridSize.y; j++)
		{
			float lerp = (float)j / (float)gridSize.y;
			Vector3 leftLerp = normal.position + Vector3.Lerp(left + top, left + bottom, lerp);
			Vector3 rightLerp = normal.position + Vector3.Lerp(right + top, right + bottom, lerp);
			Debug.DrawLine(leftLerp, rightLerp, Color.blue);
		}
	}
}
