using UnityEngine;

public class Player : MonoBehaviour
{
	public string playerName;

	[Header("Attachments")]
	public PlayerSense sensor;
	public PlayerPickup pickuper;

	// Start is called before the first frame update
	private void Start()
	{
		sensor.player = this;
		pickuper.player = this;
	}
}