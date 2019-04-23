using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogPanelController : MonoBehaviour {

    [HideInInspector]
    public static DialogPanelController instance;
    
    [Header("Messages")]
    public List<Message> dialog;
    [Header("Dialog settings")]
    public TextMeshProUGUI dialogText;

    private Animator anim;

    private TutorialCallback callback;
    private int currentIndex;

    private void Start() {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }

        instance = this;
        anim = GetComponent<Animator>();
    }

    public void Initialize(List<Message> dialog, TutorialCallback callback) {
        currentIndex = 0;
        this.dialog = dialog;
        this.callback = callback;
    }

    public void Begin() {
        if (dialog.Count <= 0) return;

        Next();
    }

    public void Next() {
        if (currentIndex >= dialog.Count) {
            End();
            return;
        }

        if (dialog[currentIndex].position > 0) {
            anim.SetBool("Up", true);
            anim.SetBool("Down", false);
        } else {
            anim.SetBool("Up", false);
            anim.SetBool("Down", true);
        }

        dialog[currentIndex].Invoke();
        dialogText.text = dialog[currentIndex++].message;
    }

    public void End () {
        anim.SetBool("In", false);
        callback();
    }
}
