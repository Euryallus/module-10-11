using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "'Go To' quest objective", menuName = "ScriptableObjects/Quest Shit/'Go To' quest objective", order = 1)]
public class GoToQuestObjective : QuestObjective
{
    public Vector3 positionToGoTo;

    public override bool checkCcompleted()
    {
        objectiveType = type.GoTo;
        //Debug.Log("Checked!");
        //Debug.Log(Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, positionToGoTo));

        return (Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, positionToGoTo) < 2.5f);
    }
}
