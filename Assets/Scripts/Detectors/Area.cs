using UnityEngine;

public class Area : MonoBehaviour
{
	public string areaName;
    
    // On Trigger Enter is called when the collider of another GameObject begins colliding with the collider of this GameObject (while this collider is trigger)
    private void OnTriggerEnter(Collider otherCollider)
    {
		PlayerUI playerUI = otherCollider.gameObject.GetComponentInParent<PlayerUI>();

		if(playerUI != null)
        {
			playerUI.SetArea(areaName);
		}
    }
}
