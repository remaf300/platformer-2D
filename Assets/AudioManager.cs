using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] AudioMixerGroup soundEffectMixer;

    [SerializeField] float cursorAFKTime = 5;
    float currentCursorAFKTime;

    Vector3 currentCursorPos;

    private void Awake()
    {
        instance = this;

        currentCursorAFKTime = cursorAFKTime;
    }

    private void Update()
    {
        Vector3 mousePos = Input.mousePosition;

        if (mousePos == currentCursorPos)
        {
            currentCursorAFKTime -= Time.deltaTime;

            if (currentCursorAFKTime < 0) currentCursorAFKTime = 0;
        }
        else
        {
            currentCursorAFKTime = cursorAFKTime;
        }

        currentCursorPos = mousePos;

        if(Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            currentCursorAFKTime = cursorAFKTime;
        }

        if(currentCursorAFKTime == 0 && !Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2))
        {
            Cursor.visible = false;
        }
        else
        {
            Cursor.visible = true;
        }
    }

    public AudioSource PlayClipAt(AudioClip clip, Transform pos, float clipVolume = 1)
    {
        GameObject tempGO = new GameObject("TempAudio");
        tempGO.transform.position = pos.position;

        AudioSource audioSource = tempGO.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.outputAudioMixerGroup = soundEffectMixer;
        audioSource.volume = clipVolume;
        audioSource.Play();
        Destroy(tempGO, clip.length);
        return audioSource;
    }
}
