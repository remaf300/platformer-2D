using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!collision.gameObject.GetComponent<PlayerMovement>().isDead)
            {
                collision.gameObject.GetComponent<PlayerMovement>().KillPlayer();
            }
        }
    }
}
