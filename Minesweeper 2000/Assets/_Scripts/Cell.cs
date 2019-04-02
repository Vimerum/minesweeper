using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [HideInInspector]
    public bool isBomb;
    [HideInInspector]
    public bool isHidden;
    [HideInInspector]
    public bool isFlagged;
    [HideInInspector]
    public int level;

    private SpriteRenderer spriteRenderer;
    private TextMeshPro levelText;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        levelText = GetComponentInChildren<TextMeshPro>();
        Initialize();
    }

    private void OnMouseOver() {
        if (Input.GetMouseButtonDown(0))
            Open();
        if (Input.GetMouseButtonDown(1))
            Flag();
    }

    private void Initialize () {
        // TODO: To be removed, since this values will be set by the GameManager
        isBomb = false;
        isHidden = true;
        isFlagged = false;
        level = 0;
    }

    private void Open () {
        if (isFlagged) return;
        if (!isHidden) return;

        Debug.Log("[Cell.Open()]: Cell.Open() called");
        isHidden = false;

        if (isBomb) {
            Debug.Log("[Cell.Open()]: It's a bomb");
            spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Bomb Cell");
            GameManager.instance.GameOver();
        }
        else {
            Debug.Log("[Cell.Open()]: Empty cell");
            spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Empty Cell");
            if (level > 0) {
                Debug.Log("[Cell.Open()]: Has level " + level);
                levelText.text = level.ToString();
            }
        }
    }

    private void Flag () {
        if (!isHidden) return;

        isFlagged = !isFlagged;
        Debug.Log("[Cell.Flag()]: isFlagged=" + isFlagged.ToString());

        if (isFlagged)
            spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Marked Cell");
        else
            spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Blank Cell");
    }
}
