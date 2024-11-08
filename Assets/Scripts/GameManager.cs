using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using JetBrains.Annotations;


public class GameManager : MonoBehaviour
{
    public string currentDifficulty;
    public List<Button> buttons = new List<Button>();
    public Sprite bgImage;
    public Sprite[] puzzles;
    public List<Sprite> gamePuzzles = new List<Sprite>();
    private List<int> picksQueue = new List<int>();
    private int countPicks;
    private int countCorrectPicks;
    private int gamePicks;
    private bool canPick = false;
    public int score = 0;
    public Text scoreText;
    public int combo = 0;
    public int comboBonus = 10;
    public Text comboText;
    public bool isFirstMatch = true;
    public GameObject dificultyPanel;
    public GameObject gamePanel;
    public int highScore = 0;
    public Text highScoreText;
    public static GameManager instance;

    private void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        LoadHighScore();
        UpdateHighScoreUI();
    }
    void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }


    public void SetDifficulty(string difficulty)
    {
        if (difficulty == "Easy")
        {
            AddPuzzle.instance.puzzleParts = 4;
            AddPuzzle.instance.gridLayout.constraintCount = 2;
            AddPuzzle.instance.gridLayout.cellSize = new Vector2(300, 300);
            
        }
        else if (difficulty == "Medium")
        {
            AddPuzzle.instance.puzzleParts = 8;
            AddPuzzle.instance.gridLayout.constraintCount = 4;
            AddPuzzle.instance.gridLayout.cellSize = new Vector2(200, 200);
            
        }
        else if (difficulty == "Hard")
        {
            AddPuzzle.instance.puzzleParts = 12;
            AddPuzzle.instance.gridLayout.constraintCount = 4;
            AddPuzzle.instance.gridLayout.cellSize = new Vector2(200, 200);
           
        }
        dificultyPanel.SetActive(false);
        gamePanel.SetActive(true);
    }
    public void UpdateHighScoreUI()
    {
        if (highScoreText != null)
        {
            highScoreText.text = "HIGHSCORE: " + highScore.ToString();
        }
    }
    public void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "SCORE: " + score.ToString();
        }
    }
    public void UpdateComboUI()
    {
        if (comboText != null)
        {
            if (combo > 0)
            {
                comboText.text = "COMBO: " + combo.ToString();
            }
            else
            {
                comboText.text = "COMBO: 0";
            }
        }
    }
    public void SetLevel()
    {
        SetDifficulty(currentDifficulty);
        AddPuzzle.instance.CreatePuzzleParts();
        GetButtons();
        AddListeners();
        AddGamePuzzles();
        Shuffle(gamePuzzles);
        gamePicks = gamePuzzles.Count / 2;
        StartCoroutine(ShowAllPuzzlesTemporarily());
    }

    public void OnEasyButtonClick()
    {
        currentDifficulty = "Easy";
        SetLevel();
    }

    public void OnMediumButtonClick()
    {
        currentDifficulty = "Medium";
        SetLevel();
    }

    public void OnHardButtonClick()
    {
        currentDifficulty = "Hard";
        SetLevel();
    }

    private void GetButtons()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("PuzzleButton");
        for (int i = 0; i < objects.Length; i++)
        {
            buttons.Add(objects[i].GetComponent<Button>());
            buttons[i].image.sprite = bgImage;
        }
    }

    public void AddListeners()
    {
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => SelectPuzzle());
        }
    }

    public void SelectPuzzle()
    {
        if (!canPick)
        {
            return;
        }

        int selectedIndex = int.Parse(EventSystem.current.currentSelectedGameObject.name);

        if (!picksQueue.Contains(selectedIndex))
        {
            picksQueue.Add(selectedIndex);
            StartCoroutine(FlipCard(buttons[selectedIndex], gamePuzzles[selectedIndex]));

            if (picksQueue.Count == 2)
            {
                StartCoroutine(CheckMatch(picksQueue[0], picksQueue[1]));
                picksQueue.Clear();
            }
        }
    }


    IEnumerator FlipCard(Button button, Sprite newSprite)
    {
        float duration = 0.3f;
        float elapsedTime = 0f;
        SFXSounds.instance.PlaySFX(0);
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float angle = Mathf.Lerp(0, 90, elapsedTime / duration);
            button.transform.localRotation = Quaternion.Euler(0, angle, 0);
            yield return null;
        }
        button.image.sprite = newSprite;
        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float angle = Mathf.Lerp(90, 0, elapsedTime / duration);
            button.transform.localRotation = Quaternion.Euler(0, angle, 0);
            yield return null;
        }
        button.transform.localRotation = Quaternion.identity;
    }

    IEnumerator ShowAllPuzzlesTemporarily()
    {
        canPick = false;
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].image.sprite = gamePuzzles[i];
        }
        yield return new WaitForSeconds(2f);
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].image.sprite = bgImage;
        }

        canPick = true;
    }
    public void ResetGame()
    {
        countPicks = 0;
        countCorrectPicks = 0;
        gamePuzzles.Clear();
        AddGamePuzzles();
        Shuffle(gamePuzzles);
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].image.sprite = bgImage;
            buttons[i].interactable = true;
            buttons[i].image.color = Color.white;
        }
        StartCoroutine(ShowAllPuzzlesTemporarily());
    }
    public void ResetGameState()
    {
        currentDifficulty = "";
        buttons.Clear();
        gamePuzzles.Clear();
        picksQueue.Clear();
        countPicks = 0;
        countCorrectPicks = 0;
        gamePicks = 0;
        canPick = false;
        combo = 0;
        isFirstMatch = true;

        if (scoreText != null)
        {
            scoreText.text = "SCORE: " + score.ToString();
        }
        if (comboText != null)
        {
            comboText.text = "COMBO: 0";
        }

        dificultyPanel.SetActive(true);
        gamePanel.SetActive(false);

        Debug.Log("Game state has been reset.");
    }

    IEnumerator CheckMatch(int firstIndex, int secondIndex)
    {
        yield return new WaitForSeconds(0.5f);

        if (gamePuzzles[firstIndex].name == gamePuzzles[secondIndex].name)
        {
            buttons[firstIndex].interactable = false;
            buttons[secondIndex].interactable = false;
            float duration = 1f;
            float elapsed = 0f;
            SFXSounds.instance.PlaySFX(1);
            Color startColor = buttons[firstIndex].image.color;
            Color targetColor = new Color(0, 0, 0, 0);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                buttons[firstIndex].image.color = Color.Lerp(startColor, targetColor, t);
                buttons[secondIndex].image.color = Color.Lerp(startColor, targetColor, t);

                yield return null;
            }
            buttons[firstIndex].image.color = targetColor;
            buttons[secondIndex].image.color = targetColor;

            countCorrectPicks++;

            score += 5;

            if (!isFirstMatch)
            {
                combo++;
                score += combo * comboBonus;
            }
            else
            {
                isFirstMatch = false;
            }

            UpdateScoreUI();
            UpdateComboUI();
            CheckIfTheGameIsFinished();
        }
        else
        {
            SFXSounds.instance.PlaySFX(2);
            combo = 0;
            isFirstMatch = true;
            UpdateComboUI();
            yield return new WaitForSeconds(0.5f);
            buttons[firstIndex].image.sprite = bgImage;
            buttons[secondIndex].image.sprite = bgImage;
        }

        countPicks++;
    }

    public void CheckIfTheGameIsFinished()
    {
        if (countCorrectPicks == gamePicks)
        {
            Debug.Log("Game Finished");
            Debug.Log("It took you " + countPicks + " guesses to finish the game");
            SFXSounds.instance.PlaySFX(3);

            if (score > highScore)
            {
                highScore = score;
                PlayerPrefs.SetInt("HighScore", highScore);
                PlayerPrefs.Save();
                Debug.Log("New High Score: " + highScore);
            }

            UpdateHighScoreUI();
            StartCoroutine(ShowNextLevelPanelWithDelay());
        }
    }

    private IEnumerator ShowNextLevelPanelWithDelay()
    {
        yield return new WaitForSeconds(1f);
        Panels.instance.NextLevelPanel();
    }

    public void AddGamePuzzles()
    {
        int loop = buttons.Count;
        int index = 0;
        for (int i = 0; i < loop; i++)
        {
            if (index == loop / 2)
            {
                index = 0;
            }
            gamePuzzles.Add(puzzles[index]);
            index++;
        }
    }
    void Shuffle(List<Sprite> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Sprite temp = list[i];
            int randoIndex = Random.Range(i, list.Count);
            list[i] = list[randoIndex];
            list[randoIndex] = temp;
        }
    }
}