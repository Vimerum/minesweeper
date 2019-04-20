using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndDialogController : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI endGameText;
    public TextMeshProUGUI timerText;

    private ButtonClick restartCallback;
    private ButtonClick backToMenuCallback;

    public void Initialize (string endGameMessage, float time, ButtonClick restartCallback, ButtonClick backToMenuCallback) {
        endGameText.text = endGameMessage;
        timerText.text = string.Format("Tempo: {0:00}:{1:00}", (Mathf.FloorToInt(time) / 60), (Mathf.Floor(time) % 60));
        this.restartCallback = restartCallback;
        this.backToMenuCallback = backToMenuCallback;
    }

    public void Restart () {
        restartCallback();
        Destroy(gameObject);
    }

    public void BackToMenu () {
        backToMenuCallback();
        Destroy(gameObject);
    }
}
