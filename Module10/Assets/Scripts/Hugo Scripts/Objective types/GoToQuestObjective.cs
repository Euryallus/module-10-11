using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest data", menuName = "Quests/Objectives/Go To objective", order = 1)]
public class GoToQuestObjective : QuestObjective
{
    public Vector3 positionToGoTo;

    [SerializeField]
    private float distanceFlagged = 2.5f;

    public override bool checkCcompleted()
    {
        objectiveType = type.GoTo;

        return (Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, positionToGoTo) < distanceFlagged);
    }
}
