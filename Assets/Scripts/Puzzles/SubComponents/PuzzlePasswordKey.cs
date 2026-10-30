using UnityEngine;

public class PuzzlePasswordKey : MonoBehaviour, IInteractable
{
	[SerializeField] private PuzzlePassword receiver;
	[SerializeField] private string value;

	public void Interact()
	{
		receiver.Append(value);
	}
}
