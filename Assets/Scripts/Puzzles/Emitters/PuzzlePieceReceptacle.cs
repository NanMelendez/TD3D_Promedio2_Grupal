using System;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePieceReceptacle : PuzzleComponent, IPuzzleProgressProvider, IInteractable
{
	[SerializeField] private Transform piecesBase;
	[SerializeField] private GameObject pieceInstance;
	[SerializeField] private GameObject fakePieceInstance = null;
	[SerializeField][Range(0, 100)] private int fakePieceSpawnChance = 40;
	[SerializeField] private PuzzlePickupType expectedType;
	[SerializeField] private string navmeshAreaName;

	readonly private List<GameObject> piecesGOs = new();
	private int collectedCount = 0;

	public event Action<int, int> OnProgressChanged;
	public int MaxProgress => piecesGOs.Count;
	public int CurrentProgress => collectedCount;

	private void Awake()
	{
		foreach (Transform child in piecesBase)
		{
			child.gameObject.SetActive(false);
			piecesGOs.Add(child.gameObject);
		}
	}

	private void Start()
	{
		PuzzlePickupSpreadManager.Instance.SpawnPieces(navmeshAreaName, piecesGOs.Count, expectedType, pieceInstance, fakePieceInstance, fakePieceSpawnChance);
	}

	public void Interact()
	{
		PlayerPickupCollector playerCollector = FindFirstObjectByType<PlayerPickupCollector>();
		PuzzlePickupData heldPiece = playerCollector.HeldPiece;

		if (collectedCount >= MaxProgress)
		{
			Debug.Log("Se entregaron todas las piezas");
			if (heldPiece.isFake)
				playerCollector.Consume();
			return;
		}

		if (!playerCollector)
		{
			Debug.Log("No existe componente recolector en el jugador");
			return;
		}

		if (!playerCollector.HoldingPiece)
		{
			Debug.Log("No tienes ninguna pieza para entregar aquí");
			return;
		}
		if (heldPiece.type != expectedType)
		{
			Debug.Log("La pieza no encaja aquí");
			return;
		}

		if (heldPiece.isFake)
		{
			Debug.Log("Ups, la pieza es falsa. Descartando...");
			playerCollector.Consume();
			return;
		}

		playerCollector.Consume();
		if (collectedCount < MaxProgress)
		{
			piecesGOs[collectedCount].SetActive(true);
			collectedCount++;
			OnProgressChanged?.Invoke(CurrentProgress, MaxProgress);

			if (collectedCount == MaxProgress)
				Activate();
		}
	}
}
