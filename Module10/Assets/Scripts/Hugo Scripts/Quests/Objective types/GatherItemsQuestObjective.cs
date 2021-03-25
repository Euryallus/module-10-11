using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Quest data", menuName = "Quests/Objectives/Gather Items objective", order = 2)]
[System.Serializable]
public class GatherItemsQuestObjective : QuestObjective
{
    [SerializeField]
    List<ContainerSlot> containerSlots;

    public ItemGroup toCollect;
    public override bool checkCcompleted()
    {
        objectiveType = Type.Collect;

        InventoryPanel inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<InventoryPanel>();

        if(inventory.ItemContainer.ContainsQuantityOfItem(toCollect, out containerSlots))
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }
}
