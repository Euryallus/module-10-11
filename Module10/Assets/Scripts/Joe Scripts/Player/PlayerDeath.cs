using System.Collections.Generic;
using UnityEngine;

public enum PlayerDeathCause
{
    Starved,
    FellOutOfWorld,
    Crushed,
    Skewered
}

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private GameObject deathPanelPrefab;

    private readonly Dictionary<PlayerDeathCause, WeightedString[]> deathCauseTextDict = new Dictionary<PlayerDeathCause, WeightedString[]>()
    {
        { PlayerDeathCause.Starved,         new WeightedString[]    { new WeightedString("You starved.", 1) } },

        { PlayerDeathCause.FellOutOfWorld,  new WeightedString[]    { new WeightedString("You fell out of the world.", 1) } },

        { PlayerDeathCause.Crushed,         new WeightedString[]    {
                                                                        new WeightedString("You were crushed.", 100),
                                                                        new WeightedString("You were flattened.", 20),
                                                                        new WeightedString("You were squished.", 20),
                                                                        new WeightedString("You were turned into a pancake.", 5),
                                                                        new WeightedString("LOL SQUASHING DEATH", 1),
                                                                    } },

        { PlayerDeathCause.Skewered,        new WeightedString[]    { new WeightedString("You were skewered by spikes.", 1) } }
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
            deathCauseText = PickRandomWeightedString(deathCauseTextDict[causeOfDeath]);
        }
        else
        {
            Debug.LogError("No dictionary entry for death cause: " + causeOfDeath);
        }

        DeathPanel deathPanel = Instantiate(deathPanelPrefab, canvasTransform).GetComponent<DeathPanel>();

        deathPanel.SetDeathCauseText(deathCauseText);
    }

    public string PickRandomWeightedString(WeightedString[] weightedStrings)
    {
        List<string> stringsPool = new List<string>();

        for (int i = 0; i < weightedStrings.Length; i++)
        {
            for (int j = 0; j < weightedStrings[i].Weight; j++)
            {
                stringsPool.Add(weightedStrings[i].Text);
            }
        }

        return stringsPool[Random.Range(0, stringsPool.Count)];
    }
}

public struct WeightedString
{
    public WeightedString(string text, int weight)
    {
        Text    = text;
        Weight  = weight;
    }

    public string   Text;
    public int      Weight;
}