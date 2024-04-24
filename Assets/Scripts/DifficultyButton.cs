using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyButton : MonoBehaviour
{
    private Button button;
    private GameManager gameManager;

    public int difficulty;

    void Start()
    {
        // get references
        button = GetComponent<Button>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        // add set difficulty method to on click
        button.onClick.AddListener(SetDifficulty);
    }

    private void SetDifficulty()
    {
        // start game with designated difficulty
        gameManager.StartGame(difficulty);
    }
}
