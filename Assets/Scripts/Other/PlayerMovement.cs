using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
	//Refs
	[HideInInspector] public Rigidbody rb;
	public Transform joint;
	public Camera playerCamera;

	//Events
	public UnityEvent OnMoveStart;
	public UnityEvent OnMoveStop;

	//Controls
	public bool cameraCanMove = true;
	public bool playerCanMove = true;
	public bool enableJump = true;
	public bool enableCrouch = true;
	public bool enableHeadBob = true;
	public bool enableSprint = true;

	//Controls setters (for unity events)
	public void SetCameraCanMove(bool cameraCanMove) { this.cameraCanMove = cameraCanMove; }
	public void SetPlayerCanMove(bool playerCanMove) { this.playerCanMove = playerCanMove; }
	public void SetEnableJump(bool enableJump) { this.enableJump = enableJump;}
	public void SetEnableCrouch(bool enableCrouch) { this.enableCrouch = enableCrouch;}
	public void SetEnableHeadBob(bool enableHeadBob) { this.enableHeadBob = enableHeadBob; }
	public void SetEnableSprint(bool enableSprint) { this.enableSprint = enableSprint; }

	//States
	private bool isWalking = false;
	private bool isSprinting = false;
	private bool isCrouched = false;
	private bool isGrounded = false;

	//Keycodes
	public KeyCode sprintKey = KeyCode.LeftShift;
	public KeyCode jumpKey = KeyCode.Space;
	public KeyCode crouchKey = KeyCode.LeftControl;
	
	//Input states
	private bool sprintPressed = false;
	private bool jumpPressed = false;
	private bool crouchPressed = false;
	private Vector3 moveInput = Vector3.zero;
	private Vector3 lookInput = Vector3.zero;

	//Origins
	private Vector3 originalScale;
	private Vector3 jointOriginalPos;

	//Camera
	public float fov = 60f;
	public float mouseSensitivity = 2f;
	public float maxLookAngle = 50f;
	private float yaw = 0.0f;
	private float pitch = 0.0f;

	//Movement
	public float walkSpeed = 5f;
	public float maxVelocityChange = 10f;

	//Sprint
	public bool unlimitedSprint = true;
	public float sprintSpeed = 7f;
	public float sprintDuration = 5f;
	public float sprintCooldown = .5f;
	public float sprintFOV = 80f;
	public float sprintFOVStepTime = 10f;

	private float sprintRemaining;
	private bool isSprintCooldown = false;
	private float sprintCooldownReset;

	//Jump
	public float jumpPower = 5f;
	private bool awaitingJump = false;

	//Grounded
	public bool IsGrounded() { return isGrounded; }
	public float maxRaycastDist = 1;

	//Crouch
	public float crouchHeight = .75f;
	public float speedReduction = .5f;

	//Head bob
	public float bobSpeed = 10f;
	public Vector3 bobAmount = new Vector3(.15f, .05f, 0f);
	private float timer = 0;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();

		// Set internal variables
		playerCamera.fieldOfView = fov;
		originalScale = transform.localScale;
		jointOriginalPos = joint.localPosition;

	}
	private void OnEnable()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}
	private void OnDisable()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	private void Update()
	{
		CheckGround();

		UpdateInputs();

		if (cameraCanMove)
		{
			UpdateCamera();
		}
		if (enableSprint)
		{
			UpdateSprint();
		}
		
		if (enableJump && jumpPressed && isGrounded)
		{
			Jump();
		}
		if (enableCrouch)
		{
			CheckCrouch();
		}
		if (enableHeadBob)
		{
			HeadBob();
		}
	}
	private void UpdateInputs()
	{
		sprintPressed = Input.GetKeyDown(sprintKey);
		jumpPressed = Input.GetKeyDown(jumpKey);
		crouchPressed = Input.GetKeyDown(crouchKey);

		moveInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		lookInput = new Vector3(Input.GetAxis("Mouse X"), 0, Input.GetAxis("Mouse Y"));
	}
	private void CheckCrouch()
	{
		if (crouchPressed)
		{
			if(isCrouched)
			{
				isCrouched = false;
				ToggleCrouch();
			}
		}
		else
		{
			if(!isCrouched)
			{
				isCrouched = true;
				ToggleCrouch();
			}	
		}
	}
	private void UpdateCamera()
	{
		yaw = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * mouseSensitivity;
		pitch -= mouseSensitivity * Input.GetAxis("Mouse Y");

		// Clamp pitch between lookAngle
		pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);
		transform.localEulerAngles = new Vector3(0, yaw, 0);
		joint.transform.localEulerAngles = new Vector3(pitch, 0, 0);
	}
	private void UpdateSprint()
	{
		if (isSprinting)
			{
				playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, sprintFOV, sprintFOVStepTime * Time.deltaTime);

				// Drain sprint remaining while sprinting
				if (!unlimitedSprint)
				{
					sprintRemaining -= 1 * Time.deltaTime;
					if (sprintRemaining <= 0)
					{
						isSprinting = false;
						isSprintCooldown = true;
					}
				}
			}
			else
			{
				// Regain sprint while not sprinting
				sprintRemaining = Mathf.Clamp(sprintRemaining += 1 * Time.deltaTime, 0, sprintDuration);
			}

			// Handles sprint cooldown 
			// When sprint remaining == 0 stops sprint ability until hitting cooldown
			if (isSprintCooldown)
			{
				sprintCooldown -= 1 * Time.deltaTime;
				if (sprintCooldown <= 0)
				{
					isSprintCooldown = false;
				}
			}
			else
			{
				sprintCooldown = sprintCooldownReset;
			}
	}
	private void HeadBob()
	{
		if (isWalking)
		{
			// Calculates HeadBob speed during sprint
			if (isSprinting)
			{
				timer += Time.deltaTime * (bobSpeed + sprintSpeed);
			}
			// Calculates HeadBob speed during crouched movement
			else if (isCrouched)
			{
				timer += Time.deltaTime * (bobSpeed * speedReduction);
			}
			// Calculates HeadBob speed during walking
			else
			{
				timer += Time.deltaTime * bobSpeed;
			}
			// Applies HeadBob movement
			joint.localPosition = new Vector3(jointOriginalPos.x + Mathf.Sin(timer) * bobAmount.x, jointOriginalPos.y + Mathf.Sin(timer) * bobAmount.y, jointOriginalPos.z + Mathf.Sin(timer) * bobAmount.z);
		}
		else
		{
			// Resets when play stops moving
			timer = 0;
			joint.localPosition = new Vector3(Mathf.Lerp(joint.localPosition.x, jointOriginalPos.x, Time.deltaTime * bobSpeed), Mathf.Lerp(joint.localPosition.y, jointOriginalPos.y, Time.deltaTime * bobSpeed), Mathf.Lerp(joint.localPosition.z, jointOriginalPos.z, Time.deltaTime * bobSpeed));
		}
	}
	private void ToggleCrouch()
	{
		if (isCrouched)
		{
			transform.localScale = new Vector3(originalScale.x, crouchHeight, originalScale.z);
			walkSpeed *= speedReduction;
		}
		else
		{
			transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
			walkSpeed /= speedReduction;
		}
	
	}
	private void Jump()
	{
		awaitingJump = true;
		
		// Uncrouch for a jump
		if (isCrouched)
		{
			isCrouched = false;
			ToggleCrouch();
		}
	}

	void FixedUpdate()
	{
		if (playerCanMove)
		{
			bool wasWalking = isWalking;
			bool wantsToMove = moveInput.x != 0 || moveInput.z != 0;
			bool canSprint = sprintRemaining > 0f && !isSprintCooldown;

			isWalking = wantsToMove && playerCanMove && isGrounded;
			isSprinting = isWalking && enableSprint && sprintPressed && canSprint;

			if(isWalking && !wasWalking)
			{
				OnMoveStart.Invoke();
			}
			else if (!isWalking && wasWalking)
			{
				OnMoveStop.Invoke();
			}

			if (isSprinting)
			{
				ApplyMoveForce(sprintSpeed);
			}
			else
			{
				ApplyMoveForce(walkSpeed);
			}
		}
	}
	private void ApplyMoveForce(float speed)
	{
		Vector3 targetDirection = transform.TransformDirection(moveInput) * speed;

		Vector3 velocity = rb.velocity;
		Vector3 velocityDifference = (targetDirection - velocity);

		float yVelocity = 0;
		
		if(awaitingJump)
		{
			yVelocity = jumpPower;
			awaitingJump = false;
		}
		
		Vector3 velocityChange = new Vector3(
			Mathf.Clamp(velocityDifference.x, -maxVelocityChange, maxVelocityChange),
			yVelocity,
			Mathf.Clamp(velocityDifference.z, -maxVelocityChange, maxVelocityChange)
		);	

		rb.AddForce(velocityChange, ForceMode.VelocityChange);
	}
	private void CheckGround()
	{
		Vector3 origin = transform.position;
		Vector3 direction = transform.TransformDirection(Vector3.down);

		if (Physics.Raycast(origin, direction, out RaycastHit hit, maxRaycastDist))
		{
			Debug.DrawRay(origin, direction * maxRaycastDist, Color.red);
			isGrounded = true;
		}
		else
		{
			isGrounded = false;
		}
	}
}