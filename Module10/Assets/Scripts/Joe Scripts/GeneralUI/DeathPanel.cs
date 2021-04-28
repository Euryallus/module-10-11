using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathPanel : MonoBehaviour
{
    private void Start()
    {
        //Play a sound when the death panel is first shown
        AudioManager.Instance.PlaySoundEffect2D("sealExplosion");
        AudioManager.Instance.PlaySoundEffect2D("believe");
    }

    public void ButtonRespawn()
    {
        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        Debug.Log("===== PLAYER DEATH: RELOADING SCENE =====");

        Cursor.visible = false;
        Time.timeScale = 1.0f;

        //Reload the active scene and hence reset progress to where the player last saved
        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name, LoadSceneMode.Single);

        while(!operation.isDone)
        {
            yield return null;
        }
    }
}