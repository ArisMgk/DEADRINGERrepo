using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text EnemiesHUD;
    public TMP_Text Progress;
    public TMP_Text BulletText;
    public GameObject Controls;
    public GameObject PlayerDiedCanvas;

    //enemies left number
    public int Enemies = 10;
    
    private static HUD instance;

    private void Awake()
    {
        instance = this;
        Controls.SetActive(false);
    }

    public static HUD GetInstance() //singleton
    {
        return instance; 
    }

    public void ShowEnemiesRemaining() //how many enemies are left to kill
    { 
       EnemiesHUD.text = "ENEMIES REMAINING: " + Enemies;
    }

    public void EnemyHUD() //Enemies INT
    {
        {
            Enemies = Enemies - 1;                    
        }
    }

    public void MissionComplete() //if you've killed everyone, change text to mission complete & to get back to the car
    {
        if (Enemies == 0)
        {
            EnemiesHUD.text = "MISSION COMPLETE";
            Progress.text = "GET BACK IN THE CAR";
        }
    }

    public void ShowControls()
    {
        Controls.SetActive(true);
    }

    public void HideControls()
    {
        Controls.SetActive(false);
    }

    public void PlayerIsDeadHUD()
    {
        PlayerDiedCanvas.SetActive(true);
    }
    
}
