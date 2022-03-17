using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Engine : Minigame
{
	public Transform itemPlane;

	public Slider fullnessSlider;

	public UnityEvent OnStart;
	public UnityEvent OnStop;

	private Plane plane;


	// Start is called before the first frame update
	protected override void Start()
	{
		base.Start();

		OnPlayerJoin += StartGame;
		OnPlayerLeave += StopGame;
		OnGameUpdate += GameUpdate;

		plane = new Plane(itemPlane.right, itemPlane.up);
	}

	private Rigidbody rb;

	private void StartGame()
	{
		OnStart.Invoke();
		if (player.pickuper.item != null)
		{
			rb = player.pickuper.item.GetComponentInChildren<Rigidbody>();

			if (rb != null) rb.isKinematic = false;
		}
	}
	private void GameUpdate()
	{
		if (player.pickuper.item == null) return;

		if (Input.GetMouseButton(0)) player.pickuper.item.transform.position = GetItemPosition();
	}
	private void StopGame()
	{
		OnStop.Invoke();
		if (player.pickuper.item != null)
		{
			if (rb != null) rb.isKinematic = true;
		}
	}

	private Vector3 GetItemPosition()
	{
		Ray ray = player.camera.ScreenPointToRay(Input.mousePosition);

		if (plane.Raycast(ray, out float enter))
		{
			Vector3 hitPoint = ray.GetPoint(enter);
			return hitPoint;
		}
		else
		{
			return Vector3.zero;
		}

	}

}