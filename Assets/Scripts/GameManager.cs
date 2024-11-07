using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject difficultyMenu;
    private string currentDifficulty;
    public List<Button> buttons = new List<Button>();
    public Sprite bgImage;
    public Sprite[] puzzles;
    public List<Sprite> gamePuzzles = new List<Sprite>();
    private List<int> picksQueue = new List<int>();
    private int countPicks;
    private int countCorrectPicks;
    private int gamePicks;
    private bool canPick = false;

    void Start()
    {
        difficultyMenu.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
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
        difficultyMenu.SetActive(false);
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


    IEnumerator CheckMatch(int firstIndex, int secondIndex)
    {
        yield return new WaitForSeconds(0.5f);

        if (gamePuzzles[firstIndex].name == gamePuzzles[secondIndex].name)
        {

            buttons[firstIndex].interactable = false;
            buttons[secondIndex].interactable = false;
            float duration = 1f;
            float elapsed = 0f;

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
            CheckIfTheGameIsFinished();
        }
        else
        {
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
            ResetGame();
        }
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
