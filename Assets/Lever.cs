using System.Collections;
using UnityEngine;

public class Lever : MonoBehaviour
{
    [SerializeField] GameObject trapDoor;

    [Header("Audio")]
    [SerializeField] AudioClip leverSoundToPlay;
    [SerializeField] AudioClip trapSoundToPlay;
    [SerializeField, Range(0, 1)] float soundVolume = 1;

    [Header("ActiveCamera")]
    [SerializeField] bool mustActiveCameraFollow;
    [SerializeField] CameraFollow cameraFollow;
    [SerializeField] float animationTime = 2;
    [SerializeField] Animator fadeUIAnimator;

    [Header("ActiveSpecialDialogue")]
    [SerializeField] bool mustActiveSpecialDialogue;
    [SerializeField] DialogueTrigger triggerDialogueScript;

    Animator cameraFollowAnimator;

    bool isActive;


    public void ActiveLever()
    {
        if(isActive) return;

        isActive = true;
        trapDoor.GetComponent<Animator>().SetBool("Open", true);
        GetComponent<Animator>().SetBool("Open", true);

        AudioManager.instance.PlayClipAt(leverSoundToPlay, transform, soundVolume);

        if (mustActiveSpecialDialogue)
        {
            triggerDialogueScript.isThereASpecialDialogue = true;
        }

        if (mustActiveCameraFollow)
        {
            cameraFollowAnimator = cameraFollow.gameObject.GetComponent<Animator>();
            cameraFollowAnimator.SetBool("ShowTrap", true);

            StartCoroutine(WaitForAnim());

            StartCoroutine(WaitForAudio());
        }
    }

    IEnumerator WaitForAudio()
    {
        yield return new WaitForSeconds(2);

        AudioManager.instance.PlayClipAt(trapSoundToPlay, transform, soundVolume);
    }

    IEnumerator WaitForAnim()
    {
        yield return new WaitForSeconds(animationTime);

        fadeUIAnimator.gameObject.SetActive(true);
        fadeUIAnimator.SetTrigger("FadeIn");

        yield return new WaitForSeconds(1.02f);


        cameraFollowAnimator.enabled = false;
        cameraFollow.enabled = true;

        fadeUIAnimator.SetTrigger("FadeOut");

        yield return new WaitForSeconds(1);

        fadeUIAnimator.gameObject.SetActive(false);
    }
}
