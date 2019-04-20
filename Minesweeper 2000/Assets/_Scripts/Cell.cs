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
    [HideInInspector]
    public int x, y;

    private SpriteRenderer spriteRenderer;
    private TextMeshPro levelText;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        levelText = GetComponentInChildren<TextMeshPro>();
        Initialize();
    }

    private void OnMouseOver() {
        if (GameManager.instance.state != State.Running) return;

        if (Input.GetMouseButtonDown(0))
            Open(true);
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

    public void Open (bool byClick) {
        if (isFlagged) return;
        if (!isHidden) return;
        
        isHidden = false;

        if (byClick)
            GameManager.instance.CellOppened(x, y, isBomb);

        if (isBomb) {
            spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Bomb Cell");
        }
        else {
            spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Empty Cell");
            if (level > 0) {
                levelText.text = level.ToString();
            }
        }
    }

    private void Flag () {
        if (!isHidden) return;

        isFlagged = !isFlagged;
        GameManager.instance.CellMarked(isFlagged, x, y, isBomb);

        if (isFlagged)
            spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Marked Cell");
        else
            spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Blank Cell");
    }
}
