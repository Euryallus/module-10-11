using UnityEngine;

// ||=======================================================================||
// || PausePanel: UI panel shown when the game is paused.                   ||
// ||=======================================================================||
// || Used on prefab: Joe/UI/PausePanel                                     ||
// ||=======================================================================||
// || Written by Joseph Allen                                               ||
// || for the prototype phase.                                              ||
// ||=======================================================================||

public class PausePanel : MonoBehaviour
{
    public void ButtonOptions()
    {
        // Show the options menu UI
        GameSceneMenuUI.Instance.ShowOptionsUI();

        // Hide this pause panel
        GameSceneMenuUI.Instance.HidePauseUI(false);
    }

    public void ButtonResume()
    {
        // Hide this pause panel to return to gameplay

        GameSceneMenuUI.Instance.HidePauseUI();
    }
}
