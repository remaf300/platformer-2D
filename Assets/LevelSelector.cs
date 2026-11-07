using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    [SerializeField] Animator fadeAnimator;

    [SerializeField] Button returnButton;

    public Button[] levelButtons;

    int levelReachedWas5;

    private void Start()
    {
        int levelReached = PlayerPrefs.GetInt("levelReached", 1);

        int collectable = PlayerPrefs.GetInt("collectable", 0);

        int firstButton = PlayerPrefs.GetInt("firstButton", 0);
        int secondButton = PlayerPrefs.GetInt("secondButton", 0);
        int thirdButton = PlayerPrefs.GetInt("thirdButton", 0);
        int fourthButton = PlayerPrefs.GetInt("fourthButton", 0);
        int fifthButton = PlayerPrefs.GetInt("fifthButton", 0);

        levelReachedWas5 = PlayerPrefs.GetInt("levelReachedWas5", 0);

        Navigation nav = new Navigation();
        nav.mode = Navigation.Mode.None;

        Navigation navRight = new Navigation();
        navRight.mode = Navigation.Mode.Explicit;
        navRight.selectOnDown = returnButton;

        for (int i = 0; i < levelButtons.Length; i++)
        {
            navRight.selectOnRight = null;
            navRight.selectOnLeft = null;

            if(i + 1 > levelReached && i != 4)
            {
                levelButtons[i].interactable = false;

                if (levelButtons[i - 1] != levelButtons[0])
                {
                    navRight.selectOnLeft = levelButtons[i - 2];
                }

                levelButtons[i - 1].navigation = navRight;
                levelButtons[i].navigation = nav;
            }

            if(firstButton == 0 || secondButton == 0 || thirdButton == 0 || fourthButton == 0)
            {
                levelButtons[4].interactable = false;

                if (levelButtons[4 - 1] != levelButtons[0])
                {
                    navRight.selectOnLeft = levelButtons[4 - 2];
                }

                levelButtons[4 - 1].navigation = navRight;
                levelButtons[4].navigation = nav;

                
            }
            //else if(levelReachedWas5 != 1)
            //{
            //    levelButtons[4].interactable = false;

            //    if (levelButtons[4 - 1] != levelButtons[0])
            //    {
            //        navRight.selectOnLeft = levelButtons[4 - 2];
            //    }

            //    levelButtons[4 - 1].navigation = navRight;
            //    levelButtons[4].navigation = nav;
            //}
        }

        if(collectable == 1)
        {
            levelButtons[0].gameObject.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
            PlayerPrefs.SetInt("firstButton", 1);
        }
        else if(collectable == 2)
        {
            levelButtons[1].transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
            PlayerPrefs.SetInt("secondButton", 1);
        }
        else if(collectable == 3)
        {
            levelButtons[2].transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
            PlayerPrefs.SetInt("thirdButton", 1);
        }
        else if(collectable == 4)
        {
            levelButtons[3].transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
            PlayerPrefs.SetInt("fourthButton", 1);
        }
        else if (collectable == 5)
        {
            levelButtons[4].transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
            PlayerPrefs.SetInt("fifthButton", 1);
        }

        if (firstButton == 1) levelButtons[0].gameObject.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
        if(secondButton == 1) levelButtons[1].gameObject.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
        if(thirdButton == 1) levelButtons[2].gameObject.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
        if(fourthButton == 1) levelButtons[3].gameObject.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
        if(fifthButton == 1) levelButtons[4].gameObject.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);

        if(levelReachedWas5 == 1 && firstButton == 1 && secondButton == 1 && thirdButton == 1 && fourthButton == 1)
        {

        }
    }

    public void LoadLevelPassed(string levelName)
    {
        StartCoroutine(Wait(levelName));
    }

    IEnumerator Wait(string levelName)
    {
        fadeAnimator.transform.gameObject.SetActive(true);
        fadeAnimator.SetTrigger("FadeIn");

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(levelName);
    }
}
