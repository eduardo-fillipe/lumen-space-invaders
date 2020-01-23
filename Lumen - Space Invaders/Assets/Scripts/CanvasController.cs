using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Gerencia os elementos de Canvass da cena
/// </summary>
public class CanvasController : MonoBehaviour
{ 
    public Text scorePoints; //Pontos do jogador na tela
    public PlayerStats playerStats; //Referência para o jogador gerenciado
    public Image[] vidas; //Vidas do jogador
    public GameObject gameOverUI; // Tela de gameOver
    public Sprite playerDead; //Sprite do player morto para atualizar na barra de vidas
    public Text messageText; //Text de mensagens ao jogador

    void Start()
    {
        messageText.text = "PRESS [ENTER] TO START!"; //Texto inicial que pede para o jogador começar o jogo.
    }

    void Update()
    { 
        if (playerStats.QtVidas > 0) //O jogador está vivo? Atualize a pontuação dele.
        {
            updateScore(); //Atualiza a pontuação.
        }
        updateVidas(); //Atualiza a quantidade de vidas
    }

    /// <summary>
    /// Atualiza a pontuação do jogador na tela.
    /// </summary>
    void updateScore()
    {
        scorePoints.text = playerStats.Score.ToString();
    }

    /// <summary>
    /// Atualiza a quantidade de vidas do jogador na tela.
    /// </summary>
    void updateVidas()
    {
        if (playerStats.QtVidas < 3) { // Tem menos que 3 vidas?
            vidas[playerStats.QtVidas].sprite = playerDead; //Altere a sprites de vida
        }

        if(playerStats.QtVidas == 0) //Se o player morreu
        { 
            showGameOver(true); //Mostre a tela de gameOver
        }

        if (Input.GetKeyDown(KeyCode.F10) && this.gameObject.GetComponent<GameState>().CurrentState == GameState.GameStateEnum.STARTED) //Se o jogador pressionou F10, vá ao Boss
        {
            showGoToBoss();
        }
    }

    /// <summary>
    /// Exibe ou esconde a tela de gameOver
    /// </summary>
    /// <param name="show"></param>
    public void showGameOver(bool show)
    {
        gameOverUI.SetActive(show);
    }

    /// <summary>
    /// Mata todos os inimigos e instancia o BOSS
    /// </summary>
    void showGoToBoss()
    {
        Debug.Log("Go to Boss");
        this.gameObject.GetComponent<InimigoControll>().killAllEnemies();
        this.gameObject.GetComponent<InimigoControll>().instantiateBoss();
    }

    /// <summary>
    /// Botão de jogar novamente quando o jogo acaba.
    /// </summary>
    public void playAgain()
    {
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Altera a mensagem no rodapé da tela.
    /// </summary>
    /// <param name="text"></param>
    public void setMessage(string text)
    {
        this.messageText.text = text.ToUpper();
    }
}
