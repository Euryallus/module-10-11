using UnityEngine;

public enum BuildPointType
{
    Floor,
    Wall
}

public class BuildPoint : MonoBehaviour
{
    public BuildPointType BuildPointType { get { return buildPointType; } }

    [SerializeField] private BuildPointType buildPointType;
}
