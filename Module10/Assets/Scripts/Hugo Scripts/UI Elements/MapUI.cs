using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUI : MonoBehaviour
{
    public Texture2D mapMask;

    private GameObject player;

    private Vector2 centre;

    [SerializeField]
    private int revealRadius = 5;

    private CanvasGroup cg;

    [SerializeField]
    

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

        cg = gameObject.GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) && !CustomInputField.AnyFieldSelected)
        {
            cg.alpha = cg.alpha == 0 ? 1 : 0;

            Cursor.lockState = cg.alpha == 0 ? CursorLockMode.Locked : CursorLockMode.None;

            if (cg.alpha == 1)
            {
                cg.blocksRaycasts = true;
                cg.interactable = true;
                mapMask.Apply();

                player.GetComponent<PlayerMovement>().StopMoving();
            }
            else
            {
                cg.blocksRaycasts = false;
                cg.interactable = false;

                player.GetComponent<PlayerMovement>().StartMoving();
            }
        }

        Vector2 playerPos = new Vector2(player.transform.position.x * 2, player.transform.position.z * 2);
        Color color = Color.clear;

        for (int y = (int)playerPos.y - revealRadius; y < (int)playerPos.y + revealRadius; y++)
        {
            for (int x = (int)playerPos.x - revealRadius; x < (int)playerPos.x + revealRadius; x++)
            {
                if (mapMask.GetPixel(x, y).a != 0 && Vector3.Distance(playerPos, new Vector3(x, y)) <= (float)revealRadius)
                {
                    mapMask.SetPixel((int)centre.x + x, (int)centre.y + y, color);
                }  
            }
        }

    }
}
