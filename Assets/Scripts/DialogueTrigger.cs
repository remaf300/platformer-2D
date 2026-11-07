using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    PlayerMovement player;

    [SerializeField] AudioClip[] soundsToPlay;
    [SerializeField, Range(0, 1)] float soundVolume = 1;


    public GameObject interactIndicator;

    public Animator animator;

    public Dialogue firstDialogue;

    [SerializeField] bool isThereASecondDialogue;
    
    public Dialogue secondDialogue;


    [HideInInspector] public bool isThereASpecialDialogue;
    public Dialogue specialDialogue;

    bool isInRange;
    bool isInStayingInRange;

    bool firstDialoguehasFinished;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interactIndicator.SetActive(true);
            player = collision.GetComponent<PlayerMovement>();
            //player.isInDialogueRange = true;
            isInRange = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInStayingInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interactIndicator.SetActive(false);

            isInRange = false;
            //player.isInDialogueRange = false;
            isInStayingInRange = false;
        }
    }

    private void Update()
    {
        if(animator != null)
        {
            if (FindObjectOfType<DialogueManager>().isTalking)
            {
                animator.SetBool("IsTalking", true);
            }
            else
            {
                animator.SetBool("IsTalking", false);
            }
        }

        if (isThereASpecialDialogue)
        {
            if(isInRange && Input.GetButtonDown("Fire2") && !FindObjectOfType<DialogueManager>().isTalking)
            {
                TriggerDialogue(specialDialogue);
            }
        }
        else
        {
            if (isThereASecondDialogue)
            {
                if (isInRange && Input.GetButtonDown("Fire2") && !FindObjectOfType<DialogueManager>().isTalking && !firstDialoguehasFinished)
                {
                    firstDialoguehasFinished = true;
                    TriggerDialogue(firstDialogue);
                }

                if (isInRange && Input.GetButtonDown("Fire2") && !FindObjectOfType<DialogueManager>().isTalking && firstDialoguehasFinished)
                {
                    TriggerDialogue(secondDialogue);
                }
            }
            else
            {
                if (isInRange && Input.GetButtonDown("Fire2") && !FindObjectOfType<DialogueManager>().isTalking)
                {
                    firstDialoguehasFinished = true;
                    TriggerDialogue(firstDialogue);
                }
            }
        }

        if (FindObjectOfType<DialogueManager>().isTalking)
        {
            interactIndicator.SetActive(false);

            //player.isInDialogueRange = false;
        }

        if (isInStayingInRange && !FindObjectOfType<DialogueManager>().isTalking)
        {
            interactIndicator.SetActive(true);
        }
    }

    public void TriggerDialogue(Dialogue dialogue)
    {
        AudioManager.instance.PlayClipAt(soundsToPlay[Random.Range(0, soundsToPlay.Length)], transform, soundVolume);

        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }
}
