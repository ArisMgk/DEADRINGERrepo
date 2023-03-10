using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interaction : MonoBehaviour
{
    //variables for looking towards player - NPC
    private CharacterController Player;
    private Transform playertrans;
    private Coroutine smoothMove = null;

    public abstract void OnInteract();
    public abstract void OnFocus();
    public abstract void OnLoseFocus();



    public void LookSmoothly()
    {
        float time = 1f;

        Vector3 lookAt = playertrans.position;
        lookAt.y = transform.position.y;

        if (smoothMove == null)
        {
            smoothMove = StartCoroutine(LookAtSmoothly(transform, lookAt, time));
        }
        else
        {
            StopCoroutine(smoothMove);
            smoothMove = smoothMove = StartCoroutine(LookAtSmoothly(transform, lookAt, time));
        }
    }



    public IEnumerator LookAtSmoothly(Transform objToMove, Vector3 worldpos, float duration)
    {
        Quaternion currentRot = objToMove.rotation;
        Quaternion newRot = Quaternion.LookRotation(worldpos - objToMove.position, objToMove.TransformDirection(Vector3.up));

        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            objToMove.rotation = Quaternion.Lerp(currentRot, newRot, counter / duration);

            yield return null;
        }
    }

    public virtual void Awake()
    {
        gameObject.layer = 9;
        Debug.Log("Layer is 9");

        Player = GameObject.FindObjectOfType<CharacterController>();
        playertrans = Player.transform;
    }
}
