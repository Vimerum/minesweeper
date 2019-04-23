using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Playables;

public class GameManagerInitializer : MonoBehaviour
{
    [Header("GameManager")]
    public GameObject gameManager;
    public bool fixedGrid = false;
    [Header("UI Elements for the GM")]
    public TextMeshProUGUI timer;
    public TextMeshProUGUI bombsLeft;
    public Canvas canvas;
    [Header("UI Elements that use GM")]
    public Button menuButton;
    public Button restartButton;

    private PlayableDirector director;

    private bool started = false;

    private void Start() {
        //Gets reference to the Director
        director = GetComponent<PlayableDirector>();

        // Checks if there is a GameManager in the scene, happens when the scene isn't loaded from the menu
        if (GameManager.instance == null)
            Instantiate(gameManager);

        // Sets the values in the GameManager
        bombsLeft.text = GameManager.instance.numberBombs + "";
        GameManager.instance.timer = timer;
        GameManager.instance.bombsLeft = bombsLeft;
        GameManager.instance.canvas = canvas;
        GameManager.instance.fixedGrid = fixedGrid;

        // Setup the values in the UI Elements
        menuButton.onClick.AddListener(() => GameManager.instance.MenuInitializer());
        restartButton.onClick.AddListener(() => GameManager.instance.RestartInitializer());

        // Setup the grid
        GameManager.instance.Setup();
    }

    private void Update() {
        if (!started) {
            if (director.state != PlayState.Playing) {
                // Starts the game when the timeline ends
                started = true;
                GameManager.instance.Initiate();
            }
        }
    }

}
