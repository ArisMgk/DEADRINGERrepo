using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trunk : Interaction
{
    [SerializeField] private GameObject Car;
    private Animation _caranim;

    [SerializeField] private AudioSource _audiosource;
    [SerializeField] private AudioClip[] _trunksounds; //[0] is open sound, [1] is close sound

    private bool doOnce = false;
    private bool closeOnce = false;

    private void Start()
    {
        _caranim = Car.GetComponent<Animation>();
    }


    void OpenTrunk()
    {
        if (!doOnce)
        {
            GunPickup.GetInstance().EnableWeapon();
            _caranim.Play("Trunk_Open");
            _audiosource.PlayOneShot(_trunksounds[0]);

            doOnce = true;
        }
    }

    private void CloseTrunk()
    {
        if (!closeOnce)
        {
            closeOnce = true;
            _caranim.Play("Trunk_Close");
            _audiosource.PlayOneShot(_trunksounds[1]);
        }      
    }

    public override void OnInteract()
    {
        OpenTrunk();
    }

    public override void OnFocus()
    {
        Debug.Log("looking at trunk");
    }

    public override void OnLoseFocus()
    {
        Debug.Log("not looking at trunk");

        if(GunPickup.GetInstance().equipped)
        {
            CloseTrunk();
        }
    }
}
