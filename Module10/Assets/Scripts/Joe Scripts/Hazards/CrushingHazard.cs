using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HazardMode
{
    PlayerTrigger,
    Continuous
}

public class CrushingHazard : MonoBehaviour, IExternalTriggerListener
{
    [Header("Hazard")]
    [SerializeField] private Animator           animator;
    [SerializeField] private ParticleGroup      impactParticles;
    [SerializeField] private HazardMode         mode;

    [SerializeField] [Range(2.5f, 120.0f)]
    private float                               continuousFallInverval = 2.5f;

    [Header("Triggers")]
    [SerializeField] private ExternalTrigger[]  hitTriggers;
    [SerializeField] private ExternalTrigger    areaTrigger;


    private void Awake()
    {
        for (int i = 0; i < hitTriggers.Length; i++)
        {
            hitTriggers[i].AddListener(this);
        }

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
        if(other.CompareTag("Player"))
        {
            if (triggerId == "triggerArea")
            {
                if (mode == HazardMode.PlayerTrigger)
                {
                    animator.SetTrigger("StartHazard");
                }
            }
            else if (triggerId == "hit")
            {
                PlayerDeath playerDeath = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDeath>();

                playerDeath.KillPlayer(PlayerDeathCause.Crushed);
            }
        }
    }

    public void OnExternalTriggerStay(string triggerId, Collider other) { }

    public void OnExternalTriggerExit(string triggerId, Collider other) { }

    private IEnumerator ContinuousFallCoroutine()
    {
        while(mode == HazardMode.Continuous)
        {
            animator.SetTrigger("StartHazard");

            yield return new WaitForSeconds(continuousFallInverval);
        }
    }

    public void ImpactEvents()
    {
        impactParticles.PlayEffect();

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
}
