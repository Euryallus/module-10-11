using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldSignpost : HeldPlaceableItem
{
    protected override GameObject PlaceItemPrefab()
    {
        GameObject signGameObj = base.PlaceItemPrefab();

        Signpost signpostScript = signGameObj.GetComponent<Signpost>();

        signpostScript.RelatedItemId = item.Id;

        signpostScript.SetSignText( item.GetCustomStringPropertyWithName("line1").Value,
                                    item.GetCustomStringPropertyWithName("line2").Value,
                                    item.GetCustomStringPropertyWithName("line3").Value,
                                    item.GetCustomStringPropertyWithName("line4").Value);

        return signGameObj;
    }
}
