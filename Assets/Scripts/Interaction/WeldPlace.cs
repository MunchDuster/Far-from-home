using UnityEngine;
using UnityEngine.Events;

public class WeldPlace : Minigame
{
	public Transform plateTransform;

	public UnityEvent onStartGame;
	public UnityEvent onStopGame;

	public Welder welder;
	public Transform welderPoint;
	public Pickupable welderPickup;

	[HideInInspector] public WeldPlate plate;

	// Start is called before the first frame update
	private void Start()
	{
		OnPlayerJoin += PlayerJoin;
	}

	private void PlayerJoin(bool on)
	{
		if(on)
		{
			welder.welderBase.position = welderPoint.position;
			onStartGame.Invoke();
		}
		else
		{
			onStopGame.Invoke();
			OnGameUpdate -= welder.GameUpdate;
			plate.StopWelding();
			Debug.Log("Leaving weld");
		}
	}

	protected override InteractionInfo CheckRequirements(Player player)
	{
		if (plate == null)
		{
			if(player.pickuper.item == null)
			{
				return InteractionInfo.Fail("Need something to cover it");
			}
			else if (player.pickuper.item.GetType() == typeof(WeldPlate))
			{
				plate = player.pickuper.item as WeldPlate;
				plate.transform.position = plateTransform.position;
				plate.transform.rotation = plateTransform.rotation;
				plate.transform.parent = transform;

				player.pickuper.Drop(false);

				plate.MakeUndetectable();

				return InteractionInfo.Fail("");
			}
			else
			{
				return InteractionInfo.Fail("Needs a plate to weld against.");
			}
		}
		else
		{
			if (player.pickuper.item == welderPickup)
			{
				welder.plate = plate;
				OnGameUpdate += welder.GameUpdate;

				Debug.Log("Starting weld");

				plate.StartWelding();

				return InteractionInfo.Success();
			}
			else
			{
				return InteractionInfo.Fail("Can't weld with that.");
			}
		}
	}
}