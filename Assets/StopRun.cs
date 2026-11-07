using UnityEngine;
using UnityEngine.EventSystems;

public class StopRun : MonoBehaviour
{
    [SerializeField] GameObject winMenu;
    [SerializeField] GameObject fadePanel;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] AudioSource audioSource;

    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] GrapplingGun grapplingGun;
    [SerializeField] EventSystem eventSystem;
    public int levelToUnlock;
    public bool isCollectableCollected;

    bool isInRange;
    bool isActive;
    bool isWaitingForMusic;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = true;
        }
    }

    private void Update()
    {
        if (isInRange && !isActive && playerMovement.gameObject.GetComponent<SpringJoint2D>() != null)
        {
            isActive = true;
            grapplingGun.isRunFinished = true;
            playerMovement.enabled = false;
            isWaitingForMusic = true;
        }

        if (isWaitingForMusic)
        {
            if (!audioSource.isPlaying && pauseMenu.activeSelf == false)
            {
                fadePanel.SetActive(false);
                winMenu.SetActive(true);
                eventSystem.SetSelectedGameObject(winMenu.transform.GetChild(0).gameObject);
                isWaitingForMusic = false;
                
                LoadAndSaveData.instance.levelToUnlock = levelToUnlock;

                if (isCollectableCollected)
                {
                    LoadAndSaveData.instance.isCollectableCollected = isCollectableCollected;
                }

                LoadAndSaveData.instance.SaveData();

                enabled = false;
            }
        }
    }
}
