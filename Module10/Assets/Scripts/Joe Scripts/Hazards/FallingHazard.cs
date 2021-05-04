using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HazardMode
{
    PlayerTrigger,
    Continuous
}

public class FallingHazard : MonoBehaviour, IExternalTriggerListener
{
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleGroup hitGroundParticles;
    [SerializeField] private HazardMode mode;
    [SerializeField] private ExternalTrigger areaTrigger;

    [SerializeField] [Range(2.5f, 120.0f)]
    private float continuousFallInverval = 2.5f;

    private void Awake()
    {
        areaTrigger.AddListener(this);
    }

    private void Start()
    {
        if (mode == HazardMode.Continuous)
        {
            StartCoroutine(ContinuousFallCoroutine());
        }
    }

    public void OnExternalTriggerEnter(string triggerId, Collider other)
    {
        if(mode == HazardMode.PlayerTrigger)
        {
            if (triggerId == "triggerArea" && other.CompareTag("Player"))
            {
                animator.SetTrigger("Fall");
            }
        }
    }

    public void OnExternalTriggerStay(string triggerId, Collider other) { }

    public void OnExternalTriggerExit(string triggerId, Collider other) { }

    private IEnumerator ContinuousFallCoroutine()
    {
        while(mode == HazardMode.Continuous)
        {
            animator.SetTrigger("Fall");

            yield return new WaitForSeconds(continuousFallInverval);
        }
    }

    public void HitGroundEvents()
    {
        hitGroundParticles.PlayEffect();

        CameraShake playerCameraShake = GameObject.FindGameObjectWithTag("Player").GetComponent<CameraShake>();

        if(playerCameraShake != null)
        {
            float distanceFromPlayer = Vector3.Distance(transform.position, playerCameraShake.gameObject.transform.position);

            float shakeAmount = 0.3f - (distanceFromPlayer * 0.025f);

            if(shakeAmount > 0.0f)
            {
                playerCameraShake.ShakeCameraForTime(0.3f, CameraShakeType.ReduceOverTime, shakeAmount);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            PlayerDeath playerDeath = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDeath>();

            playerDeath.KillPlayer(PlayerDeathCause.Crushed);
        }
    }
}
