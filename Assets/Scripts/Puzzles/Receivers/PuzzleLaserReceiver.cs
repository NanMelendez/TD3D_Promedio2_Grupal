using System.Collections.Generic;
using UnityEngine;

public class PuzzleLaserReceiver : PuzzleComponent
{
	private bool hasBeenHitThisFrame = false;

	private void LateUpdate()
	{
		if (hasBeenHitThisFrame)
		{
			if (!isActive)
				Activate();
		}
		else
		{
			if (isActive)
				Deactivate();
		}
		hasBeenHitThisFrame = false;
	}

	public void OnLaserHit()
	{
		hasBeenHitThisFrame = true;
	}
}
