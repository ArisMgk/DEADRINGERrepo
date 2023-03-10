using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    public GameObject Tutorial;
    public AudioSource Audiosource;

    public void PlayButton()
    {
        Audiosource.Play();
        SceneManager.LoadScene(1);
    }


    public void QuitButton()
    {
        Audiosource.Play();
        Application.Quit();
    }

}
