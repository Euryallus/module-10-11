using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUI : MonoBehaviour
{
    public MapElement[] mapElements;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            gameObject.GetComponent<CanvasGroup>().alpha = gameObject.GetComponent<CanvasGroup>().alpha == 0 ? 1 : 0;
        }
    }

    public void UpdateMap(string uncoveredName)
    {
        foreach(MapElement element in mapElements)
        {
            if(element.ElementName == uncoveredName)
            {
                element.Uncover();
            }
        }
    }
}
