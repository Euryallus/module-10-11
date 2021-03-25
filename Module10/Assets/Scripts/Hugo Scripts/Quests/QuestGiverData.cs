using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest data", menuName = "NPCs/NPC quest data", order = 1)]
public class QuestGiverData : ScriptableObject
{
    public List<QuestData> questsToGive = new List<QuestData>();

    [SerializeField]
    public string QuestGiverName;
}
