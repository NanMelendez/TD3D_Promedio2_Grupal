using UnityEngine;

public class PlayerPickupCollector : MonoBehaviour, IInteractable
{
	private PuzzlePickupData piece = null;

	public PuzzlePickupType Type => piece.type;
	public bool HoldingPiece => piece != null;
	public PuzzlePickupData HeldPiece => piece;

	public bool TryPickUp(PuzzlePickupData data)
	{
		if (HoldingPiece)
		{
			Debug.Log("Ya se está cargando con una pieza");
			return false;
		}

		piece = data;
		return true;
	}

	public PuzzlePickupData Consume()
	{
		PuzzlePickupData temp = piece;
		piece = null;
		return temp;
	}

	public void Interact()
	{
		// ...
	}
}
