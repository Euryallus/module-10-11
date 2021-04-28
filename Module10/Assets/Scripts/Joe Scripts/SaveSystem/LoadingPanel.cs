using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingPanel : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    private bool loadDone;

    private void Update()
    {
        if(loadDone)
        {
            canvasGroup.alpha -= Time.unscaledDeltaTime * 3.0f;

            if(canvasGroup.alpha <= 0.0f)
            {
                Destroy(gameObject);
            }
        }
    }

    public void LoadDone()
    {
        loadDone = true;
    }
}
