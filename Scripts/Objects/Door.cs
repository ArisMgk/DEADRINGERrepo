using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interaction
{
    [SerializeField] GameObject DoorGO;
    private Animation _dooranim;

    private void Start()
    {
        _dooranim = DoorGO.GetComponent<Animation>();
    }

    public override void OnFocus()
    {
        Debug.Log("looking at door");
    }

    public override void OnInteract()
    {
        if(GunPickup.GetInstance().equipped)
        {
            _dooranim.Play("Door_Open");
            HUD.GetInstance().Progress.text = "KILL EVERYONE";
        }
    }

    public override void OnLoseFocus()
    {
        Debug.Log("not looking at the door");
    }
}
