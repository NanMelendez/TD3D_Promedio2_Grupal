using UnityEngine;

public class DoorToggle : MonoBehaviour
{
	[SerializeField] private GameObject doorMesh;

	public void Activate()
	{
		doorMesh.SetActive(false);
	}

	public void Deactivate()
	{
		doorMesh.SetActive(true);
	}
}
