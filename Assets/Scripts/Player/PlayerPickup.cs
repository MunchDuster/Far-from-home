using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
	public PlayerSense sensor;
	public Transform itemParent;

	[HideInInspector] public Player player;
	[HideInInspector] public Pickupable item;

	//Vars to switch back when dropping
	private Transform oldParent;
	private Rigidbody rb;
	private Collider col;

	//Main functions
	public void Pickup(Pickupable pickupable)
	{
		item = pickupable;

		//Disable the rigibody on the item
		rb = item.gameObject.GetComponent<Rigidbody>();
		if (rb != null) rb.isKinematic = true;

		//Enable any cols on the item
		col = item.gameObject.GetComponent<Collider>();
		if (col != null) col.enabled = false;

		//Reparent to the transform
		item.transform.SetParent(itemParent);

		//Reset the local transform of the item
		item.transform.localPosition = Vector3.zero;
		item.transform.localRotation = Quaternion.identity;
	}
	public void Drop()
	{
		//Enable the rigibody on the item
		if (rb != null) rb.isKinematic = false;

		//Disable any cols on the item
		if (col != null) col.enabled = true;

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
			Drop();
		}
	}
}