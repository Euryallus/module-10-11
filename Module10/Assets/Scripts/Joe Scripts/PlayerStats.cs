using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerStats : MonoBehaviour, IPersistentObject
{
    [Header("Hunger Options (See tooltips for info)")]

    [SerializeField] [Tooltip("The number of seconds it will take for the player to starve if they are idle.")]
    private float  baseTimeToStarve        = 600.0f;

    [SerializeField] [Tooltip("How many times quicker the player's food level will decrease when they are walking/crouching.")]
    private float  walkHungerMultiplier    = 1.25f;

    [SerializeField] [Tooltip("How many times quicker the player's food level will decrease when they are running.")]
    private float  runHungerMultiplier     = 1.5f;

    [Header("UI")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider foodLevelSlider;
    [SerializeField] private Image  foodSliderFill;

    private float           health                  = 1.0f;
    private float           foodLevel               = 1.0f;
    private PlayerMovement  playerMovementScript;
    private Animator        foodSliderAnimator;

    private void Awake()
    {
        playerMovementScript = GetComponent<PlayerMovement>();
        foodSliderAnimator = foodSliderFill.gameObject.GetComponent<Animator>();
    }

    protected void Start()
    {
        SaveLoadManager slm = SaveLoadManager.Instance;
        slm.SaveObjectsEvent            += OnSave;
        slm.LoadObjectsSetupEvent       += OnLoadSetup;
        slm.LoadObjectsConfigureEvent   += OnLoadConfigure;

        UpdateHealthUI();
    }

    private void Update()
    {
        UpdateFoodLevelUI();

        float foodLevelDecreaseAmount = Time.deltaTime / baseTimeToStarve;

        if (playerMovementScript.PlayerIsMoving())
        {
            if(playerMovementScript.GetCurrentMovementState() == PlayerMovement.MovementStates.run)
            {
                foodLevelDecreaseAmount *= runHungerMultiplier;
            }
            else
            {
                foodLevelDecreaseAmount *= walkHungerMultiplier;
            }
        }

        DecreaseFoodLevel(foodLevelDecreaseAmount);
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

        saveData.AddData("playerHealth", health);
    }

    public void OnLoadSetup(SaveData saveData)
    {
        float loadedFoodLevel = saveData.GetData<float>("playerFoodLevel", out bool foodLevelLoadSuccess);
        if (foodLevelLoadSuccess)
        {
            foodLevel = loadedFoodLevel;
        }

        float loadedHealth = saveData.GetData<float>("playerHealth", out bool healthlLoadSuccess);
        if (healthlLoadSuccess)
        {
            health = loadedHealth;
        }

        UpdateHealthUI();
    }

    public void OnLoadConfigure(SaveData saveData)
    {
    }

    public void IncreaseFoodLevel(float amount)
    {
        foodLevel += amount;

        foodLevel = Mathf.Clamp(foodLevel, 0.0f, 1.0f);
    }

    public void DecreaseFoodLevel(float amount)
    {
        foodLevel -= amount;

        if(foodLevel < 0.0f)
        {
            foodLevel = 0.0f;
        }
    }

    private void UpdateFoodLevelUI()
    {
        foodLevelSlider.value = foodLevel;

        if(foodLevel < 0.1f)
        {
            foodSliderAnimator.SetBool("Flash", true);
        }
        else
        {
            foodSliderAnimator.SetBool("Flash", false);
        }
    }

    private void UpdateHealthUI()
    {
        healthSlider.value = health;
    }
}
