using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeldPlateScorer : MonoBehaviour
{
	public float maxDistance = 0.1f;

	public Vector2Int A;
	public Vector2Int B;
	public Vector2Int C;
	public Vector2Int D;

	public WeldPlate weldPlate;

	private struct Line
	{
		public Vector2 p0;
		public Vector2 p1;

		public float m, c;

		public float minX, maxX, minY, maxY;

		public Line(Vector2 p0, Vector2 p1)
		{
			this.p0 = p0;
			this.p1 = p1;

			//m = (y1 - y0) / (x1 - x0);
			m = (p1.y - p0.y) / (p1.x - p0.x);
			//c = y - mx
			c = p1.y - m * p1.x;

			//bounding box
			minX = Mathf.Min(p0.x, p1.x);
			maxX = Mathf.Max(p0.x, p1.x);
			minY = Mathf.Min(p0.y, p1.y);
			maxY = Mathf.Max(p0.y, p1.y);
		}
	}

	private Vector2Int[] scorePixels;

	// Start is called before the first frame update
	private void Start()
	{
		scorePixels = GetScorePixels();
	}

	// FixedUpdate is called every physics update
	private void FixedUpdate()
	{
		float score = CalculateScore();
		Debug.Log("score " + score);
	}

	private float CalculateScore()
	{
		int totalPossibleScore = scorePixels.Length;
		int totalScore = 0;

		foreach (var pixel in scorePixels)
		{
			if (weldPlate.GetHeatAt(pixel) > weldPlate.weldHeat)
			{
				totalScore++;
			}
		}

		return (float)totalScore / (float)totalPossibleScore;
	}

	public Texture2D debugScorePixels;

	private Vector2Int[] GetScorePixels()
	{
		List<Vector2Int> scorePixels = new List<Vector2Int>();
		Line[] lines = new Line[] {
			new Line(A, B),
			new Line(B, C),
			new Line(C, D),
			new Line(D, A)
		};

		Color32[] pixels = new Color32[weldPlate.gridSize.x * weldPlate.gridSize.y];

		for (int x = 0; x < weldPlate.gridSize.x; x++)
		{
			for (int y = 0; y < weldPlate.gridSize.y; y++)
			{
				Vector2Int pixel = new Vector2Int(x, y);
				bool hasAdded = false;
				for (int i = 0; i < lines.Length && !hasAdded; i++)
				{
					if (IsInBoundingBox(lines[i], pixel))
					{
						float distance = GetDistance(lines[i], pixel);
						if (distance < maxDistance)
						{
							scorePixels.Add(pixel);
							hasAdded = true;
						}
					}
				}
			}
		}

		debugScorePixels = new Texture2D(weldPlate.gridSize.x, weldPlate.gridSize.y);
		debugScorePixels.SetPixels32(pixels);
		debugScorePixels.Apply();

		return scorePixels.ToArray();
	}
	private bool IsInBoundingBox(Line line, Vector2Int pixel)
	{
		if (pixel.x < line.minX || pixel.x > line.maxX) return false;
		if (pixel.y < line.minY || pixel.y > line.maxY) return false;
		return true;
	}
	private float GetDistance(Line line, Vector2Int pixel)
	{
		if (line.m == 0)
		{
			return Mathf.Abs(pixel.y - line.p0.y);
		}

		float m2 = -1 / line.m;
		float c2 = pixel.y - m2 * pixel.x; // y - m2x

		float intersectX = -(line.c - c2) / (line.m - m2); // -(c - c2 ) / (m - m2)
		float intersectY = m2 * intersectX + c2;

		Vector2 intersect = new Vector2(intersectX, intersectY);

		float dist = Vector2.Distance(intersect, pixel);
		return dist;
	}

}

