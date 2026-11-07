using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] Text timerText;
    [SerializeField] GameObject endMenu;
    [SerializeField] Text endMenuTimerText;

    float timer = 0;

    bool menuSet;

    private void Start()
    {
        timerText.text = "0 s";
    }

    private void Update()
    {
        if (endMenu.activeSelf == true && !menuSet)
        {
            menuSet = true;

            endMenuTimerText.text = timer.ToString() + " s";

            timerText.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            timer += Time.deltaTime;

            timerText.text = timer.ToString() + " s";
        }
    }
}
