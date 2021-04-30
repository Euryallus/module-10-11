using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldSignpost : HeldPlaceableItem
{
    protected override GameObject PlaceItem()
    {
        GameObject signGameObj = base.PlaceItem();

        Signpost signpostScript = signGameObj.GetComponent<Signpost>();

        signpostScript.SetRelatedItem(item.Id);

        return signGameObj;
    }
}
