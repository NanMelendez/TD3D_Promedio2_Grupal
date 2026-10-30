using UnityEngine;

public class PuzzleMirror : MonoBehaviour, IInteractable
{
	[SerializeField] private Transform mirrorPivot = null;
	[SerializeField][Min(0.1f)] private float rotationStep = 45.0f;
	[SerializeField] private bool verticalRotation = false;
	[SerializeField] private int initialSteps = 0;

	private Vector2Int rotationsCount = Vector2Int.zero;

	private void Start()
	{
		if (!mirrorPivot)
			mirrorPivot = transform;
		rotationsCount = initialSteps * new Vector2Int(verticalRotation ? 0 : 1, verticalRotation ? 1 : 0);
		ApplyRotation();
	}

	public void Interact()
	{
		int maxSteps = Mathf.RoundToInt(360.0f / rotationStep);

		if (verticalRotation)
			rotationsCount.y = (rotationsCount.y + 1) % maxSteps;
		else
			rotationsCount.x = (rotationsCount.x + 1) % maxSteps;

		ApplyRotation();
	}

	private void ApplyRotation()
	{
		float x = rotationsCount.y * rotationStep;
		float y = rotationsCount.x * rotationStep;

		mirrorPivot.localRotation = Quaternion.Euler(x, y, 0.0f);
	}
}
