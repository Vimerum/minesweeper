using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum State {Running, Paused, Won, Lost};

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Game state")]
    public State state;
    [Header("Grid Configuration")]
    public Vector2 gridSize;
    [Range(0f,1f)]
    public float offset;
    public int numberBombs;
    [Header("Cells Configuration")]
    public GameObject cellPrefab;
    [Header("UI Elements")]
    public TextMeshProUGUI timer;
    public TextMeshProUGUI bombsLeft;

    [HideInInspector]
    public Bounds bounds;

    private GameObject[][] cells;
    private List<GameObject> bombs;

    private int cellsClosed;
    private int cellsOppened;
    private int cellsMarked;
    private float currentTime;

    public void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public void Start() {
        Initialize();
    }

    public void Update() {
        // Update timer
        currentTime += Time.deltaTime;
        Debug.Log(currentTime);
        timer.text = string.Format("{0:00}:{1:00}", (Mathf.FloorToInt(currentTime) / 60), (Mathf.Floor(currentTime) % 60));

        // Update bombs left
        bombsLeft.text = string.Format("{0:d}", numberBombs - cellsMarked);

        // Check if the game has ended with the winning condition
        if (state == State.Running && cellsClosed + cellsMarked == numberBombs)
            Win();
    }

    public void Restart () {
        // TODO: Add confirmation dialog
        foreach (Transform c in transform) {
            Destroy(c.gameObject);
        }
        Initialize();
    }

    private void Initialize () {
        // Set Game State
        state = State.Running;

        // Set standard values
        currentTime = 0f;
        cellsClosed = (int)gridSize.x * (int)gridSize.y;
        cellsOppened = 0;
        cellsMarked = 0;

        // Spawns the cells
        cells = new GameObject[(int)gridSize.y][];
        bounds = new Bounds();
        for (int y = 0; y < gridSize.y; y++) {
            cells[y] = new GameObject[(int)gridSize.x];
            for (int x = 0; x < gridSize.x; x++) {
                cells[y][x] = Instantiate(cellPrefab, new Vector3(transform.position.x + x, transform.position.y + y), Quaternion.identity, transform) as GameObject;
                bounds.Encapsulate(cells[y][x].GetComponent<Collider2D>().bounds);

                Cell c = cells[y][x].GetComponent<Cell>();
                c.x = x;
                c.y = y;
            }
        }
        Vector3 newTopOffset = new Vector3(bounds.size.x, bounds.size.y, 0f);
        newTopOffset.x += (bounds.size.x * 0.05f);
        newTopOffset.y += (bounds.size.y * offset);
        bounds.Encapsulate(newTopOffset);

        Vector3 newBottomOffset = new Vector3(-(bounds.size.x * 0.05f), -(bounds.size.y * 0.05f), 0f);
        bounds.Encapsulate(newBottomOffset);

        // Set with of them is the cells
        bombs = new List<GameObject>();
        for (int i = 0; i < numberBombs; i++) {
            int bombX, bombY;
            do {
                bombX = Random.Range(0, (int)gridSize.x);
                bombY = Random.Range(0, (int)gridSize.y);
            } while (bombs.Contains(cells[bombY][bombX]));
            bombs.Add(cells[bombY][bombX]);
            SetBomb(cells[bombY][bombX]);
            UpdateNeighbours(bombX, bombY);
        }
    }

    private void SetBomb (GameObject bomb) {
        Cell cell = bomb.GetComponent<Cell>();
        if (cell != null)
            cell.isBomb = true;
    }

    private void UpdateNeighbours (int bombX, int bombY) {
        for (int x = bombX - 1; x <= bombX + 1; x++) {
            for (int y = bombY - 1; y <= bombY + 1; y++) {
                if (x == bombX && y == bombY) continue;
                if (x < 0 || x >= gridSize.x) continue;
                if (y < 0 || y >= gridSize.y) continue;

                Cell c = cells[y][x].GetComponent<Cell>();
                c.level++;
            }
        }
    }

    public void CellOppened (int x, int y, bool isBomb) {
        if (isBomb)
            GameOver();
        cellsClosed--;
        cellsOppened++;
        Open(x, y);
    }

    public void Open (int cellX, int cellY) {
        if (cells[cellY][cellX].GetComponent<Cell>().level > 0) return;

        for (int x = cellX - 1; x <= cellX + 1; x++) {
            for (int y = cellY - 1; y <= cellY + 1; y++) {
                if (x == cellX && y == cellY) continue;
                if (x < 0 || x >= gridSize.x) continue;
                if (y < 0 || y >= gridSize.y) continue;

                Cell c = cells[y][x].GetComponent<Cell>();
                if (c.isHidden && !c.isFlagged) {
                    c.Open(false);
                    cellsClosed--;
                    cellsOppened++;
                    if (c.level == 0) {
                        Open(x, y);
                    }
                }
            }
        }
    }

    public void CellMarked(bool isFlagged, int x, int y, bool isBomb) {
        if (isFlagged) {
            cellsClosed--;
            cellsMarked++;
        } else {
            cellsClosed++;
            cellsMarked--;
        }
    }

    public void Win () {
        Debug.Log("Win!!!!");
        state = State.Won;
    }

    public void GameOver () {
        Debug.Log("GAME OVER!!");
        state = State.Lost;

        // Reveal all cells
        for (int x = 0; x < gridSize.x; x++) {
            for (int y = 0; y < gridSize.y; y++) {
                Cell c = cells[y][x].GetComponent<Cell>();
                c.Open(false);
            }
        }
    }
}
