using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WarningDialogController : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI positiveButtonText;
    public TextMeshProUGUI negativeButtonText;

    private ButtonReturn callback;

    public void Initialize (string title, string message, string positiveAnswer, string negativeAnswer, ButtonReturn callback) {
        titleText.text = title;
        messageText.text = message;
        positiveButtonText.text = positiveAnswer;
        negativeButtonText.text = negativeAnswer;
        this.callback = callback;
    }

    public void ButtonResult (int answer) {
        callback(answer > 0 ? true : false);
        Destroy(gameObject);
    }
}
