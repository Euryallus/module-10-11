using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player data", menuName = "Player/Player quest data", order = 1)]
public class PlayerQuestBacklog : ScriptableObject
{
    public List<QuestData> questBacklog = new List<QuestData>();
    public List<QuestData> completedQuests = new List<QuestData>();

    public QuestData pendingQuest = null;
    public QuestGiverData offer = null;

    public List<QuestGiverData> questGivers;
}
