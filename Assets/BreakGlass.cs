using UnityEngine;

public class BreakGlass : MonoBehaviour
{
    [SerializeField] float destroyDelay;
    [SerializeField] GameObject glassParts;
    SpriteRenderer spriteRenderer;
    BoxCollider2D bx2D;

    [SerializeField] AudioClip soundsToPlay;
    [SerializeField, Range(0, 1)] float soundVolume = 1;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        bx2D = GetComponent<BoxCollider2D>();
    }


    public void Break()
    {
        glassParts.SetActive(true);
        spriteRenderer.enabled = false;
        bx2D.enabled = false;

        gameObject.layer = 0;
        
        if(soundsToPlay != null) AudioManager.instance.PlayClipAt(soundsToPlay, transform, soundVolume);

        Destroy(gameObject, destroyDelay);
    }
}
