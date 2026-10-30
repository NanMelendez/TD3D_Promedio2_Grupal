using System;

public interface IPuzzleProgressProvider
{
	int CurrentProgress { get; }
	int MaxProgress { get; }

	event Action<int, int> OnProgressChanged;
}
