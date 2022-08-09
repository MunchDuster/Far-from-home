using UnityEngine;
using UnityEngine.Events;

public class PlayerPickup : MonoBehaviour
{
	public PlayerSense sensor;
	public Transform itemTarget;

	[HideInInspector] public bool isAllowedToDropItem = true;

	[HideInInspector] public Player player;
	[HideInInspector] public Pickupable item;

	[Space(10)]
	public UnityEvent OnPickupItem;
	public UnityEvent OnDropItem;

	//Vars to switch back when dropping
	private Transform oldParent;
	private Rigidbody rb;

	//Main functions
	public void Pickup(Pickupable pickupable)
	{
		OnPickupItem.Invoke();
		item = pickupable;
		itemTarget.localPosition = pickupable.offset;
		itemTarget.localRotation = Quaternion.Euler(pickupable.offsetRotation);
		rb = item.gameObject.GetComponent<Rigidbody>();
		PlayerUI.ui.itemText.text = "<mark=#202020A0>Right click to drop item.</mark>";
		SetControllingItem(true);
	}
	public void Drop(bool enableRigidbody = true)
	{
		OnDropItem.Invoke();
		SetControllingItem(false, enableRigidbody);
		PlayerUI.ui.itemText.text = "";
		rb = null;
		item = null;

	}

	[Space(10)]
	public float matchForce = 40;
	public float matchForceDamping = 0.5f;
	public float matchTorque = 10;
	public float matchTorqueDamping = 0.5f;

	private bool updateRigidbody = false;

	public void SetControllingItem(bool control, bool enableRigidbody = true)
	{
		updateRigidbody = control && rb != null;
		if(item != null) item.MakeDetectable(!control);

		if(rb != null)
		{
			rb.isKinematic = enableRigidbody ? false : !control;
			if(control) rb.position = itemTarget.position;
		}
	}

	void FixedUpdate()
    {
        if(updateRigidbody)
		{
			Vector3 force = (itemTarget.position - rb.position) * matchForce - rb.velocity * matchForceDamping;
        	rb.AddForce(force, ForceMode.VelocityChange);
			
			Vector3 deltaEulers = Quaternion.FromToRotation(rb.transform.forward, itemTarget.forward).eulerAngles;
			Vector3 turnOffset = new Vector3(
				deltaEulers.x > 180f ? deltaEulers.x - 360f : deltaEulers.x,
				deltaEulers.y > 180f ? deltaEulers.y - 360f : deltaEulers.y,
				deltaEulers.z > 180f ? deltaEulers.z - 360f : deltaEulers.z
			);

			Vector3 torque = turnOffset * matchTorque - rb.angularVelocity * matchTorqueDamping;
			rb.AddTorque(torque);
		}
    }

	// Update is called every frame
	private void Update()
	{
		//Check if wants to drop
		if (Input.GetMouseButtonDown(1) && item != null)
		{
			if (isAllowedToDropItem)
			{
				Drop();
			}
		}
	}
}