using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldSignpost : HeldPlaceableItem
{
    protected override GameObject PlaceItem()
    {
        GameObject signGameObj = base.PlaceItem();

        Signpost signpostScript = signGameObj.GetComponent<Signpost>();

        signpostScript.SetupAsPlacedObject(item.Id);

        HotbarPanel hotbar = GameObject.FindGameObjectWithTag("Hotbar").GetComponent<HotbarPanel>();

        hotbar.RemoveItemFromHotbar(item);

        return signGameObj;
    }
}
