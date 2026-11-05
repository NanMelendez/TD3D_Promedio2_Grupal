using TMPro;
using UnityEngine;

public class PuzzlePasswordUI : MonoBehaviour
{
	[SerializeField] private PuzzlePassword receiver;
	[SerializeField] private TextMeshProUGUI TmpText;

	private void Awake()
	{
		receiver.OnInputChanged += UpdateText;
	}

	private void OnDestroy()
	{
		receiver.OnInputChanged -= UpdateText;
	}

	private void UpdateText(string str)
	{
		TmpText.text = str.Length > 0 ? str : new string('#', receiver.ExpectedPassword.Length);
	}
}
