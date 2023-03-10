using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

public class ResetRigAI : MonoBehaviour
{

    [SerializeField] TwoBoneIKConstraint LeftHandIK;
    [SerializeField] TwoBoneIKConstraint RightHandIK;
    [SerializeField] GameObject Gun;

    private Rigidbody rb;

    private static ResetRigAI instance;

    private void Awake()
    {
        instance = this;
        
        //cache
        rb = Gun.GetComponent<Rigidbody>();
    }

    public static ResetRigAI GetInstance()
    {
        return instance;
    }


    public void ResetRig()
    {
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;

        RightHandIK.weight = 0;
        LeftHandIK.weight = 0;
    }
}
