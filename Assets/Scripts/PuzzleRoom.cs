using System.Collections.Generic;
using UnityEngine;

public class PuzzleRoom : MonoBehaviour
{
    [SerializeField] string salaId;
    [SerializeField] int orden;
    [SerializeField] PuzzlePool pool;
    [SerializeField] Transform[] puntosSpawn;

    readonly List<GameObject> actuales = new();

    public int Dificultad { get; private set; }

    public int CantidadPuzzles => actuales.Count;

    void Start()
    {
        if (pool == null || pool.puzzles == null || pool.puzzles.Length == 0)
        {
            Debug.LogWarning($"[PuzzleRoom] La sala '{salaId}' no tiene puzzles en su pool.", this);
            return;
        }
        if (puntosSpawn == null || puntosSpawn.Length == 0)
        {
            Debug.LogWarning($"[PuzzleRoom] La sala '{salaId}' no tiene puntos de spawn.", this);
            return;
        }

        PuzzleManager.Asignacion asignacion =
            PuzzleManager.Instance.Obtener(salaId, orden, pool, puntosSpawn.Length);
        Dificultad = asignacion.dificultad;

        for (int k = 0; k < asignacion.indices.Length; k++)
        {
            Transform punto = puntosSpawn[k];
            GameObject prefab = pool.puzzles[asignacion.indices[k]].prefab;
            actuales.Add(Instantiate(prefab, punto.position, punto.rotation, transform));
        }
    }

    void OnDestroy()
    {
        foreach (GameObject p in actuales)
            if (p != null) Destroy(p);
    }
}