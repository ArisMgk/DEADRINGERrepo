using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GunPickup : Interaction
{
    private Rigidbody rb;
    private BoxCollider coll;

    [SerializeField] Transform player, gunContainer, fpsCam;

    public float pickUpRange;
    public float dropForwardForce, dropUpwardForce;

    public bool equipped = false;
    public bool slotFull = false;

    public static GunPickup instance;


    private void Start()
    {
        //cache
        rb = this.gameObject.GetComponent<Rigidbody>();
        coll = this.gameObject.GetComponent<BoxCollider>();

        instance = this;
        this.gameObject.SetActive(false);

        if (!equipped)
        {
            ShootingBullets.GetInstance().enabled = false;
            rb.isKinematic = false;
            coll.isTrigger = false;
            
        }
        if (equipped)
        {
            ShootingBullets.GetInstance().enabled = true;
            rb.isKinematic = true;
            coll.isTrigger = true;
            slotFull = true;              
        }
    }

    public static GunPickup GetInstance()
    {
        return instance;
    }

    private void PickUp()
    {
        //show objective
        HUD.GetInstance().Progress.text = "ENTER THE FACTORY";

        //equipped and slot is full bool true
        equipped = true;
        slotFull = true;

        //transform weapon rotation and location to the appropriate height and rotation for the player POV
        transform.SetParent(gunContainer);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = Vector3.one;

        rb.isKinematic = true;
        coll.isTrigger = true;

        ShootingBullets.GetInstance().enabled = true;
    }

    public override void OnInteract()
    {
        PickUp();
    }

    public override void OnFocus()
    {
        
    }

    public override void OnLoseFocus()
    {
        
    }

    public void EnableWeapon()
    {
        this.gameObject.SetActive(true);
    }
}



