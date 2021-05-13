using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum OptionsOpenType
{
    GameScenePause,
    MainMenu
}

public class OptionsPanel : MonoBehaviour
{
    private OptionsOpenType openType;

    [SerializeField] private TextMeshProUGUI musicVolumeText;
    [SerializeField] private TextMeshProUGUI soundEffectsVolumeText;

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundEffectsSlider;

    [SerializeField] private OptionsToggle screenShakeToggle;
    [SerializeField] private OptionsToggle viewBobbingToggle;

    private void Start()
    {
        screenShakeToggle.ToggleEvent += ScreenShakeToggled;
        viewBobbingToggle.ToggleEvent += ViewBobbingToggled;
    }

    public void Setup(OptionsOpenType openType)
    {
        this.openType = openType;

        SaveLoadManager slm = SaveLoadManager.Instance;

        UpdateMusicVolumeUI       (slm.GetIntFromPlayerPrefs("musicVolume"));
        UpdateSoundEffectsVolumeUI(slm.GetIntFromPlayerPrefs("soundEffectsVolume"));

        screenShakeToggle.SetSelected(slm.GetBoolFromPlayerPrefs("screenShake"));
        viewBobbingToggle.SetSelected(slm.GetBoolFromPlayerPrefs("viewBobbing"));
    }

    private void UpdateMusicVolumeUI(int savedMusicVolume)
    {
        musicSlider.value = savedMusicVolume;
        musicVolumeText.text = "Music Volume (" + (savedMusicVolume * 5) + "%)";

        AudioManager.Instance.UpdateAudioSourcesVolume(savedMusicVolume);
    }

    private void UpdateSoundEffectsVolumeUI(int savedSoundEffectsVolume)
    {
        soundEffectsSlider.value = savedSoundEffectsVolume;
        soundEffectsVolumeText.text = "Sound Effects Volume (" + (savedSoundEffectsVolume * 5) + "%)";

        AudioManager.Instance.UpdateActiveLoopingSoundsVolume(savedSoundEffectsVolume);
    }

    private void ScreenShakeToggled(bool enabled)
    {
        SaveLoadManager.Instance.SaveBoolToPlayerPrefs("screenShake", enabled);

        AudioManager.Instance.PlaySoundEffect2D("buttonClickMain1");
    }

    private void ViewBobbingToggled(bool enabled)
    {
        SaveLoadManager.Instance.SaveBoolToPlayerPrefs("viewBobbing", enabled);

        AudioManager.Instance.PlaySoundEffect2D("buttonClickMain1");
    }

    // Called by UI events:
    //=====================

    public void ButtonReturn()
    {
        if(openType == OptionsOpenType.GameScenePause)
        {
            GameSceneUI.Instance.HideOptionsUI();
            GameSceneUI.Instance.PauseAndShowPauseUI();
        }
    }

    public void MusicSliderValueChanged(float value)
    {
        int valueAsInt = (int)value;

        SaveLoadManager.Instance.SaveIntToPlayerPrefs("musicVolume", valueAsInt);

        UpdateMusicVolumeUI(valueAsInt);

        AudioManager.Instance.PlaySoundEffect2D("buttonClickMain1");
    }

    public void SoundEffectsSliderValueChanged(float value)
    {
        int valueAsInt = (int)value;

        SaveLoadManager.Instance.SaveIntToPlayerPrefs("soundEffectsVolume", valueAsInt);

        UpdateSoundEffectsVolumeUI(valueAsInt);

        AudioManager.Instance.PlaySoundEffect2D("buttonClickMain1");
    }
}
