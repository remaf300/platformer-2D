using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunManager : MonoBehaviour
{
    [SerializeField] AudioSource audioScource;
    [SerializeField] GameObject objToActive;
    //[SerializeField] CameraFollow cameraFollowScript;

    bool isInRange;

    bool isRunActivate;

    private void Start()
    {
        objToActive.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = false;
        }
    }

    void Update()
    {
        if(isInRange && !isRunActivate)
        {
            isRunActivate = true;

            objToActive.SetActive(true);

            //cameraFollowScript.player = objToActive.transform.GetChild(0).gameObject;

            audioScource.Play();
        }
    }
}
