using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleSignalReceiver : PuzzleComponent, IPuzzleProgressProvider
{
	[SerializeField] private List<PuzzleComponent> activators = new();
	public UnityEvent OnActivated;
	public UnityEvent OnDeactivated;
	public event Action<int, int> OnProgressChanged;
	public int CurrentProgress => activators.Cast<IPuzzleActivator>().Count(activator => activator.IsActive);
	public int MaxProgress => activators.Count;

	private void Awake()
	{
		foreach (IPuzzleActivator activator in activators.Cast<IPuzzleActivator>())
			activator.OnStateChanged += OnActivatorStateChanged;
	}

	private void OnDestroy()
	{
		foreach (IPuzzleActivator activator in activators.Cast<IPuzzleActivator>())
			activator.OnStateChanged -= OnActivatorStateChanged;
	}

	private void OnActivatorStateChanged(bool newState)
	{
		OnProgressChanged?.Invoke(CurrentProgress, MaxProgress);
		EvaluateState();
	}

	private void EvaluateState()
	{
		bool allActive = activators.Count > 0 && activators.Cast<IPuzzleActivator>().All(activator => activator.IsActive);

		if (allActive != isActive)
		{
			if (allActive)
				Activate();
			else
				Deactivate();

			if (isActive)
				OnActivated?.Invoke();
			else
				OnDeactivated?.Invoke();
		}
	}
}
