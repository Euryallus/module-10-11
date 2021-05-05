using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    public float reachLength = 5f;
    public override void PerformMainAbility()
    {
        if(cooldown >= cooldownTime)
        {
            if (Physics.Raycast(playerTransform.position, playerTransform.forward, out RaycastHit weaponHit, reachLength))
            {
                if (weaponHit.transform.GetComponent<EnemyHealth>())
                {
                    float damage = CalculateDamage();
                    weaponHit.transform.GetComponent<EnemyHealth>().DoDamage(damage);

                    Debug.Log(damage);
                }
            }

            if(animator != null)
            {
                animator.SetTrigger("Swing");
            }
            cooldown = 0f;
            base.PerformMainAbility();
        }
        
    }
}
