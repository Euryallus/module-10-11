using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player data", menuName = "Player/Player quest data", order = 1)]
public class PlayerQuestBacklog : ScriptableObject
{
    [Header("Quest lists (completed and in-progress)")]
    public List<QuestData> questBacklog = new List<QuestData>();
    public List<QuestData> completedQuests = new List<QuestData>();

    [HideInInspector]
    public QuestData pendingQuest = null;
    [HideInInspector]
    public QuestGiverData offer = null;

    [Header("NPCData obj.s must be added here!")]
    public List<QuestGiverData> questGivers;
}
