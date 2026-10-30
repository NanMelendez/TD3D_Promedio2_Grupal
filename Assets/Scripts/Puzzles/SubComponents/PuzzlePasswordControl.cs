using UnityEngine;

public class PuzzlePasswordControl : MonoBehaviour, IInteractable
{
	public enum ControlType { Delete, Submit };

	[SerializeField] private PuzzlePassword receiver;
	[SerializeField] private ControlType actionType;

	public void Interact()
	{
		switch (actionType)
		{
			case ControlType.Delete:
				receiver.DeleteLast();
				break;
			case ControlType.Submit:
				receiver.Submit();
				break;
		}
	}
}
