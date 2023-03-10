using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;


public abstract class AIBehavior : MonoBehaviour
{

    public Transform player;
    public LayerMask WhatIsGround, WhatIsPlayer;
    public GameObject Bullet;
    public ParticleSystem Muzzleflash;
    public AudioClip BulletSound;
    public AudioSource Audiosource;
    public Camera EnemyCam;
    public Animator anim;
    public ShootingBullets PlayerBullet;
    public CapsuleCollider ColliderEnemy;
    public AudioSource GruntSource;
    public AudioClip[] GruntSounds;

    public float TimeBetweenAttacks;
    public bool AlreadyAttacked;
    public float shotSpread = 1f;

    //vars for checking where the player is
    public float SightRange, AttackRange;
    public bool PlayerInSightRange, PlayerInAttackRange;

    public virtual void TakeDamage()
    {
        ResetRigAI.GetInstance().ResetRig(); //reset IK rig

        //change sight and attack range
        SightRange = 0f;
        AttackRange = 0f;

        anim.SetBool("IsDead", true); //play death animation

        Destroy(gameObject, 3f);       
    }
}
