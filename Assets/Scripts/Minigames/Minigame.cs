using UnityEngine;

public abstract class Minigame : Interactable
{
	public Transform cameraPoint;
	public RequirementList requirementsToPlay;

	protected delegate void MinigameEvent();
	protected event MinigameEvent OnPlayerJoin;
	protected event MinigameEvent OnPlayerLeave;
	protected event MinigameEvent OnGameUpdate;

	protected Player player;

	private Vector3 initialLocalPosition;
	private Quaternion initialLocalRotation;

	// Start is called before the first frame update
	protected virtual void Start()
	{
		requirementsToPlay.Start();
	}

	//Checks whether the player is leaving the game
	private void CheckLeave()
	{
		//Move player camera to cameraPoint
		player.camera.transform.position = cameraPoint.position;
		player.camera.transform.rotation = cameraPoint.rotation;

		if (Input.GetKeyDown(KeyCode.Space))
		{
			EndGame();
		}
	}

	//Interact is called when the player clicks on this
	public override InteractionInfo Interact(Player player)
	{
		//Player already using, clicking on it for another reason
		if (this.player != null) return InteractionInfo.Success();

		//Check if reuirements met
		if (!requirementsToPlay.completed)
		{
			return InteractionInfo.Fail(requirementsToPlay.GetIncompleteTask().description);
		}

		//Set var
		this.player = player;

		StartGame();

		//Return success
		return InteractionInfo.Success();
	}

	private void StartGame()
	{
		//Save the current player camera transform
		initialLocalPosition = player.camera.transform.localPosition;
		initialLocalRotation = player.camera.transform.localRotation;

		//Move player camera to cameraPoint
		player.camera.transform.position = cameraPoint.position;
		player.camera.transform.rotation = cameraPoint.rotation;

		//Unlock the cursor
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		//Disable player movement
		player.movement.enabled = false;

		//Add check for if plaer is trying to leave game
		OnGameUpdate += CheckLeave;

		//Call delegate
		OnPlayerJoin();
	}

	private void EndGame()
	{
		//Reset camera
		player.camera.localPosition = initialLocalPosition;
		player.camera.localRotation = initialLocalRotation;

		//Lock the cursor
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		//Enable player movement
		player.movement.enabled = true;

		//Remove check for if plaer is trying to leave game
		OnGameUpdate -= CheckLeave;

		//Call delegate
		OnPlayerLeave();

		//Reset player var
		player = null;
	}

	// Update is called every frame
	private void Update()
	{
		if (player != null) OnGameUpdate();
	}

}