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

    private bool settings = false;
    private bool difficult = false;
    private bool custom = false;

    private void Start() {
        anim = GetComponent<Animator>();
    }

    private void Update() {
        anim.SetBool("Settings", settings);
        anim.SetBool("Difficult", difficult);
        anim.SetBool("Custom", custom);
    }

    public void Settings (bool settings) {
        difficult = false;
        this.settings = settings;
    }

    public void Difficult (bool difficult) {
        settings = false;
        this.difficult = difficult;
    }

    public void Easy () {
        // Set GameManager
        GameManager.instance.gridSize = new Vector2(20,10);
        GameManager.instance.numberBombs = 10;

        // Start Game
        anim.SetTrigger("StartGame");
    }

    public void Medium () {
        // Set GameManager
        GameManager.instance.gridSize = new Vector2(30, 10);
        GameManager.instance.numberBombs = 15;

        // Start Game
        anim.SetTrigger("StartGame");
    }

    public void Hard () {
        // Set GameManager
        GameManager.instance.gridSize = new Vector2(30, 20);
        GameManager.instance.numberBombs = 20;

        // Start Game
        anim.SetTrigger("StartGame");
    }

    public void StartCustomGame () {
        try {
            // Checks if the values are correct
            float x = float.Parse(gridX.text);
            float y = float.Parse(gridY.text);
            int bombs = int.Parse(numberBombs.text);

            if (x <= 0 || y <= 0 || x > 100 || y > 100 || bombs >= (x * y)) return;

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
        SceneManager.LoadScene("GameScene");
    }

    public void Custom (bool custom) {
        this.custom = custom;
    }

    public void QuitGame () {
        Application.Quit();
        Debug.Log("Bye bye");
    }
}
