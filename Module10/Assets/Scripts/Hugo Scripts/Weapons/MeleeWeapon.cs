using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
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

            animator.SetTrigger("Swing");
            
            cooldown = 0f;

            base.PerformMainAbility();
        }
        
    }
}
