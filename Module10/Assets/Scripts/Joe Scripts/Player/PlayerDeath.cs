using System.Collections.Generic;
using UnityEngine;

public enum PlayerDeathCause
{
    Starved,
    FellOutOfWorld,
    Crushed
}

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private GameObject deathPanelPrefab;

    private Dictionary<PlayerDeathCause, string> deathCauseTextDict = new Dictionary<PlayerDeathCause, string>()
    {
        { PlayerDeathCause.Starved,         "You starved." },
        { PlayerDeathCause.FellOutOfWorld,  "You fell out of the world." },
        { PlayerDeathCause.Crushed,         "You were crushed." }
    };

    public void KillPlayer(PlayerDeathCause causeOfDeath)
    {
        Debug.Log("Player died! Cause of death: " + causeOfDeath);

        Transform canvasTransform = GameObject.FindGameObjectWithTag("JoeCanvas").transform;

        GetComponent<PlayerMovement>().StopMoving();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Time.timeScale = 0.0f;

        string deathCauseText = "";

        if (deathCauseTextDict.ContainsKey(causeOfDeath))
        {
            deathCauseText = deathCauseTextDict[causeOfDeath];
        }
        else
        {
            Debug.LogError("No dictionary entry for death cause: " + causeOfDeath);
        }

        DeathPanel deathPanel = Instantiate(deathPanelPrefab, canvasTransform).GetComponent<DeathPanel>();

        deathPanel.SetDeathCauseText(deathCauseText);
    }
}
