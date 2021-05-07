using UnityEngine;

// ||=======================================================================||
// || UIPanel: A base class for UI panels that can be shown/hidden          ||
// ||=======================================================================||
// || Written by Joseph Allen                                               ||
// || for the prototype phase.                                              ||
// ||=======================================================================||

[RequireComponent(typeof(CanvasGroup))]
public class UIPanel : MonoBehaviour
{
    #region Properties

    public bool Showing { get { return showing; } }

    #endregion

    protected   bool        showing;        // Whether or not the panel is currently showing
    private     CanvasGroup canvasGroup;    // CanvasGroup attathed to the panel

    protected virtual void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public virtual void Show()
    {
        // Set the canvas group alpha to 1 to fully show the panel
        canvasGroup.alpha = 1.0f;

        // Allow the panel to block raycasts and hence prevent UI behind it from being interacted with
        canvasGroup.blocksRaycasts = true;

        // The panel is showing
        showing = true;
    }

    public virtual void Hide()
    {
        // Set the canvas group alpha to 0 to fully hide the panel
        canvasGroup.alpha = 0.0f;

        // Stop the panel from blocking raycasts and hence allow UI behind it to be interacted with
        canvasGroup.blocksRaycasts = false;

        // The panel is hidden
        showing = false;
    }
}
