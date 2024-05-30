using UnityEngine;

[CreateAssetMenu(fileName = "Create New Level")]
public class SO_LevelData : ScriptableObject
{
    public int levelID;
    public int tileAmount; // have to : 3, 6, 9, ..., 3k
    public float timeAmount;
}
