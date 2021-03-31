using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUI : MonoBehaviour
{
    public Texture2D mapMask;

    private GameObject player;

    private Vector2 centre;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        for (int y = 0; y < mapMask.height; y++)
        {
            for (int x = 0; x < mapMask.width; x++)
            {
                Color color = new Color(0, 0, 0, 1);
                mapMask.SetPixel(x, y, color);
            }
        }
        mapMask.Apply();
        centre = new Vector2(mapMask.width / 2, mapMask.height / 2);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            gameObject.GetComponent<CanvasGroup>().alpha = gameObject.GetComponent<CanvasGroup>().alpha == 0 ? 1 : 0;

            if(gameObject.GetComponent<CanvasGroup>().alpha == 1)
            {
                mapMask.Apply();
            }
        }

        Vector2 playerPos = new Vector2(player.transform.position.x * 2, player.transform.position.z * 2);

        for (int y = (int)playerPos.y - 20; y < (int)playerPos.y + 20; y++)
        {
            for (int x = (int)playerPos.x - 20; x < (int)playerPos.x + 20; x++)
            {
                Color color = Color.clear;
                mapMask.SetPixel((int)centre.x + x, (int)centre.y + y, color);
            }
        }
    }
}
