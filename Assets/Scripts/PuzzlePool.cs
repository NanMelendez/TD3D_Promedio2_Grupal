using System;
using UnityEngine;

[Serializable]
public class PuzzleEntry
{
    public GameObject prefab;

    [Range(PuzzlePool.DificultadMin, PuzzlePool.DificultadMax)]
    public int dificultad = 1;
}

[CreateAssetMenu(menuName = "Puzzles/Pool")]
public class PuzzlePool : ScriptableObject
{
    public const int DificultadMin = 1;
    public const int DificultadMax = 10;

    public PuzzleEntry[] puzzles;

    public static float Normalizar(int dificultad)
    {
        return (dificultad - DificultadMin) / (float)(DificultadMax - DificultadMin);
    }
}