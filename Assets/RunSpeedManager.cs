using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunSpeedManager : MonoBehaviour
{
    [SerializeField] PlayerMovement player;


    [SerializeField] bool setMaxMoveSpeedToNormal;
    [SerializeField] bool setMaxMoveSpeedToDown;
    [SerializeField] float speedToAdd;

    bool isInRange;

    float startMaxMoveSpeed;

    private void Start()
    {
        startMaxMoveSpeed = player.maxMoveSpeed;
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

    private void Update()
    {
        if (isInRange)
        {
            if (setMaxMoveSpeedToNormal)
            {
                player.maxMoveSpeed = startMaxMoveSpeed;
            }
            else if(setMaxMoveSpeedToDown)
            {
                player.maxMoveSpeed = startMaxMoveSpeed - speedToAdd;
            }
            else
            {
                player.maxMoveSpeed = startMaxMoveSpeed + speedToAdd;
            }
            
        }
    }
}
