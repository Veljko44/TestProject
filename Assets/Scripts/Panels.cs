using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Panels : MonoBehaviour
{
    public GameObject nextLevelPanel;
    public GameObject gamePanel;
    public GameObject optionsPanel;
    public GameObject dificultyPanel;
    public static Panels instance;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void NextLevelPanel()
    {
        gamePanel.SetActive(false);
        optionsPanel.SetActive(false);
        nextLevelPanel.SetActive(true);
        dificultyPanel.SetActive(false);
    }
    public void BackToMainManu()
    {
        SceneManager.LoadScene(0);
    }
    public void PlayGame()
    {
        gamePanel.SetActive(true);
        optionsPanel.SetActive(false);
        nextLevelPanel.SetActive(false);
        dificultyPanel.SetActive(false);
    }
    public void DificultyPanel()
    {
        gamePanel.SetActive(false);
        optionsPanel.SetActive(false);
        nextLevelPanel.SetActive(false);
        dificultyPanel.SetActive(true);
    }
    public void Options()
    {
        gamePanel.SetActive(false);
        optionsPanel.SetActive(true);
        nextLevelPanel.SetActive(false);
        dificultyPanel.SetActive(false);
    }
}
