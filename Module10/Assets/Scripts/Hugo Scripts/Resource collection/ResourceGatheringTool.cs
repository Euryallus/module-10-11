using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGatheringTool : MonoBehaviour
{
    public CollectableResource attachedResource;
    public string swingSoundName = "whoosh";
    public string hitSoundName;
    public ParticleGroup particle;

    public void StopSwinging()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().StartMoving();

        attachedResource.toolRenderer.enabled = true;
        attachedResource.canBeHit = true;
    }

    public void SwingEvents()
    {
        AudioManager.Instance.PlaySoundEffect3D(swingSoundName, transform.position);
    }

    public void ChopEvents()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<CameraShake>().ShakeCameraForTime(0.3f, CameraShakeType.ReduceOverTime, 0.05f);

        AudioManager.Instance.PlaySoundEffect3D(hitSoundName, transform.position);

        particle.PlayEffect();
        attachedResource.TryToDestroy();
    }
}
