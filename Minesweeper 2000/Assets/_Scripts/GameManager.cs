using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Grid Configuration")]
    public Vector2 gridSize;
    public int numberBombs;
    [Header("Grid Configuration")]
    public GameObject cellPrefab;

    private List<GameObject> cells;

    public void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public void Start() {
        Initialize();
    }

    private void Initialize () {
        cells = new List<GameObject>();
        for (int y = 0; y > -gridSize.y; y--) {
            for (int x = 0; x < gridSize.x; x++) {
                GameObject go = Instantiate(cellPrefab, new Vector3(transform.position.x + x, transform.position.y + y), Quaternion.identity, transform);
                cells.Add(go);
            }
        }
    }

    public void GameOver () {
        Debug.Log("GAME OVER!!");
    }
}
