using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerStats : MonoBehaviour, IPersistentObject
{
    #region InspectorVariables
    //Variables in this region are set in the inspector. See tooltips for more info.

    [Header("Hunger Options (See tooltips for info)")]

    [SerializeField] [Tooltip("The number of seconds it will take for the player to starve if they are idle.")]
    private float   baseTimeToStarve        = 600.0f;

    [SerializeField] [Tooltip("How many times quicker the player's food level will decrease when they are walking/crouching.")]
    private float   walkHungerMultiplier    = 1.25f;

    [SerializeField] [Tooltip("How many times quicker the player's food level will decrease when they are running.")]
    private float   runHungerMultiplier     = 1.5f;

    [SerializeField] [Tooltip("How full the player's food level has to be before that cannot eat any more food (1.0 = full, 0.0 = starving)")]
    private float   fullThreshold           = 0.9f;

    [SerializeField] [Tooltip("How much the player's health decreases by every [starveDamageInterval] seconds when starving.")]
    private float   starveDamage            = 0.05f;

    [SerializeField] [Tooltip("How frequently (in seconds) the player takes damage when starving.")]
    private float   starveDamageInterval    = 2.0f;

    [SerializeField]
    private float   drownDamageInterval     = 2.0f;

    [SerializeField]
    private float   drownDamage               = 0.1f;

    [SerializeField] [Tooltip("Time player takes to drown")]
    private float baseTimeToDrown           = 60.0f;

    [Header("UI")]
    [SerializeField] private Slider healthSlider;       //The slider showing health
    [SerializeField] private Image  healthSliderFill;   //The fill bar for the above slider
    [SerializeField] private Slider foodLevelSlider;    //The slider showing food level
    [SerializeField] private Image  foodSliderFill;     //The fill bar for the above slider
    [SerializeField] private Slider breathLevelSlider;  //The slider showing breath
    [SerializeField] private Image breathSliderFill;    //The fill bar for the above slider
    [SerializeField] private CanvasGroup breathCanvasGroup;
        


    #endregion

    private float           health    = 1.0f;       //The player's health (0 = death, 1 = full)
    private float           foodLevel = 1.0f;       //The player's food level (0 = starving, 1 = full)

    //Hugo added this blame him if it goes wrong
    private float           breath    = 1.0f;       //The player's breath level (0 = drowning, 1 = full)

    private float           starveDamageTimer;      //Keeps track of seconds passed since damage was taken from starving
    private float           drownDamageTimer;
    private PlayerMovement  playerMovementScript;   //Reference to the script that controls player movement
    private Animator        healthSliderAnimator;   //Animator used for flashing the health slider bar red when health is low
    private Animator        foodSliderAnimator;     //Animator used for flashing the food slider bar red when food level is low
    private Animator        foodSliderBgAnimator;   //Animator used for flashing the food slider background red when player is starving

    private Animator        breathSliderAnimator;   //Animator used for flashing the breath slider red when player is drowning

    private void Awake()
    {
        playerMovementScript = GetComponent<PlayerMovement>();

        healthSliderAnimator = healthSliderFill.gameObject.GetComponent<Animator>();
        foodSliderAnimator   = foodSliderFill.gameObject.GetComponent<Animator>();
        foodSliderBgAnimator = foodLevelSlider.transform.Find("Background").GetComponent<Animator>();

        breathSliderAnimator = breathSliderFill.gameObject.GetComponent<Animator>();
    }

    protected void Start()
    {
        SaveLoadManager.Instance.SubscribeSaveLoadEvents(OnSave, OnLoadSetup, OnLoadConfigure);
    }

    private void OnDestroy()
    {
        SaveLoadManager.Instance.UnsubscribeSaveLoadEvents(OnSave, OnLoadSetup, OnLoadConfigure);
    }

    private void Update()
    {
        float foodLevelDecreaseAmount = Time.deltaTime / baseTimeToStarve;
        float breathLevelDecreaseAmount = Time.deltaTime / baseTimeToDrown;

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

        breathCanvasGroup.alpha = breath < 1.0 ? 1 : 0;

        if(breath > 0f)
        {
            if(breath < 1.0f)
            {
                breathCanvasGroup.alpha = 1f;
            }
            if(playerMovementScript.currentMovementState == PlayerMovement.MovementStates.dive)
            {
                DecreaseBreath(breathLevelDecreaseAmount);
            }
            else
            {
                IncreaseBreath(Time.deltaTime);
            }
        }
        else
        {
            //player is drowning
            drownDamageTimer += Time.deltaTime;

            if(drownDamageTimer >= drownDamageInterval)
            {
                DecreaseHealth(drownDamage);
                drownDamageTimer = 0.0f;
            }
        }

        UpdateHealthUI();
        UpdateFoodLevelUI();
        UpdateBreathUI();
    }

    public void OnSave(SaveData saveData)
    {
        saveData.AddData("playerFoodLevel", foodLevel);

        saveData.AddData("playerHealth", health);

        saveData.AddData("playerBreath", breath);
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

        float loadedBreath = saveData.GetData<float>("playerBreath", out bool breathLoadSuccess);
        if(breathLoadSuccess)
        {
            breath = loadedBreath;
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

    //Hugo added
    public void IncreaseBreath(float amount)
    {
        breath += amount;

        breath = Mathf.Clamp(breath, 0.0f, 1.0f);
    }

    public void DecreaseBreath(float amount)
    {
        breath -= amount;

        if(breath < 0.0f)
        {
            breath = 0.0f;
        }
    }

    private void UpdateBreathUI()
    {
        breathLevelSlider.value = Mathf.Lerp(breathLevelSlider.value, breath, Time.deltaTime * 25.0f);

        breathSliderAnimator.SetBool("Flash", (breath < 0.15f));

        breathSliderAnimator.SetBool("Flash", (breath == 0.0f));
    }

    private void UpdateFoodLevelUI()
    {
        foodLevelSlider.value = Mathf.Lerp(foodLevelSlider.value, foodLevel, Time.deltaTime * 25.0f);

        foodSliderAnimator.SetBool("Flash", (foodLevel < 0.15f));

        foodSliderBgAnimator.SetBool("Flash", (foodLevel == 0.0f));
    }

    private void UpdateHealthUI()
    {
        healthSlider.value = Mathf.Lerp(healthSlider.value, health, Time.deltaTime * 25.0f);

        healthSliderAnimator.SetBool("Flash", (health < 0.15f));
    }

    public bool PlayerIsFull()
    {
        return (foodLevel > fullThreshold);
    }
}
