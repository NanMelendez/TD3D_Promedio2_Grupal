using UnityEngine;

[CreateAssetMenu(fileName = "PuzzlePickupData", menuName = "Scriptable Objects/PuzzlePickupData")]
public class PuzzlePickupData : ScriptableObject
{
    public PuzzlePickupType type;
    public bool isFake;
}
