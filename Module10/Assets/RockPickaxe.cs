using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockPickaxe : MonoBehaviour
{
    public Rock rock;
    public void StopSwinging()
    {
        gameObject.GetComponent<Animator>().SetBool("Swing", false);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().StartMoving();
        GameObject.FindGameObjectWithTag("Pickaxe").GetComponent<MeshRenderer>().enabled = true;
        rock.CanBeHit = true;
    }

    //Added by Joe - plays a sound when the axe is swung, called by an animation event
    public void SwingEvents()
    {
        AudioManager.Instance.PlaySoundEffect3D("whoosh", transform.position);
    }

    //Added by Joe - shakes the player camera and plays a sound when the axe hits the tree, called by an animation event
    public void ChopEvents()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<CameraShake>().ShakeCameraForTime(0.3f, CameraShakeType.ReduceOverTime, 0.03f);

        AudioManager.Instance.PlaySoundEffect3D("treeChop", transform.position);
    }
}
