using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisableLeaving : MonoBehaviour
{
    [SerializeField] GameObject Player;
    [SerializeField] GameObject Blockage;
    private BoxCollider blockagecol;

    private bool CannotLeave = false;
    private bool isObjectiveCompleted = false;

    [SerializeField] GameObject[] Sounds;
    [SerializeField] AudioSource Audiosource;

    private void Awake()
    {
        blockagecol = Blockage.GetComponent<BoxCollider>();
    }

    private void OnTriggerStay(Collider other)
    {
        HUD.GetInstance().ShowEnemiesRemaining();

        if(HUD.GetInstance().Enemies == 0 && !isObjectiveCompleted)
        {
            isObjectiveCompleted = true;
            HUD.GetInstance().MissionComplete();
            blockagecol.enabled = false;
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.tag == "Player") && CannotLeave == false)
        {
            CannotLeave = true;

            HUD.GetInstance().ShowEnemiesRemaining(); //show enemies remaining HUD

            blockagecol.enabled = true; //enable blockage so player cannot leave

            foreach (var SFX in Sounds)
            {
                SFX.SetActive(false); //disable sounds
            }

            Audiosource.Play(); //play sound on trigger enter
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (HUD.GetInstance().Enemies == 0)
        {
            foreach (var SFX in Sounds) //enable sounds
            {
                SFX.SetActive(true);
            }
            
            Audiosource.Stop(); //stop sound on trigger exit
        }      
    }
}
