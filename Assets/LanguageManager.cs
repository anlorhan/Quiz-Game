using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class LanguagePack
{
    public Text texts;
    public string English;
    public string Turkish;
    //0 Settings
    //1 Master Volume:
    //2 Music Volume:
    //3 SFX Volume:
    //4 Loading
    //5 Country
    //6 Completed!
    //7 SHOP
    //8 WATCH VIDEO GET 100 
    //9 SKIP QUESTİON
    //10 OPEN 1 LETTER
}
public class LanguageManager : MonoBehaviour
{
    public static LanguageManager instance;

    public LanguagePack[] texts;
    public GameObject tr;
    public GameObject uk;
    public int wordcount = 11;
    public int languageId=0;

    private void Start()
    {
        CheckLanguage();
    }

    public void CheckLanguage()
    {
        languageId = PlayerPrefs.GetInt("Language", 0);
        if (languageId == 0)//en
        {
            uk.SetActive(true);
            tr.SetActive(false);
            for (int i = 0; i < wordcount; i++)
            {
                texts[i].texts.text = texts[i].English;
            }
        }
        else if (languageId == 1)//tr
        {
            uk.SetActive(false);
            tr.SetActive(true);
            for (int i = 0; i < wordcount; i++)
            {
                texts[i].texts.text = texts[i].Turkish;
            }
        }
    }
    public void ChangeLanguage()
    {
        languageId = PlayerPrefs.GetInt("Language", 0);
        FindObjectOfType<AudioManager>().Play("Button");
        if (languageId==0)
        {
            uk.SetActive(false);
            tr.SetActive(true);
            for (int i = 0; i < wordcount; i++)
            {
                texts[i].texts.text = texts[i].Turkish;
            }

            languageId = 1;
        }
        else if (languageId == 1)
        {
            uk.SetActive(true);
            tr.SetActive(false);
            for (int i = 0; i < wordcount; i++)
            {
                texts[i].texts.text = texts[i].English;
            }
            languageId = 1;
        }
        PlayerPrefs.SetInt("Language", languageId);
    }
}
