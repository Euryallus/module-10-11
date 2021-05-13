using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneUI : MonoBehaviour
{
    public static GameSceneUI Instance;

    #region InspectorVariables
    // Variables in this region are set in the inspector

    [SerializeField] private GameObject pausePanelPrefab;
    [SerializeField] private GameObject optionsPanelPrefab;

    #endregion

    private PlayerMovement playerMovement;
    private GameObject pausePanel;
    private GameObject optionsPanel;

    private bool pausePanelShowing;
    private bool optionsPanelShowing;

    private void Awake()
    {
        // Ensure that an instance of the class does not already exist
        if (Instance == null)
        {
            // Set this class as the instance
            Instance = this;
        }
        // If there is an existing instance that is not this, destroy the GameObject this script is connected to
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if(!InputFieldSelection.AnyFieldSelected)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                if(pausePanelShowing)
                {
                    HidePauseUI();
                }
                else if(optionsPanelShowing)
                {
                    HideOptionsUI();
                    UnpauseGame();
                }
                else if(!UIPanel.AnyBlockingPanelShowing() && Time.timeScale > 0.0f)
                {
                    PauseAndShowPauseUI();
                }
            }
        }
    }

    private void PauseGame()
    {
        playerMovement.StopMoving();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0.0f;

        AudioManager.Instance.PlaySoundEffect2D("buttonClickMain1");
    }

    private void UnpauseGame()
    {
        playerMovement.StartMoving();

        Cursor.lockState = CursorLockMode.Locked;

        Time.timeScale = 1.0f;

        AudioManager.Instance.PlaySoundEffect2D("buttonClickMain2");
    }

    public void PauseAndShowPauseUI()
    {
        pausePanel = Instantiate(pausePanelPrefab, GameObject.FindGameObjectWithTag("JoeCanvas").transform);

        pausePanelShowing = true;

        PauseGame();
    }

    public void HidePauseUI(bool unpauseGame = true)
    {
        if (pausePanel != null)
        {
            Destroy(pausePanel);
        }

        pausePanelShowing = false;

        if(unpauseGame)
        {
            UnpauseGame();
        }
    }

    public void ShowOptionsUI()
    {
        optionsPanel = Instantiate(optionsPanelPrefab, transform.parent);

        optionsPanel.GetComponent<OptionsPanel>().Setup(OptionsOpenType.GameScenePause);

        optionsPanelShowing = true;

        AudioManager.Instance.PlaySoundEffect2D("buttonClickMain1");
    }

    public void HideOptionsUI()
    {
        if(optionsPanel != null)
        {
            Destroy(optionsPanel);
        }

        optionsPanelShowing = false;
    }
}
