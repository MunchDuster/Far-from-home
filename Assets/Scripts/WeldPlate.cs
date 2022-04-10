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

		heatGrid[index.x, index.y] += heat;
	}

	private IEnumerator HeatUpdate()
	{
		while (true)
		{
			Debug.Log("HeatUpdate");

			UpdateHeatMap();
			yield return new WaitForSeconds(heatDeltaTime / 2);

			UpdateTexture();
			yield return new WaitForSeconds(heatDeltaTime / 2);
		}
	}

	//Gets index of cell from position on plane
	private Vector2Int GetIndex(Vector3 position)
	{
		Vector3 direction = position - normal.position;
		Vector3 projectedDirection = Vector3.ProjectOnPlane(direction, normal.forward);
		Vector2 planeDirection = new Vector2(projectedDirection.x, projectedDirection.y);

		Vector2Int indexFromCenter = new Vector2Int(
			(int)(planeDirection.x / burnGridDensity),
			(int)(planeDirection.y / burnGridDensity)
		);

		Vector2Int indexFromTopLeft = new Vector2Int(
			indexFromCenter.x - gridSize.x / 2,
			indexFromCenter.y - gridSize.y / 2
		);

		Vector2Int boundedIndex = new Vector2Int(
			Mathf.Clamp(indexFromTopLeft.x, 0, gridSize.x),
			Mathf.Clamp(indexFromTopLeft.y, 0, gridSize.y)
		);

		return boundedIndex;
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
