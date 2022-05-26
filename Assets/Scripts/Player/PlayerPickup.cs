using UnityEngine;
using UnityEngine.Events;

public class PlayerPickup : MonoBehaviour
{
	public PlayerSense sensor;
	public Transform itemParent;

	[HideInInspector] public bool isAllowedToDropItem = true;

	[HideInInspector] public Player player;
	[HideInInspector] public Pickupable item;

	[Space(10)]
	public UnityEvent OnPickupItem;
	public UnityEvent OnDropItem;

	//Vars to switch back when dropping
	private Transform oldParent;
	private Rigidbody rb;
	private Collider[] cols;

	//Main functions
	public void Pickup(Pickupable pickupable)
	{
		OnPickupItem.Invoke();

		item = pickupable;

		PlayerUI.ui.itemText.text = "Right click to drop item.";

		//Disable the rigibody on the item
		rb = item.gameObject.GetComponent<Rigidbody>();
		if (rb != null) rb.isKinematic = true;

		//Disable any colliders on the item
		cols = item.gameObject.GetComponentsInChildren<Collider>();
		foreach (Collider col in cols)
		{
			if (col != null) col.enabled = false;
		}

		//Reparent to the transform
		item.transform.SetParent(itemParent);

		//Reset the local transform of the item
		item.transform.localPosition = Vector3.zero;
		item.transform.localRotation = Quaternion.identity;
	}
	public void Drop(bool enableRigidbody = true)
	{
		OnDropItem.Invoke();

		PlayerUI.ui.itemText.text = "";

		//Enable the rigibody on the item
		if (rb != null) rb.isKinematic = !enableRigidbody;

		//Disable any cols on the item
		foreach (Collider col in cols)
		{
			if (col != null) col.enabled = enableRigidbody;
		}

		//Reparent to the old transform
		item.transform.SetParent(oldParent);

		item = null;
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