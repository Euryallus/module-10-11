using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopLeaves : MonoBehaviour
{
    public Transform leavesPos;  //point where leaves should appear when hit (visuals)
    private void OnCollisionEnter(Collision collision)
    {
        //causes leaves to shake out when hits other objects
        ParticleManager.Instance.SpawnParticle(leavesPos.position, "Trees");
    }
}
