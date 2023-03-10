using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class PlayerDie : MonoBehaviour
{
    private static PlayerDie instance;
    public static bool isPlayerDead = false;

    private void Awake()
    {
        instance = this;
    }

    public static PlayerDie GetInstance()
    {
        return instance;
    }

    public void DieFPS()
    {
        isPlayerDead = true;

        this.gameObject.transform.position = new Vector3(0, 0, 0);

        HUD.GetInstance().PlayerIsDeadHUD();

        Invoke("Restart", 2.5f);
    }

    void Restart()
    {
        SceneManager.LoadScene(0);
    }

    
}
