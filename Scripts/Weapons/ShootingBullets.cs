using UnityEngine;
using System.Collections;
using TMPro;

public class ShootingBullets : MonoBehaviour
{
    [Header("Range & firerate variables")]
    [SerializeField] float range = 100f;
    [SerializeField] float FireRate = 15f;

    [Header("Sound & Enemy arrays")]
    public AudioClip[] ClipSounds;
    public AudioSource[] Audiosource;
    [SerializeField] GameObject[] Enemies;
    
    [Header("Weapon functionality variables")]
    [SerializeField] int MaxAmmo = 30;
    [SerializeField] int CurrentAmmo;
    [SerializeField] float ReloadTime = 1f;
    [SerializeField] bool IsReloading = false;
    [SerializeField] float NextTimeToFire = 0;

    public Camera FPScam;
    public GameObject Bullet;
    public ParticleSystem Muzzleflash;

    private bool CanReload => Input.GetKeyDown(KeyCode.R) || CurrentAmmo <= 0;
    private bool CanShoot => Input.GetButton("Fire1") && Time.time >= NextTimeToFire && !PlayerDie.isPlayerDead;
    private static ShootingBullets instance;

    private void Awake()
    {
        instance = this;
       
        Enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }

    public static ShootingBullets GetInstance()
    {
        return instance;
    }

    void Update()
    {
        if (IsReloading)
        {
            return;
        }

        if (CanReload)
        {
            StartCoroutine(Reload());
            return;
        }

        if (CanShoot)
        {
            NextTimeToFire = Time.time + 1f / FireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        Muzzleflash.Play();

        CurrentAmmo--;

        Audiosource[0].PlayOneShot(ClipSounds[0], 1f);

        BulletGUI();

        //Raycast for shooting the bullets
        RaycastHit hit;

        if (Physics.Raycast(FPScam.transform.position, FPScam.transform.forward, out hit))
        {

            foreach (GameObject Enemy in Enemies)                           
            {
                if (hit.collider.gameObject == Enemy.gameObject)       
                {
                    EnemyTakeDamage.GetInstance().TakeDamage();
                    HUD.GetInstance().EnemyHUD();                   
                }
            }

            //instantiate the bullet gameobject
            GameObject BulletGO = Instantiate(Bullet, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(BulletGO, 1f);
        }
    }


    IEnumerator Reload()
    {
        HUD.GetInstance().BulletText.text = "RELOADING...";
        Audiosource[1].PlayOneShot(ClipSounds[1], 1f);

        IsReloading = true;

        yield return new WaitForSeconds(ReloadTime);

        CurrentAmmo = MaxAmmo;
        IsReloading = false;

        BulletGUI();
    }


    void BulletGUI()
    {
        HUD.GetInstance().BulletText.text = CurrentAmmo + "";
    }


}
