using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest data", menuName = "Quests/Objectives/Go To objective", order = 1)]
[System.Serializable]
public class GoToQuestObjective : QuestObjective
{
    [Tooltip("Position player has to be in / near to complete")]
    public Vector3 positionToGoTo;

    [Tooltip("Distance from player to positionToGoTo to complete")]
    [SerializeField]
    private float distanceFlagged = 2.5f;

    public override bool checkCcompleted()
    {
        objectiveType = Type.GoTo;

        return (Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, positionToGoTo) < distanceFlagged);
    }
}
