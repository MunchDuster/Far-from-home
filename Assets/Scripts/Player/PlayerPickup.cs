using UnityEngine;
using UnityEngine.Events;

public class PlayerPickup : MonoBehaviour
{
	public PlayerSense sensor;
	public Transform itemParent;
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

		PlayerUI.ui.itemText.text = "<mark=#202020A0>Right click to drop item.</mark>";

		//Disable the rigibody on the item
		rb = item.gameObject.GetComponent<Rigidbody>();
		//if (rb != null) rb.isKinematic = true;

		//Reparent to the transform
		//item.transform.SetParent(itemParent);

		//Reset the local transform of the item
			rb.position = itemTarget.position;

		//item.transform.localPosition = item.offset;
		//item.transform.localRotation = Quaternion.Euler(item.offsetRotation);
	}
	public void Drop(bool enableRigidbody = true)
	{
		OnDropItem.Invoke();

		PlayerUI.ui.itemText.text = "";

		//Enable the rigibody on the item
		//if (rb != null) rb.isKinematic = !enableRigidbody;
		rb = null;

		//Reparent to the old transform
		//item.transform.SetParent(oldParent);

		item = null;
	}

	[Space(10)]
	public float matchForce = 40;
	public float matchForceDamping = 0.5f;
	public float matchTorque = 10;
	public float matchTorqueDamping = 0.5f;

	public bool updateRigidbody = true;

	public void SetControllingItem(bool control)
	{
		updateRigidbody = control;
		rb.isKinematic = !control;

		if(control)
		{
			//Move item to target (less chance of gettig stuck behind something)
			rb.position = itemTarget.position;
		}
	}

	void FixedUpdate()
    {
        if(rb != null && updateRigidbody)
		{
			Vector3 force = (itemTarget.position - rb.position) * matchForce - rb.velocity * matchForceDamping;
        	rb.AddForce(force, ForceMode.VelocityChange);
			
			Vector3 deltaEulers = Quaternion.FromToRotation(rb.transform.forward, transform.forward).eulerAngles;
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