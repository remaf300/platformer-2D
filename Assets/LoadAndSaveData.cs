using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadAndSaveData : MonoBehaviour
{
    public static LoadAndSaveData instance;

    [HideInInspector] public int levelToUnlock = 1;
    [HideInInspector] public bool isCollectableCollected;

    private void Awake()
    {
        instance = this;
    }

    public void SaveData()
    {
        if(levelToUnlock > PlayerPrefs.GetInt("levelReached", 1))
        {
            PlayerPrefs.SetInt("levelReached", levelToUnlock);
        }

        if(isCollectableCollected)
        {
            PlayerPrefs.SetInt("collectable", levelToUnlock - 1);

            if (levelToUnlock - 1 == 1)
            {
                PlayerPrefs.SetInt("firstButton", 1);
            }
            else if (levelToUnlock - 1 == 2)
            {
                PlayerPrefs.SetInt("secondButton", 1);
            }
            else if (levelToUnlock - 1 == 3)
            {
                PlayerPrefs.SetInt("thirdButton", 1);
            }
            else if (levelToUnlock - 1 == 4)
            {
                PlayerPrefs.SetInt("fourthButton", 1);
            }
            else if (levelToUnlock - 1 == 5)
            {
                PlayerPrefs.SetInt("fifthButton", 1);
            }
        }

    }
}
