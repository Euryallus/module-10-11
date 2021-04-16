using UnityEngine;

public interface ISavePoint
{
    public abstract string GetSavePointId();

    public abstract Vector3 GetRespawnPosition();
}
