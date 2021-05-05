using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HazardMode
{
    PlayerTrigger,
    Continuous
}

public class Hazard : MonoBehaviour, IExternalTriggerListener
{
    [Header("Hazard")]
    [SerializeField] private Animator           animator;
    [SerializeField] private ParticleGroup      impactParticles;
    [SerializeField] private SoundClass         impactSound;
    [SerializeField] private PlayerDeathCause   deathCause              = PlayerDeathCause.Crushed;
    [SerializeField] private float              cameraShakeMultiplier   = 1.0f;
    [SerializeField] private HazardMode         mode                    = HazardMode.PlayerTrigger;

    [SerializeField] [Range(2.0f, 120.0f)]
    private float                               continuousInverval = 2.5f;

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

                playerDeath.KillPlayer(deathCause);
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

            yield return new WaitForSeconds(continuousInverval);
        }
    }

    public void ImpactEvents()
    {
        //ImpactEvents is called by an animation event

        //Spawn impact particles if any were set in the inspector
        if(impactParticles != null)
        {
            impactParticles.PlayEffect();
        }

        //Find the player's camera and shake it

        CameraShake playerCameraShake = GameObject.FindGameObjectWithTag("Player").GetComponent<CameraShake>();

        if(playerCameraShake != null)
        {
            float distanceFromPlayer = Vector3.Distance(transform.position, playerCameraShake.gameObject.transform.position);

            //The further from the hazard, the less intense screen shake will be
            float shakeIntensity = (0.3f - (distanceFromPlayer * 0.025f)) * cameraShakeMultiplier;

            if(shakeIntensity > 0.0f)
            {
                playerCameraShake.ShakeCameraForTime(0.3f, CameraShakeType.ReduceOverTime, shakeIntensity);
            }
        }

        //Play an impact sound if one was set in the inspector
        if(impactSound != null)
        {
            AudioManager.Instance.PlaySoundEffect3D(impactSound, transform.position);
        }
    }
}
