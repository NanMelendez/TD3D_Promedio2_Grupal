using System.Collections;
using UnityEngine;

public class PuzzleButton : PuzzleComponent, IInteractable
{
	[SerializeField] private bool isToggleMode = false;
	[SerializeField][Min(0.01f)] private float togglebackTimer = 5.0f;

	private Coroutine toggleCoroutine;
	private float remainingTime = 0.0f;

	public float TogglebackDuration => togglebackTimer;
	public float RemainingTime => remainingTime;

	public void Interact()
	{
		if (isToggleMode)
		{
			if (isActive)
				Deactivate();
			else
				Activate();
		}
		else
		{
			if (!isActive)
				Activate();
			else
			{
				remainingTime = togglebackTimer;
				if (toggleCoroutine != null)
					StopCoroutine(toggleCoroutine);
				toggleCoroutine = StartCoroutine(ToggleBackCountdown());
			}
		}
	}

	protected override void Activate()
	{
		base.Activate();

		if (!isToggleMode)
		{
			remainingTime = togglebackTimer;
			if (toggleCoroutine != null)
				StopCoroutine(toggleCoroutine);
			toggleCoroutine = StartCoroutine(ToggleBackCountdown());
		}
	}

	protected override void Deactivate()
	{
		if (!isActive)
			return;

		if (toggleCoroutine != null)
		{
			StopCoroutine(toggleCoroutine);
			toggleCoroutine = null;
		}

		remainingTime = 0.0f;
		base.Deactivate();
	}

	private IEnumerator ToggleBackCountdown()
	{
		while (remainingTime > 0.0f)
		{
			remainingTime -= Time.deltaTime;
			yield return null;
		}

		Deactivate();
	}
}
