using UnityEngine;

public class PuzzleRoom : MonoBehaviour
{
    [SerializeField] string salaId;
    [SerializeField] PuzzlePool pool;
    [SerializeField] Transform puntoSpawn;

    GameObject actual;

    void Start()
    {
        int i = PuzzleManager.Instance.ObtenerIndice(salaId, pool.prefabs.Length);
        actual = Instantiate(pool.prefabs[i], puntoSpawn.position, puntoSpawn.rotation, transform);
    }

    void OnDestroy()
    {
        if (actual != null) Destroy(actual);
    }
}