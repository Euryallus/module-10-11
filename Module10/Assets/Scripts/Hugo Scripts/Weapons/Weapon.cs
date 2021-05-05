using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Weapon : HeldItem
{

    public float baseDamage = 0.25f;
    public float damageVariation = 0.1f;


    public float critDamageMultiplier = 1.5f;
    [Range(0.0f, 1.0f)]
    public float critChance = 0.05f;



    public float damageModifier = 1f;

    public float cooldownTime = 0.5f;
    protected float cooldown = 0f;


    public Animator animator;

    protected override void Awake()
    {
        base.Awake();
    }
    
    public virtual void Update()
    {
        cooldown += Time.deltaTime;
    }

    public float CalculateDamage()
    {
        float damage = Random.Range(baseDamage - (damageVariation / 2), baseDamage + (damageVariation / 2)) * damageModifier;

        if (Random.Range(0f, 1f) < critChance)
        {
            damage *= critDamageMultiplier;
        }

        return damage;
    }
}
