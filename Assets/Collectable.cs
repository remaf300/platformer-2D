using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] EndPortal portal;
    [SerializeField] StopRun stopRun;

    [SerializeField] AudioClip soundsToPlay;
    [SerializeField, Range(0, 1)] float soundVolume = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (portal != null) portal.isCollectableCollected = true;

            if (stopRun != null) stopRun.isCollectableCollected = true;

            AudioManager.instance.PlayClipAt(soundsToPlay, transform, soundVolume);

            Destroy(gameObject);
        }
    }
}
