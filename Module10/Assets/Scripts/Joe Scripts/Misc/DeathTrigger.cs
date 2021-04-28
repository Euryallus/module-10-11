using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    [SerializeField] private GameObject deathPanelPrefab;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Transform canvasTransform = GameObject.FindGameObjectWithTag("JoeCanvas").transform;

            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().StopMoving();

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            Time.timeScale = 0.0f;

            Instantiate(deathPanelPrefab, canvasTransform);
        }
    }
}
