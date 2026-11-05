using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents
{
    public static event Action OnPlayerDeath;
    public static void PlayerDied() => OnPlayerDeath?.Invoke();
}

public enum NivelDificultad { Facil, Medio, Dificil, MuyDificil }

[Serializable]
public class PasoSecuencia
{
    public string nombre;
    public NivelDificultad[] puzzles;
}

public class PuzzleManager : MonoBehaviour
{
    public readonly struct Asignacion
    {
        public readonly int[] indices;
        public readonly int dificultad;
        public Asignacion(int[] indices, int dificultad)
        {
            this.indices = indices;
            this.dificultad = dificultad;
        }
    }

    public static PuzzleManager Instance { get; private set; }

    [SerializeField] Vector2Int rangoFacil = new(1, 3);
    [SerializeField] Vector2Int rangoMedio = new(4, 6);
    [SerializeField] Vector2Int rangoDificil = new(7, 8);
    [SerializeField] Vector2Int rangoMuyDificil = new(9, 10);

    [Header("Secuencia de salas")]
    [SerializeField]
    List<PasoSecuencia> secuencia = new()
    {
        Paso("Fácil", NivelDificultad.Facil),
        Paso("Fácil", NivelDificultad.Facil),
        Paso("Fácil", NivelDificultad.Facil),
        Paso("Fácil", NivelDificultad.Facil),
        
        Paso("Combinado fácil", NivelDificultad.Facil, NivelDificultad.Facil),
        
        Paso("Difícil", NivelDificultad.Dificil),
        Paso("Difícil", NivelDificultad.Dificil),

        Paso("Combinado fácil",           NivelDificultad.Facil,   NivelDificultad.Facil),
        Paso("Difícil",                   NivelDificultad.Dificil),
        Paso("Combinado fácil + difícil", NivelDificultad.Facil,   NivelDificultad.Dificil),
        Paso("Más difícil",               NivelDificultad.MuyDificil),
        Paso("Combinado difícil",         NivelDificultad.Dificil, NivelDificultad.Dificil),
    };

    [SerializeField, Min(0)] int inicioCiclo = 7;

    [Header("Combinaciones")]
    [SerializeField, Range(0f, 0.5f)] float bonusPorPuzzleExtra = 0.15f;

    [SerializeField, Min(0)] int salasSinRepetir = 3;

    [SerializeField, Min(1)] int intentosMaximos = 30;
    readonly Dictionary<string, Asignacion> asignaciones = new();
    readonly Queue<string> recientes = new();

    static PasoSecuencia Paso(string nombre, params NivelDificultad[] puzzles) =>
        new PasoSecuencia { nombre = nombre, puzzles = puzzles };

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void OnEnable() => GameEvents.OnPlayerDeath += Reiniciar;
    void OnDisable() => GameEvents.OnPlayerDeath -= Reiniciar;

    void Reiniciar()
    {
        asignaciones.Clear();
        recientes.Clear();
    }

    public Asignacion Obtener(string salaId, int orden, PuzzlePool pool, int maxPuzzles)
    {
        if (asignaciones.TryGetValue(salaId, out Asignacion existente))
            return existente;

        var validos = new List<int>();
        for (int i = 0; i < pool.puzzles.Length; i++)
            if (pool.puzzles[i].prefab != null) validos.Add(i);

        if (validos.Count == 0)
        {
            Debug.LogWarning("[PuzzleManager] Ningún puzzle válido en el pool, se usa el índice 0.");
            return Registrar(salaId, pool, new[] { 0 }, pool.puzzles[0].dificultad);
        }

        NivelDificultad[] niveles = PasoPara(orden).puzzles;
        if (niveles == null || niveles.Length == 0) niveles = new[] { NivelDificultad.Facil };

        int cantidad = Mathf.Min(niveles.Length, Mathf.Max(1, maxPuzzles), validos.Count);
        if (cantidad < niveles.Length)
            Debug.LogWarning($"[PuzzleManager] La sala '{salaId}' (orden {orden}) pide {niveles.Length} puzzles " +
                             $"pero solo caben/hay {cantidad}.");

        int[] combinacion = null;
        for (int intento = 0; intento < intentosMaximos; intento++)
        {
            combinacion = GenerarCombinacion(pool, validos, niveles, cantidad);
            if (!EsReciente(pool, combinacion)) break;
            if (intento == intentosMaximos - 1)
                Debug.LogWarning($"[PuzzleManager] No hay combinaciones nuevas para '{salaId}'; se permite repetir.");
        }

        return Registrar(salaId, pool, combinacion, DificultadCombinada(pool, combinacion));
    }

    public int? ObtenerDificultad(string salaId)
    {
        Asignacion a;
        if (asignaciones.TryGetValue(salaId, out a))
        {
            return a.dificultad;
        }
        else
        {
            return null;
        }
    }

    PasoSecuencia PasoPara(int orden)
    {
        if (secuencia == null || secuencia.Count == 0)
        {
            Debug.LogWarning("[PuzzleManager] La secuencia está vacía.");
            return new PasoSecuencia();
        }

        orden = Mathf.Max(0, orden);
        if (orden < secuencia.Count) return secuencia[orden];

        int inicio = Mathf.Clamp(inicioCiclo, 0, secuencia.Count - 1);
        int largoCiclo = secuencia.Count - inicio;
        return secuencia[inicio + (orden - inicio) % largoCiclo];
    }

    Vector2Int Rango(NivelDificultad nivel) => nivel switch
    {
        NivelDificultad.Facil => rangoFacil,
        NivelDificultad.Medio => rangoMedio,
        NivelDificultad.Dificil => rangoDificil,
        _ => rangoMuyDificil,
    };


    Asignacion Registrar(string salaId, PuzzlePool pool, int[] indices, int dificultad)
    {
        var asignacion = new Asignacion(indices, dificultad);
        asignaciones[salaId] = asignacion;

        if (salasSinRepetir > 0)
        {
            recientes.Enqueue(Clave(pool, indices));
            while (recientes.Count > salasSinRepetir) recientes.Dequeue();
        }
        return asignacion;
    }

    int[] GenerarCombinacion(PuzzlePool pool, List<int> validos, NivelDificultad[] niveles, int cantidad)
    {
        var disponibles = new List<int>(validos);
        var elegidos = new List<int>(cantidad);

        for (int k = 0; k < cantidad && disponibles.Count > 0; k++)
        {
            int idx = ElegirEnRango(pool, disponibles, Rango(niveles[k]));
            elegidos.Add(idx);
            disponibles.Remove(idx);
        }

        elegidos.Sort();
        return elegidos.ToArray();
    }

    int ElegirEnRango(PuzzlePool pool, List<int> candidatos, Vector2Int rango)
    {
        var mejores = new List<int>();
        int mejorDistancia = int.MaxValue;

        foreach (int i in candidatos)
        {
            int d = pool.puzzles[i].dificultad;
            int distancia = d < rango.x ? rango.x - d : (d > rango.y ? d - rango.y : 0);

            if (distancia < mejorDistancia) { mejorDistancia = distancia; mejores.Clear(); }
            if (distancia == mejorDistancia) mejores.Add(i);
        }

        return mejores[UnityEngine.Random.Range(0, mejores.Count)];
    }

    int DificultadCombinada(PuzzlePool pool, int[] indices)
    {
        float suma = 0f;
        foreach (int i in indices)
        {
            suma += PuzzlePool.Normalizar(pool.puzzles[i].dificultad);
        }
        float normalizada = Mathf.Clamp01(suma / indices.Length + bonusPorPuzzleExtra * (indices.Length - 1));
        return Mathf.RoundToInt(Mathf.Lerp(PuzzlePool.DificultadMin, PuzzlePool.DificultadMax, normalizada));
    }

    bool EsReciente(PuzzlePool pool, int[] indices) => recientes.Contains(Clave(pool, indices));

    static string Clave(PuzzlePool pool, int[] indicesOrdenados)
    {
        return $"{pool.GetInstanceID()}:{string.Join(",", indicesOrdenados)}";
    }
}