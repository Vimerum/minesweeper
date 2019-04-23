using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField gridX;
    public TMP_InputField gridY;
    public TMP_InputField numberBombs;

    private Animator anim;

    private bool startGameCalled = false;
    private bool settings = false;
    private bool difficult = false;
    private bool custom = false;
    private bool tutorial = false;

    private void Start() {
        anim = GetComponent<Animator>();
    }

    private void Update() {
        anim.SetBool("Settings", settings);
        anim.SetBool("Difficult", difficult);
        anim.SetBool("Tutorial", tutorial);
        anim.SetBool("Custom", custom);
    }

    public void Settings (bool settings) {
        difficult = false;
        tutorial = false;
        this.settings = settings;
    }

    public void Difficult (bool difficult) {
        settings = false;
        tutorial = false;
        this.difficult = difficult;
    }

    public void Tutorial () {
        settings = false;
        difficult = false;
        tutorial = true;
        Destroy(GameManager.instance.gameObject);
    }

    public void BeginTutorial () {
        SceneManager.LoadScene("TutorialScene");
    }

    public void Easy () {
        // Set GameManager
        GameManager.instance.gridSize = new Vector2(8,8);
        GameManager.instance.numberBombs = 10;

        // Start Game
        anim.SetTrigger("StartGame");
    }

    public void Medium () {
        // Set GameManager
        GameManager.instance.gridSize = new Vector2(16, 16);
        GameManager.instance.numberBombs = 40;

        // Start Game
        anim.SetTrigger("StartGame");
    }

    public void Hard () {
        // Set GameManager
        GameManager.instance.gridSize = new Vector2(32, 16);
        GameManager.instance.numberBombs = 99;

        // Start Game
        anim.SetTrigger("StartGame");
    }

    public void StartCustomGame () {
        try {
            // Checks if the values are correct
            float x = float.Parse(gridX.text);
            float y = float.Parse(gridY.text);
            int bombs = int.Parse(numberBombs.text);

            if (x <= 0 || y <= 0 || x > 50 || y > 50 || bombs >= (x * y)) return;

            // Set GameManager
            GameManager.instance.gridSize = new Vector2(x, y);
            GameManager.instance.numberBombs = bombs;

            // Starts game
            StartGame();
        } catch (Exception e) {
            Debug.Log(e.Message);
        }
    }

    public void StartGame () {
        if (!startGameCalled) {
            startGameCalled = true;
            SceneManager.LoadScene("GameScene");
        }
    }

    public void Custom (bool custom) {
        this.custom = custom;
    }

    public void QuitGame () {
        Application.Quit();
        Debug.Log("Bye bye");
    }
}
