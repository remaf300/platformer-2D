using UnityEngine;

public class ActiveObject : MonoBehaviour
{
    [SerializeField] GameObject objectToActive;

    bool isObjectSet;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isObjectSet)
        {
            isObjectSet = true;
            objectToActive.SetActive(true);
        }
    }
}
