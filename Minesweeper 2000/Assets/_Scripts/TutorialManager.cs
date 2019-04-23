using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public delegate void MessageCallback(int index);
public delegate void TutorialCallback();

public class TutorialManager : MonoBehaviour
{
    [Header("Initial grid setup")]
    public List<Vector2> initialBombs;

    private Animator anim;
    private int stage = 0;
    private int nextPart = 0;

    private void Start() {
        anim = GetComponent<Animator>();
        stage = 1;
        InitiateGameManager();
        Tutorial();
    }

    private void Update() {
        Tutorial();
    }

    private void InitiateGameManager () {
        foreach (Vector2 v in initialBombs) {
            GameManager.instance.bombs.Add(GameManager.instance.cells[(int)v.y][(int)v.x]);
            GameManager.instance.SetBomb(GameManager.instance.cells[(int)v.y][(int)v.x]);
            GameManager.instance.UpdateNeighbours((int)v.x, (int)v.y);
        }
    }

    private void Tutorial () {

        switch (stage) {
            case 1: { // Fisrt part of tutorial: the objectives
                    GameManager.instance.state = State.Paused;
                    Debug.Log("Fisrt part");
                    // The messages
                    List<Message> messages = new List<Message>();
                    messages.Add(new Message("Olá! Seja bem-vindo(a) ao Minesweeper 2000!", 0));
                    messages.Add(new Message("Primeiro vamos ao objetivo do jogo.", 0));
                    messages.Add(new Message("Essa parte é facíl, marcar em quais quadrados estão todas as bombas, sem explodir nenhuma delas.", 0));

                    stage = -1;
                    nextPart = 2;
                    DialogPanelController.instance.Initialize(messages, Continue);
                    DialogPanelController.instance.Begin();
                    break;
                }
            case 2: { // Second part of the tutorial: the interface
                    Debug.Log("Second part");
                    // The messages
                    List<Message> messages = new List<Message>();
                    messages.Add(new Message("Mas antes, vamos entender a interface.", 0));
                    messages.Add(new Message("Esse é o cronometro. Serve para marcar quanto tempo você demorou para completar a partida.", 0, 1, Focus));
                    messages.Add(new Message("Esse contador serve para mostrar quantas bombas ainda faltam marcar.", 0, 2, Focus));
                    messages.Add(new Message("Mas atenção, ele sempre irá diminuir quando um quadrado for marcado, independente se realmente houver uma bomba nele.", 0, 2, Focus));
                    messages.Add(new Message("Esse botão serve para voltar para o menu inicial.", 0, 3, Focus));
                    messages.Add(new Message("E esse serve para reiniciar a partida atual, mas as bombas irão mudar de posição.", 0, 4, Focus));
                    messages.Add(new Message("Por fim, esse é o seu campo, onde estão as bombas a serem descobertas.", 1, 5, Focus));
                    
                    stage = -1;
                    nextPart = 3;
                    DialogPanelController.instance.Initialize(messages, Continue);
                    DialogPanelController.instance.Begin();
                    break;
                }
            case 3: { // Third part of the tutorial: the rules
                    Debug.Log("Third part");
                    // The messages
                    List<Message> messages = new List<Message>();
                    messages.Add(new Message("O campo começa com todos os quadrados fechados.", 0));
                    messages.Add(new Message("Para abrir um quadrado basta clicar sobre ele com o botão direito.", 0, 6, Focus));
                    messages.Add(new Message("Após aberto, um quadrado pode ter três formas.", 0, 7, Focus));
                    messages.Add(new Message("A primeira é a de um quadrado vazio.", 0, 8, Focus));
                    messages.Add(new Message("A segunda é a de um quadrado com uma bomba.", 0, 9, Focus));
                    messages.Add(new Message("E a terceira é a de um quadrado próximo de uma bomba. Ele possui um número que vai de 1 até 8.", 0, 10, Focus));
                    messages.Add(new Message("Esse número indica quantas bombas existem em volta dessa quadrado", 0, 10, Focus));
                    messages.Add(new Message("Também podemos marcar um quadrado fechado, clicando com o botão direito sobre ele.", 0, 11, Focus));
                    messages.Add(new Message("Dessa forma, o quadrado fica bloqueado, evitando cliques acidentais.", 0, 12, Focus));
                    messages.Add(new Message("Para desmarcar um quadrado, basta clicar novamente sobre ele com o botão direito.", 0, 11, Focus));

                    stage = -1;
                    nextPart = 4;
                    DialogPanelController.instance.Initialize(messages, Continue);
                    DialogPanelController.instance.Begin();
                    break;
                }
            case 4: { // Fourth part of the tutorial: the ending
                    Debug.Log("Fourth part");
                    // The messages
                    List<Message> messages = new List<Message>();
                    messages.Add(new Message("Pronto!", 0));

                    stage = -1;
                    nextPart = -1;
                    DialogPanelController.instance.Initialize(messages, Continue);
                    DialogPanelController.instance.Begin();
                    break;
                }
        }

    }

    public void Continue () {
        // Reset all variables in the animator
        anim.SetInteger("Stage", 0);

        // Set the next part of the tutorial
        if (nextPart > 0) {
            stage = nextPart;
            nextPart = 0;
        } else if (nextPart < 0) {
            // Ends the tutorialTutorialScene
            Destroy(GameManager.instance.gameObject);
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void Focus (int index) {
        anim.SetInteger("Stage", index);
    }

    public void ChangeState (int state) {
        if (state > 0) GameManager.instance.state = State.Running;
        else GameManager.instance.state = State.Paused;
    }
}
