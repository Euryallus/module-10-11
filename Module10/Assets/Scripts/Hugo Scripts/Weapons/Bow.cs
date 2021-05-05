using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : Weapon
{

    public float chargeTime = 0.5f;
    public float arrowDamage = 0.2f;

    bool isHeld = false;
    float heldTime = 0;

    public float arrowReleaseVelocity = 5f;

    public GameObject arrowPrefab;

    public override void StartSecondardAbility()
    {
        if(cooldown >= cooldownTime && !isHeld)
        {
            isHeld = true;
            heldTime = 0f;
        }

        base.StartSecondardAbility();
    }

    public override void Update()
    {
        base.Update();

        if(isHeld)
        {
            heldTime += Time.deltaTime;
            if(heldTime > chargeTime)
            {
                heldTime = chargeTime;
            }

            transform.localScale = new Vector3(transform.localScale.x, 1.75f - (heldTime / 2) , transform.localScale.z);

        }

    }

    public override void EndSecondaryAbility()
    {
        base.EndSecondaryAbility();

        GameObject newArrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
        newArrow.GetComponent<Rigidbody>().velocity = transform.forward * (arrowReleaseVelocity * (heldTime / chargeTime));

        isHeld = false;
        cooldown = 0f;

        transform.localScale = new Vector3(transform.localScale.x, 1.75f, transform.localScale.z);

    }

}
