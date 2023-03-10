using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTakeDamage : AIBehavior
{
    private static EnemyTakeDamage instance;

    private void Awake()
    {
        instance = this;
    }

    public static EnemyTakeDamage GetInstance()
    {
        return instance;
    }

    void Start()
    {
        //cache
        player = GameObject.Find("CharacterController").transform;
        anim = this.gameObject.GetComponent<Animator>();
        ColliderEnemy = this.gameObject.GetComponent<CapsuleCollider>();

    }

    void Update()
    {
        PlayerInSightRange = Physics.CheckSphere(transform.position, SightRange, WhatIsPlayer);
        PlayerInAttackRange = Physics.CheckSphere(transform.position, AttackRange, WhatIsPlayer);

        if (PlayerInAttackRange && PlayerInSightRange)
        {
            AttackPlayer();
        }
    }

    void AttackPlayer()
    {

        Vector3 newPlayer = player.position;
        newPlayer.y = transform.position.y;
        transform.LookAt(newPlayer);


        if (!AlreadyAttacked)
        {
            anim.SetBool("IsShooting", true);
            Muzzleflash.Play();
            Audiosource.PlayOneShot(BulletSound, 1f);


            // randomlize bullet direction
            var RayDirection = EnemyCam.transform.forward;
            RayDirection.x += Random.Range(-shotSpread, shotSpread);
            RayDirection.y += Random.Range(-shotSpread, shotSpread);
            RayDirection.z += Random.Range(-shotSpread, shotSpread);


            //Raycast for shooting the bullets
            RaycastHit hit;

            if (Physics.Raycast(EnemyCam.transform.position, RayDirection, out hit))
            {
                if (hit.collider.tag == "Player")
                {
                    PlayerDie.GetInstance().DieFPS();
                    Debug.Log("Restarted game because player got hit");
                }
               

                //instantiate bullet of EnemyAI
                GameObject BulletGO = Instantiate(Bullet, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(BulletGO, 1f);
            }

            AlreadyAttacked = true;
            Invoke(nameof(ResetAttack), TimeBetweenAttacks);
        }
    }

    void ResetAttack()
    {
        AlreadyAttacked = false;
        anim.SetBool("IsShooting", false);
    }


    public override void TakeDamage()
    {
        PlaySoundOnDeath();

        ColliderEnemy.enabled = false;
        ResetRigAI.GetInstance().ResetRig();

        SightRange = 0f;
        AttackRange = 0f;

        anim.SetBool("IsDead", true);
        Destroy(this.gameObject, 3f);
    }

    void PlaySoundOnDeath()
    {
        int RandomClip = Random.Range(0, GruntSounds.Length);
        GruntSource.clip = GruntSounds[RandomClip];
        GruntSource.Play();        
    }
}
