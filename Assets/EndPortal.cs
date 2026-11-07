using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class EndPortal : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] GameObject endMenu;
    [SerializeField] GameObject player;
    [SerializeField] GameObject fadePanel;
    [SerializeField] EventSystem eventSystem;
    [SerializeField] int levelToUnlock;


    [Header("Audio")]
    [SerializeField] AudioClip soundsToPlay;
    [SerializeField, Range(0, 1)] float soundVolume = 1;

    [HideInInspector] public bool isCollectableCollected;
    bool isInRange;
    bool isGameFinished;

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
        if (isInRange && !isGameFinished)
        {
            StartCoroutine(Wait());
        }
    }

    IEnumerator Wait()
    {
        isGameFinished = true;
        player.GetComponent<PlayerMovement>().enabled = false;
        player.GetComponent<Animator>().SetBool("GameIsFinished", true);
        player.GetComponent<Rigidbody2D>().velocity = Vector3.zero;

        yield return new WaitForSeconds(1);

        // Sauvegarder les données
        LoadAndSaveData.instance.levelToUnlock = levelToUnlock;

        if (isCollectableCollected)
        {
            LoadAndSaveData.instance.isCollectableCollected = isCollectableCollected;
        }

        LoadAndSaveData.instance.SaveData();

        AudioManager.instance.gameObject.GetComponent<AudioSource>().Stop();
        AudioManager.instance.PlayClipAt(soundsToPlay, transform, soundVolume);

        fadePanel.SetActive(false);
        endMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(endMenu.transform.GetChild(0).gameObject);
    }
}
