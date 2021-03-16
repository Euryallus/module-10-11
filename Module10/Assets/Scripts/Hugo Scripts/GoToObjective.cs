using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToObjective : Objective
{
    [SerializeField]
    private Vector3 positionToGoTo;

    public override bool checkComplete()
    {
        return (Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, positionToGoTo) < 2.0f);

    }
}
