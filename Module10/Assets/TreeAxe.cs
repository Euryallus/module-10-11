using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeAxe : MonoBehaviour
{
    public NewTree tree;
    public void StopSwinging()
    {
        gameObject.GetComponent<Animator>().SetBool("Swing", false);

        tree.CanBeHit = true;
    }
}
