using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour, IPersistentObject
{
    [SerializeField] private Slider foodLevelSlider;

    private float foodLevel = 1.0f;

    protected void Start()
    {
        SaveLoadManager slm = SaveLoadManager.Instance;
        slm.SaveObjectsEvent            += OnSave;
        slm.LoadObjectsSetupEvent       += OnLoadSetup;
        slm.LoadObjectsConfigureEvent   += OnLoadConfigure;

        UpdateFoodLevelUI();
    }

    private void OnDestroy()
    {
        SaveLoadManager slm = SaveLoadManager.Instance;
        slm.SaveObjectsEvent            -= OnSave;
        slm.LoadObjectsSetupEvent       -= OnLoadSetup;
        slm.LoadObjectsConfigureEvent   -= OnLoadConfigure;
    }

    public void OnSave(SaveData saveData)
    {
        saveData.AddData("playerFoodLevel", foodLevel);
    }

    public void OnLoadSetup(SaveData saveData)
    {
        float loadedFoodLevel = saveData.GetData<float>("playerFoodLevel", out bool foodLevelLoadSuccess);

        if (foodLevelLoadSuccess)
        {
            foodLevel = loadedFoodLevel;
        }

        UpdateFoodLevelUI();
    }

    public void OnLoadConfigure(SaveData saveData)
    {
    }

    public void IncreaseFoodLevel(float amount)
    {
        foodLevel += amount;

        foodLevel = Mathf.Clamp(foodLevel, 0.0f, 1.0f);

        UpdateFoodLevelUI();
    }

    public void DecreaseFoodLevel(float amount)
    {
        foodLevel -= amount;

        if(foodLevel < 0.0f)
        {
            foodLevel = 0.0f;
        }

        UpdateFoodLevelUI();
    }

    private void UpdateFoodLevelUI()
    {
        foodLevelSlider.value = foodLevel;
    }
}
