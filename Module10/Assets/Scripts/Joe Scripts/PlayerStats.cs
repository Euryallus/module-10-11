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

    [SerializeField] [Tooltip("How much the player's health decreases by every [starveDamageInterval] seconds when starving.")]
    private float starveDamage = 0.05f;

    [SerializeField] [Tooltip("How frequently (in seconds) the player takes damage when starving.")]
    private float starveDamageInterval = 2.0f;

    [Header("UI")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image  healthSliderFill;
    [SerializeField] private Slider foodLevelSlider;
    [SerializeField] private Image  foodSliderFill;

    private float           health                  = 1.0f;
    private float           foodLevel               = 1.0f;

    private float           starveDamageTimer;
    private PlayerMovement  playerMovementScript;
    private Animator        healthSliderAnimator;
    private Animator        foodSliderAnimator;

    private void Awake()
    {
        playerMovementScript = GetComponent<PlayerMovement>();

        healthSliderAnimator = healthSliderFill .gameObject.GetComponent<Animator>();
        foodSliderAnimator   = foodSliderFill   .gameObject.GetComponent<Animator>();
    }

    protected void Start()
    {
        SaveLoadManager slm = SaveLoadManager.Instance;
        slm.SaveObjectsEvent            += OnSave;
        slm.LoadObjectsSetupEvent       += OnLoadSetup;
        slm.LoadObjectsConfigureEvent   += OnLoadConfigure;
    }

    private void Update()
    {
        float foodLevelDecreaseAmount = Time.deltaTime / baseTimeToStarve;

        if(foodLevel > 0.0f)
        {
            //Decrease food level depending on player state
            if (playerMovementScript.PlayerIsMoving())
            {
                if (playerMovementScript.GetCurrentMovementState() == PlayerMovement.MovementStates.run)
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
        else
        {
            //Player is starving
            starveDamageTimer += Time.deltaTime;
            if(starveDamageTimer >= starveDamageInterval)
            {
                DecreaseHealth(starveDamage);

                starveDamageTimer = 0.0f;
            }
        }

        UpdateHealthUI();
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

    public void IncreaseHealth(float amount)
    {
        health += amount;

        health = Mathf.Clamp(health, 0.0f, 1.0f);
    }

    public void DecreaseHealth(float amount)
    {
        health -= amount;

        if (health < 0.0f)
        {
            health = 0.0f;
        }
    }

    private void UpdateFoodLevelUI()
    {
        foodLevelSlider.value = Mathf.Lerp(foodLevelSlider.value, foodLevel, Time.deltaTime * 25.0f);

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
        healthSlider.value = Mathf.Lerp(healthSlider.value, health, Time.deltaTime * 25.0f);

        if (health < 0.1f)
        {
            healthSliderAnimator.SetBool("Flash", true);
        }
        else
        {
            healthSliderAnimator.SetBool("Flash", false);
        }
    }
}
