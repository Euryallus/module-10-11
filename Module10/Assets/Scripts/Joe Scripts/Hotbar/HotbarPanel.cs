using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotbarPanel : UIPanel
{
    [SerializeField] private List<ContainerSlotUI>  slotsUI;
    [SerializeField] private ItemContainer          itemContainer;

    protected override void Start()
    {
        base.Start();

        itemContainer.LinkSlotsToUI(slotsUI);

        Show();
    }
}
