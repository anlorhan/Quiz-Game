﻿using UnityEngine;
using UnityEngine.UI;

public class WordData : MonoBehaviour
{
    [SerializeField] 
    private Text wordText;

    [HideInInspector]
    public char wordValue;

    private Button buttonComponent;

    public void Awake()
    {
        buttonComponent = GetComponent<Button>();
        if (buttonComponent)
        {
            buttonComponent.onClick.AddListener(() => WordSelected());
        }
    }

    public void SetWord(char value)
    {
        wordText.text = value + "";
        wordValue = value;
    }

    public void WordSelected()
    {
        QuizManager.instance.SelectedOption(this);
    }
    
}
