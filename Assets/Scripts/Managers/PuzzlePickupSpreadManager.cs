using UnityEngine;
using UnityEngine.AI;

public class PuzzlePickupSpreadManager : MonoBehaviour
{
	[SerializeField] private Transform whereToAnchor;
	[SerializeField] private int maxFakes = 10;

	private Vector3 navMeshMinBounds;
	private Vector3 navMeshMaxBounds;

	public static PuzzlePickupSpreadManager Instance
	{
		get;
		private set;
	}

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;

		NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();
		foreach (Vector3 vertex in navMeshData.vertices)
		{
			navMeshMinBounds = Vector3.Min(navMeshMinBounds, vertex);
			navMeshMaxBounds = Vector3.Max(navMeshMaxBounds, vertex);
		}
	}

	public void SpawnPieces(string navmeshAreaName, int n, PuzzlePickupType collectibleType, GameObject instance, GameObject fakeInstance = null, int fakeSpawnChance = 10)
	{
		int fakeCount = 0;

		NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();
		int roomMask = 1 << NavMesh.GetAreaFromName(navmeshAreaName);

		if (triangulation.vertices == null || triangulation.vertices.Length == 0)
			return;

		int spawnedCount = 0;

		while (spawnedCount < n)
		{
			float randomX = Random.Range(navMeshMinBounds.x, navMeshMaxBounds.x);
			float randomZ = Random.Range(navMeshMinBounds.z, navMeshMaxBounds.z);

			float spawnY = navMeshMaxBounds.y + 2.0f;
			Vector3 randomPoint = new(randomX, spawnY, randomZ);

			float searchRadius = (navMeshMaxBounds.y - navMeshMinBounds.y) + 5.0f;

			if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, searchRadius, roomMask))
			{
				Vector3 final = hit.position + Vector3.up * 0.5f;
				if (collectibleType != PuzzlePickupType.Key)
				{
					Instantiate(instance, final, Quaternion.identity, whereToAnchor);
					spawnedCount++;
				}
				else
				{
					int rng = Random.Range(0, 100);
					GameObject toSpawnInstance;
					if (rng < fakeSpawnChance && fakeCount < maxFakes)
					{
						toSpawnInstance = fakeInstance;
						fakeCount++;
					}
					else
					{
						toSpawnInstance = instance;
						spawnedCount++;
					}

					Instantiate(toSpawnInstance, final, Quaternion.identity, whereToAnchor);
				}
			}
		}
	}
}
