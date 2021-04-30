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
    [SerializeField] private Collider       buildPointCollider;
    
    public void SetColliderEnabled(bool colliderEnabled)
    {
        buildPointCollider.enabled = colliderEnabled;
    }

    private void Start()
    {
        WorldSave.Instance.AddPlacedBuildPoint(this);
    }

    private void OnDestroy()
    {
        WorldSave.Instance.RemovePlacedBuildPoint(this);
    }
}
