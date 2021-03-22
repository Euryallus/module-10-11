using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIPanel : MonoBehaviour
{
    public bool Showing { get { return showing; } }

    protected   bool        showing;        //Whether or not the panel is currently showing
    private     CanvasGroup canvasGroup;    //CanvasGroup attathed to the panel

    protected virtual void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public virtual void Show()
    {
        canvasGroup.alpha = 1.0f;
        showing = true;
    }

    public virtual void Hide()
    {
        canvasGroup.alpha = 0.0f;
        showing = false;
    }
}
