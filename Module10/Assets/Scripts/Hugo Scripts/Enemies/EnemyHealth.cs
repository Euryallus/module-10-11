using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    protected float health = 1;
    public float maxHealth = 1;

    public void DoDamage(float damageAmount)
    {
        health -= damageAmount;
        if(health <= 0.0f)
        {
            Die();
        }
    }

    protected void Die()
    {
        Destroy(gameObject);

        Debug.LogWarning(gameObject.name + " has died");
    }
}
