using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class WeldPlace : Minigame
{
	public static WeldPlace current;

	public Transform plateTransform;

	public UnityEvent<bool> onStartGame;
	public Welder welder;
	public Transform welderPoint;
	public Pickupable welderPickup;

	[HideInInspector] public WeldPlate plate;

	// Awake is called when the gameObject is activated
	private void Awake()
	{
		current = this;
	}
	// Start is called before the first frame update
	private void Start()
	{
		OnPlayerJoin += PlayerJoin;
	}

	private void PlayerJoin(bool on)
	{
		onStartGame.Invoke(on);
		if(on)
		{
			welder.welderBase.position = welderPoint.position;
		}
		else
		{
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
				plate.RecalculatePlane();

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
				plate.StartWelding();
				welder.plate = plate;
				OnGameUpdate += welder.GameUpdate;

				Debug.Log("Starting weld");


				return InteractionInfo.Success();
			}
			else
			{
				return InteractionInfo.Fail("Can't weld with that.");
			}
		}
	}
}