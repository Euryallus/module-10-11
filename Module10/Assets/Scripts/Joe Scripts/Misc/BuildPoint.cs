using UnityEngine;

public enum BuildPointType
{
    Floor,
    Wall,
    Stairs,
    RoofSide
}

public class BuildPoint : MonoBehaviour
{
    public BuildPointType BuildPointType { get { return buildPointType; } }

    [SerializeField] private BuildPointType buildPointType;
}
