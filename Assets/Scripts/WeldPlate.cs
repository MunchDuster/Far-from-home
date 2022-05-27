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

	private Vector3 topLeft, topRight, bottomLeft, bottomRight;
	Vector3 top, right;

	[HideInInspector] public Color32[] baseColors;

	public void StartWelding()
	{
		heatUpdateCoroutine = StartCoroutine(HeatUpdate());
		UpdateCorners();
	}

	public void StopWelding()
	{
		StopCoroutine(heatUpdateCoroutine);
	}

	public void AddHeat(Vector3 position, float heat)
	{
		Vector2Int index = GetIndex(position);

		if (index.x < 0 || index.x >= gridSize.x || index.y < 0 || index.y >= gridSize.y) return;

		heatGrid[index.x, index.y] += heat;
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
		//Make the vector in line with topLeft and topRight
		Vector3 horizontalProjection = Vector3.Project((position - topLeft), (topLeft - topRight).normalized);
		float x = InverseLerp(topLeft, topRight, topLeft + horizontalProjection);

		//Make the vector in line with topLeft and bottomLeft
		Vector3 verticalProjection = Vector3.Project((position - topLeft), (bottomLeft - topLeft).normalized);
		float y = InverseLerp(topLeft, bottomLeft, topLeft + verticalProjection);

		Debug.DrawRay(topLeft + horizontalProjection, normal.forward, Color.grey);
		Debug.DrawRay(topLeft + verticalProjection, normal.forward, Color.cyan);

		//Mirror
		x = 1 - x;
		y = 1 - y;

		Vector2Int index = new Vector2Int(
			(int)(x * ((float)gridSize.x - 1f)),
			(int)(y * ((float)gridSize.y - 1f))
		);

		return index;
	}

	public float GetHeatAt(Vector2Int index)
	{
		return heatGrid[index.x, index.y];
	}
	//Assumes all points are on a line
	private float InverseLerp(Vector3 A, Vector3 B, Vector3 C)
	{
		float A2B = (B - A).magnitude;
		float C2A = (C - A).magnitude;
		return C2A / A2B;
	}

	//Average out values on heat map, even out heat
	private void UpdateHeatMap()
	{
		float[,] lastHeats = heatGrid.Clone() as float[,];

		//Center grid
		for (int i = 1; i < gridSize.x - 1; i++)
		{
			for (int j = 1; j < gridSize.y - 1; j++)
			{
				float[] sides = new float[] {
					lastHeats[i + 0, j + 1], //Up
					lastHeats[i + 0, j - 1], //Down
					lastHeats[i + 1, j + 0], //Left
					lastHeats[i - 1, j + 0], //Right
					lastHeats[i + 1, j + 1], //Up - Left
					lastHeats[i + 1, j - 1], //Down - Left
					lastHeats[i - 1, j + 1], //Up - Right
					lastHeats[i - 1, j - 1]  //Down - Right
				};

				float sum = 0;
				System.Array.ForEach<float>(sides, delegate (float i) { sum += i; });

				float avg = sum / 8f;

				heatGrid[i, j] = Mathf.Lerp(lastHeats[i, j], avg, heatDeltaTime * heatDispersionSpeed);
			}
		}

		//Bottom (y = maxY, no down)
		for (int i = 1; i < gridSize.x - 1; i++)
			{
				int j = gridSize.y - 1;
				
				float[] sides = new float[] {
					lastHeats[i + 0, j + 1], //Up
					lastHeats[i + 1, j + 0], //Left
					lastHeats[i - 1, j + 0], //Right
					lastHeats[i + 1, j + 1], //Up - Left
					lastHeats[i - 1, j + 1], //Up - Right
				};

				float sum = 0;
				System.Array.ForEach<float>(sides, delegate (float i) { sum += i; });

				float avg = sum / 5;

				heatGrid[i, j] = Mathf.Lerp(lastHeats[i, j], avg, heatDeltaTime * heatDispersionSpeed);
			}

		//Top (y = 0, no up)
		for (int i = 1; i < gridSize.x - 1; i++)
			{
				int j = 0;
				
				float[] sides = new float[] {
					lastHeats[i + 0, j - 1], //Down
					lastHeats[i + 1, j + 0], //Left
					lastHeats[i - 1, j + 0], //Right
					lastHeats[i + 1, j - 1], //Down - Left
					lastHeats[i - 1, j - 1]  //Down - Right
				};

				float sum = 0;
				System.Array.ForEach<float>(sides, delegate (float i) { sum += i; });

				float avg = sum / 5;

				heatGrid[i, j] = Mathf.Lerp(lastHeats[i, j], avg, heatDeltaTime * heatDispersionSpeed);
			}

		//Right (x = 0, no right)
		for (int j = 1; j < gridSize.y - 1; j++)
			{
				int i = 0;
				
				float[] sides = new float[] {
					lastHeats[i + 0, j + 1], //Up
					lastHeats[i + 0, j - 1], //Down
					lastHeats[i + 1, j + 0], //Left
					lastHeats[i + 1, j + 1], //Up - Left
					lastHeats[i + 1, j - 1], //Down - Left
				};

				float sum = 0;
				System.Array.ForEach<float>(sides, delegate (float i) { sum += i; });

				float avg = sum / 5;

				heatGrid[i, j] = Mathf.Lerp(lastHeats[i, j], avg, heatDeltaTime * heatDispersionSpeed);
			}

			//Left (x = maxX, no left)
			for (int j = 1; j < gridSize.y - 1; j++)
			{
				int i = gridSize.x - 1;
				
				float[] sides = new float[] {
					lastHeats[i + 0, j + 1], //Up
					lastHeats[i + 0, j - 1], //Down
					lastHeats[i - 1, j + 0], //Right
					lastHeats[i - 1, j + 1], //Up - Right
					lastHeats[i - 1, j - 1]  //Down - Right
				};

				float sum = 0;
				System.Array.ForEach<float>(sides, delegate (float i) { sum += i; });

				float avg = sum / 5;

				heatGrid[i, j] = Mathf.Lerp(lastHeats[i, j], avg, heatDeltaTime * heatDispersionSpeed);
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

				if (heatGrid[i, j] > weldHeat)
				{
					float bright2TooBright = (heatGrid[i, j] - weldHeat) / (overHeat - weldHeat);
					pixels[index] = Color32.Lerp(brightColor, tooBrightColor, bright2TooBright);

					if (heatGrid[i, j] > overHeat)
					{
						//Melt code here
					}
				}
				else
				{
					float dark2Bright = heatGrid[i, j] / weldHeat;
					pixels[index] = Color32.Lerp(baseColors[index], brightColor, dark2Bright);
				}
			}
		}

		texture.SetPixels32(pixels);
		texture.Apply();
	}

	//Update corners positions for calculations
	private void UpdateCorners()
	{
		float vertical = gridSize.y / burnGridDensity;
		float horizontal = gridSize.x / burnGridDensity;

		top = normal.up * vertical;
		right = normal.right * horizontal;

		topLeft = normal.position + top - right;
		topRight = normal.position + top + right;
		bottomLeft = normal.position - top - right;
		bottomRight = normal.position - top + right;
	}

	// Start is called before the first frame update
	private void Start()
	{
		plane = new Plane(normal.forward, normal.position);

		heatGrid = new float[gridSize.x, gridSize.y];

		texture = new Texture2D(gridSize.x, gridSize.y);
		texture.filterMode = FilterMode.Point;

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
		DrawDebugCorners();
	}

	//Draws corners
	private void DrawDebugCorners()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(topLeft, 0.1f);

		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(bottomLeft, 0.1f);

		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(topRight, 0.1f);

		Gizmos.color = Color.cyan;
		Gizmos.DrawSphere(bottomRight, 0.1f);
	}

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