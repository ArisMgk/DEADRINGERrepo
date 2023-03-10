using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingTrigger : MonoBehaviour
{
    [SerializeField] AudioSource Audiosource;

    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.tag == "Player") && HUD.GetInstance().Enemies == 0)
        {
            Audiosource.Play();
            Invoke("Ending", 1.5f);
        }

    }

    void Ending()
    {
        SceneManager.LoadScene(0);
    }

}
