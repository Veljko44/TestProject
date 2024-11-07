using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddPuzzle : MonoBehaviour
{
    public Transform puzzleBoard;
    public GameObject puzzlePartButton;
    public int puzzleParts;
    public static AddPuzzle instance;
    public GridLayoutGroup gridLayout;

    public void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void CreatePuzzleParts()
    {
        for (int i = 0; i < puzzleParts; i++)
        {
            GameObject puzzlePart = Instantiate(puzzlePartButton);
            puzzlePart.name = "" + i;
            puzzlePart.transform.SetParent(puzzleBoard, false);
        }
    }
}
