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

	//Main functions
	public void Pickup(Pickupable pickupable)
	{
		OnPickupItem.Invoke();

		item = pickupable;

		PlayerUI.ui.itemText.text = "<mark=#202020A0>Right click to drop item.</mark>";

		//Disable the rigibody on the item
		rb = item.gameObject.GetComponent<Rigidbody>();
		if (rb != null) rb.isKinematic = true;

		//Reparent to the transform
		item.transform.SetParent(itemParent);

		//Reset the local transform of the item
		item.transform.localPosition = item.offset;
		item.transform.localRotation = Quaternion.Euler(item.offsetRotation);
	}
	public void Drop(bool enableRigidbody = true)
	{
		OnDropItem.Invoke();

		PlayerUI.ui.itemText.text = "";

		//Enable the rigibody on the item
		if (rb != null) rb.isKinematic = !enableRigidbody;

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