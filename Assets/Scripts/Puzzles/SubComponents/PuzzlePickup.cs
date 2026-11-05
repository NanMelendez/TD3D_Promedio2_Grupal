using UnityEngine;

public class PuzzlePickup : MonoBehaviour, IInteractable
{
	[SerializeField] private PuzzlePickupData data;

	public PuzzlePickupType Type => data.type;
	public bool IsFake => data.isFake;

	public void Interact()
	{
		Debug.Log("Interactuando con pieza");
		PlayerPickupCollector collector = FindFirstObjectByType<PlayerPickupCollector>();

		if (collector && collector.TryPickUp(data))
			Destroy(gameObject);
	}
}
