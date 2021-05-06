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
    public ItemGroup arrowRequired;

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
        InventoryPanel inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<InventoryPanel>();


        if (inventory.ContainsQuantityOfItem(arrowRequired))
        {
            GameObject newArrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
            newArrow.GetComponent<Rigidbody>().velocity = transform.forward * (arrowReleaseVelocity * (heldTime / chargeTime));

            newArrow.transform.forward = transform.forward;

            isHeld = false;
            cooldown = 0f;

            inventory.RemoveItemFromInventory(arrowRequired.Item);
        }
        else
        {
            Debug.LogWarning("No arrow in inventory!");
        }

        transform.localScale = new Vector3(transform.localScale.x, 1.75f, transform.localScale.z);

    }

}
