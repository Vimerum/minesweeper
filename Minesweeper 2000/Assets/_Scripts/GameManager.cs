using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public delegate void ButtonClick();
public delegate void ButtonReturn(bool answer);

public enum State {Running, Paused, Won, Lost};

public class GameManager : MonoBehaviour
{
    #region Public Variables
    public static GameManager instance;

    [Header("Game state")]
    public State state;
    [Header("Grid Configuration")]
    public bool fixedGrid = false;
    public Vector2 gridSize;
    [Range(0f,1f)]
    public float offset;
    public int numberBombs;
    [Header("Cells Configuration")]
    public GameObject cellPrefab;
    [Header("UI Elements")]
    public TextMeshProUGUI timer;
    public TextMeshProUGUI bombsLeft;
    [Header("Dialogs")]
    public Canvas canvas;
    public GameObject warningDialogPrefab;
    public GameObject endGameDialog;

    [HideInInspector]
    public Bounds bounds;
    [HideInInspector]
    public List<GameObject> bombs;
    [HideInInspector]
    public GameObject[][] cells;
    #endregion

    #region Private Variables
    private int cellsClosed;
    private int cellsOppened;
    private int cellsMarked;
    private float currentTime;
    #endregion

    #region MonoBehaviour Callbacks
    public void Awake() {
        // Ensure this is the only instance of this object
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }  else {
            Destroy(this);
        }
    }

    public void Update() {
        if (state == State.Running) {
            // Update timer
            currentTime += Time.deltaTime;
            timer.text = string.Format("{0:00}:{1:00}", (Mathf.FloorToInt(currentTime) / 60), (Mathf.Floor(currentTime) % 60));

            // Update bombs left
            bombsLeft.text = string.Format("{0:d}", numberBombs - cellsMarked);

            // Check if the game has ended with the winning condition
            if (state == State.Running && cellsClosed + cellsMarked == numberBombs)
                Win();
        }
    }
    #endregion

    #region Warning Dialogs
    public void MenuInitializer () {
        // Pauses the game
        state = State.Paused;

        // Instantiate the dialog
        GameObject go = Instantiate(warningDialogPrefab, canvas.transform) as GameObject;
        WarningDialogController controller = go.GetComponent<WarningDialogController>();

        // Stop if something went wrong (it's problably not going to do this)
        if (controller == null) return;

        // Initialize the dialog
        controller.Initialize("Deseja voltar ao menu inicial?", "Se sair da partida, todo o progresso será perdido. Você tem certeza que deseja sair?", "Sair", "Cancelar", ReturnToMenu);
    }

    public void RestartInitializer () {
        // Pauses the game
        state = State.Paused;

        // Instantiate the dialog
        GameObject go = Instantiate(warningDialogPrefab, canvas.transform) as GameObject;
        WarningDialogController controller = go.GetComponent<WarningDialogController>();

        // Stop if something went wrong (it's problably not going to do this)
        if (controller == null) return;

        // Initialize the dialog
        controller.Initialize("Deseja reiniciar?", "Se reiniciar a partida, todo o progresso será perdido. Você tem certeza que deseja reiniciar?", "Reiniciar", "Cancelar", Restart);
    }
    #endregion

    #region State Changers
    public void ReturnToMenu () {
        ReturnToMenu(true);
    }

    public void ReturnToMenu(bool shouldReturn) {
        if (shouldReturn) {
            // Reset the game
            foreach (Transform c in transform) {
                Destroy(c.gameObject);
            }
            // Load main menu scene
            SceneManager.LoadScene("MainMenu");
        } else {
            // Resumes the game
            state = State.Running;
        }
    }

    public void Restart () {
        Restart(true);
    }

    public void Restart (bool shouldRestart) {
        // Resumes the game
        state = State.Running;

        // If the user cancelled, exit the function
        if (!shouldRestart) return;

        // Pauses game
        state = State.Paused;

        // Reset the game
        foreach (Transform c in transform) {
            Destroy(c.gameObject);
        }

        SceneManager.LoadScene("GameScene");
    }
    #endregion

    #region Initializers
    public void Setup() {
        if (timer == null) {
            Debug.LogError("NullReferenceException::GameManager: The variable GameManager.instance.timer is set to null");
            return;
        }
        if (bombsLeft == null) {
            Debug.LogError("NullReferenceException::GameManager: The variable GameManager.instance.bombsLeft is set to null");
            return;
        }
        if (canvas == null) {
            Debug.LogError("NullReferenceException::GameManager: The variable GameManager.instance.canvas is set to null");
            return;
        }
        
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

        // Add the top and right offset
        Vector3 newTopOffset = new Vector3(bounds.size.x, bounds.size.y, 0f);
        newTopOffset.x += (bounds.size.x * 0.05f);
        newTopOffset.y += (bounds.size.y * offset);
        bounds.Encapsulate(newTopOffset);
        // Add the bottom and left offset
        Vector3 newBottomOffset = new Vector3(-(bounds.size.x * 0.05f), -(bounds.size.y * 0.05f), 0f);
        bounds.Encapsulate(newBottomOffset);

        // Set with of them is the cells
        if (!fixedGrid) {
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
    }

    public void Initiate () {
        // Set Game State
        state = State.Running;
    }
    #endregion

    #region Cell Changers
    public void CellOppened(int x, int y, bool isBomb) {
        if (isBomb)
            GameOver();
        cellsClosed--;
        cellsOppened++;
        Open(x, y);
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

    private void Open(int cellX, int cellY) {
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
    #endregion

    #region Grid Setup
    public void SetBomb (GameObject bomb) {
        Cell cell = bomb.GetComponent<Cell>();
        if (cell != null)
            cell.isBomb = true;
    }

    public void UpdateNeighbours (int bombX, int bombY) {
        // Increase the level of every bomb's neighbour by one
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
    #endregion

    #region End Game States
    private void Win () {
        // Changes game state
        state = State.Won;

        // Instantiate Dialog
        GameObject go = Instantiate(endGameDialog, canvas.transform) as GameObject;
        EndDialogController controller = go.GetComponent<EndDialogController>();

        // Stop if something went wrong (it's problably not going to do this)
        if (controller == null) return;

        // Initialize the dialog
        controller.Initialize("Você ganhou o jogo!", currentTime, Restart, ReturnToMenu);
    }

    private void GameOver () {
        // Changes game state
        state = State.Lost;

        // Initiate game over animation
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence () {
        // Reveal bombs
        foreach (GameObject b in bombs) {
            Cell c = b.GetComponent<Cell>();
            c.Open(false);
            yield return new WaitForSeconds(0.2f);
        }
        yield return new WaitForSeconds(0.5f);

        // Reveal all cells
        for (int x = 0; x < gridSize.x; x++) {
            for (int y = 0; y < gridSize.y; y++) {
                Cell c = cells[y][x].GetComponent<Cell>();
                c.Open(false);
            }
        }
        yield return new WaitForSeconds(0.5f);
        
        // Instantiate Dialog
        GameObject go = Instantiate(endGameDialog, canvas.transform) as GameObject;
        EndDialogController controller = go.GetComponent<EndDialogController>();

        // Stop if something went wrong (it's problably not going to do this)
        if (controller != null) {
            // Initialize the dialog
            controller.Initialize("Não foi dessa vez!", currentTime, Restart, ReturnToMenu);
        }
    }
    #endregion
}
