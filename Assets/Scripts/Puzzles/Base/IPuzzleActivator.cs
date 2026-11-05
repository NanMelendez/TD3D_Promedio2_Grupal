using System;

public interface IPuzzleActivator
{
	bool IsActive { get; }
	event Action<bool> OnStateChanged;
}
