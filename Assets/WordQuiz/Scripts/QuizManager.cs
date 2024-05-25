using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[Serializable]
public class QuestionCollection
{
    public Questions[] LOLQuestions;
    public Questions[] CountryQuestions;
    public Questions[] AnimeQuestions;
    public QuestionCount[] Counts;
    public QuestionCategoryCounts[] CategoryCount;
}

[Serializable]
public class Questions
{
    public string ImageURL;
    public string Answer;
}

[Serializable]
public class QuestionCount
{
    public int Count;
    public bool isTrue;
}

[Serializable]
public class QuestionCategoryCounts
{
    public int AllCategories;
}
[System.Serializable]
public class SaveInfo
{
    public Button Buttons;
    public Text QuestionCountsText;
    public string QuestionId;
    public string EndedCategoryId;
    public string LastQuestionId;
    public int CQI;
    public Slider Slider;
}
public class QuizManager : MonoBehaviour
{
    public static QuizManager instance; //Instance to make is available in other scripts without reference

    //Scriptable data which store our questions data
    [SerializeField]
    public RawImage questionImage;           //image element to show the image

    [SerializeField]
    public WordData[] answerWordList;     //list of answers word in the game

    [SerializeField]
    private WordData[] optionsWordList;    //list of options word in the game

    private GameStatus gameStatus = GameStatus.Playing;     //to keep track of game status
    private char[] wordsArray = new char[24];               //array which store char of each options

    public int[] QuestionCount;
    private List<int> selectedWordsIndex;                   //list which keep track of option word index w.r.t answer word index
    public int currentAnswerIndex = 0;  //index to keep track of current answer and current question
    private bool correctAnswer = true;                      //bool to decide if answer is correct or not
    public string answerWord;                              //string to store answer of current question
    private string jsonURL = "https://drive.google.com/uc?export=download&id=1iCzJycorvMWRpD-ohzFCmT7KiU7Ozu3_";
    public int CurrentCategoryID;
    public bool[] isCategoryTrue;
    public int allCategoryCounts=3;
    public GameObject Menu;
    public GameObject LevelSelection;
    public GameObject GamePanel;
    public GameObject LoadingScene;
    public GameObject CategoryEndPanel;
    public GameObject SettingsPanel;
    public GameObject Opening;
    public GameObject Skills;
    public GameObject AnswerHolder;
    public Image LoadSlider;
    public int coins = 0;
    public Text Cointext;
    public SaveInfo[] Saves;
    public Text loadingPercent;
    public int currentLetter = 0;
    public int[] OpenedLetters;
    public Vector3 vector = new Vector3(1.5f, 1.5f, 1.5f);

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
        OpeningAnimation();
        StartCoroutine(GetUpdate(jsonURL));
        GetSaves();
    }

    private void Start()
    {
        coins = PlayerPrefs.GetInt("Coins", 0);
        Cointext.text = coins.ToString();
    }

    public void GetSaves()
    {
        for (int i = 0; i < allCategoryCounts; i++)
        {
            Saves[i].CQI = PlayerPrefs.GetInt(Saves[i].QuestionId, 0);
            Saves[i].QuestionCountsText.text = PlayerPrefs.GetInt(Saves[i].QuestionId, 0) + "/" + QuestionCount[i];
            Saves[i].Slider.maxValue = QuestionCount[i];
            Saves[i].Slider.value = PlayerPrefs.GetInt(Saves[i].QuestionId, 0);
        }
    }

    public void OpenHintMenu()
    {
        FindObjectOfType<AudioManager>().Play("Button");
        Vector3 vector = new Vector3(1, 1, 1);
        Skills.LeanScale(vector, 0.5f).setEaseOutBack();
    }

    public void CloseHintMenu()
    {
        FindObjectOfType<AudioManager>().Play("Button");
        Vector3 vector = new Vector3(0, 0, 0);
        Skills.LeanScale(vector, 0.5f).setEaseInBack();
    }
    public void ScaleNormalize()
    {
        Vector3 vector = new Vector3(1, 1, 1);
        Cointext.transform.LeanScale(vector, 0.3f).setEaseInBack();
    }

    public void OneLetter()
    {
        Cointext.transform.LeanScale(vector, 0.3f).setEaseOutBack().setLoopPingPong(1).setOnComplete(ScaleNormalize);
        if (coins>=100)
        {
            FindObjectOfType<AudioManager>().Play("Button");
            coins -= 100;
            Cointext.text = coins.ToString();
            ResetQuestion();
            currentLetter++;
            FindObjectOfType<AudioManager>().Play("Answer");
            for (int i = 0; i < currentLetter; i++)
            {
                answerWordList[i].SetWord(char.ToUpper( answerWord[i]));
            }
            if (currentLetter == answerWord.Length)
            {
                currentLetter = 0;
                Saves[CurrentCategoryID].CQI++;
                SaveProgress(CurrentCategoryID);
                LoadNextQuestion();
                Invoke("SetQuestion", 1.5F);
                FindObjectOfType<AudioManager>().Play("NextLevel");
            }
            if (answerWord[currentLetter]=='/')
            {
                currentLetter++;
            }
            currentAnswerIndex += currentLetter;
            
        }
        else
        {
            FindObjectOfType<AudioManager>().Play("WrongAnswer");
            //reklam izleme bildirimi aç
        }
    }

    public void SkipQuestion()
    {
        Cointext.transform.LeanScale(vector, 0.5f).setEaseOutBack().setLoopPingPong(1).setOnComplete(ScaleNormalize);
        if (coins >= 500)
        {
            FindObjectOfType<AudioManager>().Play("Button");
            coins -= 500;
            Cointext.text = coins.ToString();
            currentLetter = 0;
            Saves[CurrentCategoryID].CQI++;
            SaveProgress(CurrentCategoryID);
            LoadNextQuestion();
            Invoke("SetQuestion", 1.5F);
            FindObjectOfType<AudioManager>().Play("NextLevel");
            

        }
        else
        {
            FindObjectOfType<AudioManager>().Play("WrongAnswer");
            //reklam izleme bildirimi aç
        }
    }

    public void OpeningAnimation()
    {
        Opening.LeanMoveLocalY(Screen.height*3, 1.5f).setEaseInQuint().setOnComplete(
                    () => {
                        Destroy(Opening);
                    }
                );
    }
    public void BackToLevelSelection()
    {
        GetSaves();
        FindObjectOfType<AudioManager>().Play("Button");
        LevelSelection.LeanMoveLocalX(0, 0);
        
        CategoryEndPanel.SetActive(false);

    }
    public void BackToMenu()
    {
        FindObjectOfType<AudioManager>().Play("Button");
        Menu.LeanMoveLocalX(0, 0.5f).setEaseOutQuint();
    }
    public void QuitGame()
    {
        FindObjectOfType<AudioManager>().Play("Button");
        GetSaves();
        LevelSelection.LeanMoveLocalX(0, 0.5f);
    }
    public void CategorySelector()
    {
        FindObjectOfType<AudioManager>().Play("Button");
        GetSaves();
        Menu.LeanMoveLocalX(-Screen.width*2, 0.5f);
    }

    public void SettingScreen()
    {
        FindObjectOfType<AudioManager>().Play("Button");
        Vector3 vector = new Vector3(1, 1, 1);
        SettingsPanel.LeanScale(vector, 0.5f).setEaseOutBack();
    }
    public void CloseSettingScreen()
    {
        FindObjectOfType<AudioManager>().Play("Button");
        Vector3 vector = new Vector3(0, 0, 0);
        SettingsPanel.LeanScale(vector, 0.5f).setEaseInBack();
    }

    public void QuizChanger()
    {
        
        LoadNextQuestion();
        selectedWordsIndex = new List<int>();           //create a new list at start
        Invoke("SetQuestion", 3f);                             //set question
        
    }
    public double loadtime=0;
    public double changepersecond = 22.22;
    public bool update = false;
    public void Update()
    {
        if (loadtime>=100)
        {
            
        }
        else if (update)
        {
            loadingPercent.text = "%" + (int)loadtime;
            loadtime += changepersecond * Time.deltaTime;
            
        }
        
    }
    IEnumerator CategoryStarter(int id)
    {
        //loadtime = 0;
        
        Saves[id].CQI = PlayerPrefs.GetInt(Saves[id].QuestionId, 0);
        if (Saves[id].CQI  >= QuestionCount[id])
        {
            CategoryEnd(id);
        }
        else
        {
            update = true;
            LoadingScene.LeanMoveLocalY(0, 0.5f).setEaseInBack();
            FindObjectOfType<AudioManager>().Play("Loading");
            LoadSlider.transform.LeanMoveLocalX(0, 4.5f).setEaseLinear();
            QuizChanger();
            yield return new WaitForSeconds(4.5f);
            FindObjectOfType<AudioManager>().Play("Loading");
            LoadingScene.LeanMoveLocalY(Screen.height * 2, 0.5f).setEaseInExpo().setOnComplete(
                        () => {
                            loadtime = 0;
                        }
                    );
            LevelSelection.LeanMoveLocalX(Screen.width * 2, 0);
            LoadSlider.transform.LeanMoveLocalX(-Screen.width + 120, 2).setEaseInCirc();
            update = false;
        }
    }
    public void LolQuiz()
    {
        FindObjectOfType<AudioManager>().Play("Button");
        CurrentCategoryID = 0;
        StartCoroutine(CategoryStarter(CurrentCategoryID));

    }
    public void CountryQuiz()
    {
        FindObjectOfType<AudioManager>().Play("Button");
        CurrentCategoryID = 1;
        StartCoroutine(CategoryStarter(CurrentCategoryID));
    }
    
    public void AnimeQuiz()
    {
        FindObjectOfType<AudioManager>().Play("Button");
        CurrentCategoryID = 2;
        StartCoroutine(CategoryStarter(CurrentCategoryID));
    }

    public void SaveProgress(int id)
    {
        PlayerPrefs.SetInt(Saves[id].QuestionId, Saves[id].CQI);
        GetSaves();
        coins += 10;
        Cointext.text =coins.ToString();
        PlayerPrefs.SetInt("Coins", coins);
    }
    public void LoadNextQuestion()
    {
        StartCoroutine(GetData(jsonURL));
    }

    public IEnumerator GetUpdate(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            print("Error");
        }
        else
        {
            QuestionCollection data = JsonUtility.FromJson<QuestionCollection>(request.downloadHandler.text);
           // allCategoryCounts = data.CategoryCount.Count();
            for (int i = 0; i < 3; i++)
            {
                isCategoryTrue[i] = data.Counts[i].isTrue;
                Saves[i].Buttons.gameObject.SetActive(data.Counts[i].isTrue);
                QuestionCount[i] = data.Counts[i].Count;
            }
            
        }
        request.Dispose();
    }

    public IEnumerator GetData(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            print("Error");
        }
        else
        {
            QuestionCollection data = JsonUtility.FromJson<QuestionCollection>(request.downloadHandler.text);
            // print data in UI
            if (CurrentCategoryID == 0)
            {
                answerWord = data.LOLQuestions[Saves[CurrentCategoryID].CQI].Answer;
                StartCoroutine(GetImage(data.LOLQuestions[Saves[CurrentCategoryID].CQI].ImageURL));
            }
            else if (CurrentCategoryID == 1)
            {
                answerWord = data.CountryQuestions[Saves[CurrentCategoryID].CQI].Answer;
                StartCoroutine(GetImage(data.CountryQuestions[Saves[CurrentCategoryID].CQI].ImageURL));
            }
            else if (CurrentCategoryID == 2)
            {
                answerWord = data.AnimeQuestions[Saves[CurrentCategoryID].CQI].Answer;
                StartCoroutine(GetImage(data.AnimeQuestions[Saves[CurrentCategoryID].CQI].ImageURL));
            }
        }
        request.Dispose();
    }

    public IEnumerator GetImage(string url)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                print("Error");
            }
            else
            {
                //success...
                questionImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                // Clean up any resources it is using.
                request.Dispose();
            }
        }
    }

    public void SetQuestion()
    {
        gameStatus = GameStatus.Playing;                //set GameStatus to playing 
        
        ResetQuestion();                               //reset the answers and options value to orignal    
        selectedWordsIndex.Clear();                     //clear the list for new question
        Array.Clear(wordsArray, 0, wordsArray.Length);  //clear the array
        //add the correct char to the wordsArray
        for (int i = 0; i < answerWord.Length; i++)
        {
            wordsArray[i] = char.ToUpper(answerWord[i]);

            if (answerWord[i]=='/')
            {
                wordsArray[i] = (char)UnityEngine.Random.Range(65, 90);
            }
        }

        //add the dummy char to wordsArray
        for (int j = answerWord.Length; j < wordsArray.Length; j++)
        {
            wordsArray[j] = (char)UnityEngine.Random.Range(65, 90);
        }

        wordsArray = ShuffleList.ShuffleListItems<char>(wordsArray.ToList()).ToArray(); //Randomly Shuffle the words array

        //set the options words Text value
        for (int k = 0; k < optionsWordList.Length; k++)
        {
            optionsWordList[k].SetWord(wordsArray[k]);
        }
    }
    public void WordCheck(int spacecount)
    {
        Vector2 vector2 = new Vector2();
        GridLayoutGroup grid = answerWordList[0].GetComponentInParent<GridLayoutGroup>();
        switch (spacecount)
        {
            case 0:
                if (answerWord.Length > 7)
                {
                    int extrawords = 100 - ((answerWord.Length - 7) * 7);
                    grid.constraintCount = spacecount;
                    vector2.x = extrawords;
                    vector2.y = extrawords;
                    grid.cellSize = vector2;
                }
                else
                {
                    grid.constraintCount = spacecount;
                }
                break;
            case 1:
                if (answerWord.Length <= 7)
                {
                    grid.constraintCount = spacecount ;
                }
                else
                {
                    grid.constraintCount = spacecount+1;
                }
                break;
            case 2:
                if (answerWord.Length <=14)
                {
                    grid.constraintCount = spacecount ;
                }
                else
                {
                    grid.constraintCount = spacecount+1 ;
                }
                break;
            default:
                print("Incorrect intelligence level.");
                break;
        }
    }
    public int[] SpaceIndex;
    private Image temp;
    //Method called on Reset Button click and on new question
    public void ResetQuestion()
    {
        int spacecount = 0;
        //activate all the answerWordList gameobject and set their word to "_"
        for (int i = 0; i < answerWord.Length; i++)
        {
            
            answerWordList[i].gameObject.SetActive(true);
            answerWordList[i].SetWord('_');
            if (answerWord[i] == '/')
            {
                SpaceIndex[spacecount] = i;
                spacecount++;
                answerWordList[i].gameObject.SetActive(true);
                answerWordList[i].SetWord('/');
                temp = answerWordList[i].GetComponentInParent<Image>();
                temp.color = new Color(0, 0, 0, 0);
            }
        }
        WordCheck(spacecount);

        //Now deactivate the unwanted answerWordList gameobject(object more than answer string length)
        for (int i = answerWord.Length; i < answerWordList.Length; i++)
        {
            answerWordList[i].gameObject.SetActive(false);
        }

        //activate all the optionsWordList objects
        for (int i = 0; i < optionsWordList.Length; i++)
        {
            optionsWordList[i].gameObject.SetActive(true);
        }
        currentAnswerIndex = 0;
    }

    public void WrongAnim()
    {
        AnswerHolder.LeanMoveLocalX(0, .1f).setEaseInOutElastic();
    }
    public void SelectedOption(WordData value)
    {
        if (currentAnswerIndex >= answerWord.Length)
        {

        }
        else 
        { 
            if (answerWord[currentAnswerIndex] == '/')
            {
                answerWordList[currentAnswerIndex].SetWord('/');
                currentAnswerIndex++;

            }
            //if gameStatus is next or currentAnswerIndex is more or equal to answerWord length
            if (gameStatus == GameStatus.Next || currentAnswerIndex >= answerWord.Length) return;

            selectedWordsIndex.Add(value.transform.GetSiblingIndex()); //add the child index to selectedWordsIndex list
            value.gameObject.SetActive(false); //deactivate options object
            answerWordList[currentAnswerIndex].SetWord(value.wordValue); //set the answer word list
            FindObjectOfType<AudioManager>().Play("Answer");
            currentAnswerIndex++;
        } 
        

        //if currentAnswerIndex is equal to answerWord length
        if (currentAnswerIndex == answerWord.Length)
        {
            correctAnswer = true;   //default value
            //loop through answerWordList
            for (int i = 0; i < answerWord.Length; i++)
            {
                //if answerWord[i] is not same as answerWordList[i].wordValue
                if (char.ToUpper(answerWord[i]) != char.ToUpper(answerWordList[i].wordValue))
                {
                    correctAnswer = false; //set it false
                    AnswerHolder.LeanMoveLocalX(-20, .1f).setEaseInOutElastic().setLoopPingPong(1).setOnComplete(WrongAnim);
                    ResetQuestion();
                    FindObjectOfType<AudioManager>().Play("WrongAnswer");
                    
                    int j;
                    for (j = 0; j < currentLetter; j++)
                    {
                        answerWordList[j].SetWord(char.ToUpper(answerWord[j]));
                    }
                    currentAnswerIndex = j;
                    //animasyon yap
                    break; //and break from the loop
                }
            }

            if (correctAnswer)
            {
                gameStatus = GameStatus.Next; //set the game status
                FindObjectOfType<AudioManager>().Play("NextLevel");
                AdsManager.instance.RequestInterstitial();
                currentLetter = 0;
                if (Saves[CurrentCategoryID].CQI + 1>=QuestionCount[CurrentCategoryID])
                {
                    Saves[CurrentCategoryID].CQI = QuestionCount[CurrentCategoryID];
                    CategoryEnd(CurrentCategoryID);
                    CategoryEndPanel.SetActive(true);
                    GetSaves();
                }
                else
                {
                    Saves[CurrentCategoryID].CQI++;
                    SaveProgress(CurrentCategoryID);
                    LoadNextQuestion();
                    Invoke("SetQuestion", 1.5F);
                }
                
            }
        }
    }

    public void CategoryEnd(int id)
    {
        PlayerPrefs.SetInt(Saves[id].EndedCategoryId,id);
        PlayerPrefs.SetInt(Saves[id].QuestionId, Saves[id].CQI);
        Saves[id].Buttons.interactable = false;//bittiğini bildirsin
        Saves[id].Buttons.transform.LeanScale(vector, 0.5f).setEaseOutBack().setLoopPingPong(1).setOnComplete(ScaleNormalize);
        
    }

    public void ResetLastWord()
    {
        if (selectedWordsIndex.Count > 0)
        {
            int index = selectedWordsIndex[selectedWordsIndex.Count - 1];
            optionsWordList[index].gameObject.SetActive(true);
            selectedWordsIndex.RemoveAt(selectedWordsIndex.Count - 1);
            
            currentAnswerIndex--;

            if (currentAnswerIndex<currentLetter)
            {
                currentAnswerIndex = currentLetter;
            }
            if (currentAnswerIndex<0 && currentLetter<0)
            {
                currentAnswerIndex = 0;
            }
            
            if (answerWord[currentAnswerIndex]=='/')
            {
                if (currentAnswerIndex==0)
                {

                }
                else
                {
                    currentAnswerIndex--;
                }
                   
            }
            answerWordList[currentAnswerIndex].SetWord('_');
            if (currentLetter >0)
            {
                
                int i;
                for (i = 0; i < currentLetter; i++)
                {
                    answerWordList[i].SetWord(char.ToUpper(answerWord[i]));
                }
            }
        }
        FindObjectOfType<AudioManager>().Play("ResetLastWord");
    }
}
public enum GameStatus
{
   Next,
   Playing
}