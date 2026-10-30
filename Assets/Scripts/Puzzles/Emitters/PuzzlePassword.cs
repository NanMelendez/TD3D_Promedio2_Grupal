using System;
using UnityEngine;
using UnityEngine.Events;

public class PuzzlePassword : PuzzleComponent
{
	[SerializeField] private string expectedPassword;
	[SerializeField][Min(4)] private int maxCharacters = 4;

	private string currentInput = "";

	public UnityEvent OnPasswordCorrect;
	public UnityEvent OnPasswordWrong;
	public string CurrentInput => currentInput;
	public int MaxCharacters => maxCharacters;
	public string ExpectedPassword => expectedPassword;
	public event Action<string> OnInputChanged;

	private void Start()
	{
		OnInputChanged?.Invoke(currentInput);
	}

	public void Append(string ch)
	{
		if (isActive)
			return;

		if (currentInput.Length < maxCharacters)
		{
			currentInput += ch;
			OnInputChanged?.Invoke(currentInput);
		}
	}

	public void DeleteLast()
	{
		if (isActive)
			return;

		if (currentInput.Length > 0)
		{
			currentInput = currentInput.Substring(0, currentInput.Length - 1);
			OnInputChanged?.Invoke(currentInput);
		}
	}

	public void Submit()
	{
		if (isActive)
			return;

		if (string.Equals(currentInput, expectedPassword, StringComparison.OrdinalIgnoreCase))
		{
			Activate();
			OnPasswordCorrect?.Invoke();
		}
		else
		{
			OnPasswordWrong?.Invoke();
			ClearInput();
		}
	}

	public void ClearInput()
	{
		currentInput = "";
		OnInputChanged?.Invoke(currentInput);
	}
}
