using UnityEngine;

public class PausePanel : MonoBehaviour
{
    public void ButtonOptions()
    {
        GameSceneUI.Instance.ShowOptionsUI();

        GameSceneUI.Instance.HidePauseUI(false);
    }

    public void ButtonResume()
    {
        GameSceneUI.Instance.HidePauseUI();
    }
}
