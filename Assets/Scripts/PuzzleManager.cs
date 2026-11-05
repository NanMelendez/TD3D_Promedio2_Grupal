using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents
{
    public static event Action OnPlayerDeath;
    public static void PlayerDied() => OnPlayerDeath?.Invoke();
}

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance { get; private set; }

    // salaId -> índice del puzzle elegido
    readonly Dictionary<string, int> asignaciones = new();

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void OnEnable() => GameEvents.OnPlayerDeath += Reiniciar;
    void OnDisable() => GameEvents.OnPlayerDeath -= Reiniciar;

    void Reiniciar() => asignaciones.Clear();

    public int ObtenerIndice(string salaId, int cantidad)
    {
        if (!asignaciones.TryGetValue(salaId, out int indice))
        {
            indice = UnityEngine.Random.Range(0, cantidad);
            asignaciones[salaId] = indice;
        }
        return indice;
    }
}