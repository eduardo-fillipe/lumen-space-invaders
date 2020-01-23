using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controla o estado do jogo: STARTED, PAUSED, STOPPED, OVER
/// </summary>
public class GameState : MonoBehaviour
{
    /// <summary>
    /// Estado atual do jogo
    /// </summary>
    private GameStateEnum currentState = GameStateEnum.STOPPED;

    public GameStateEnum CurrentState { get => currentState; set => currentState = value; }

    /// <summary>
    /// Representa os estados do jogo
    /// </summary>
    public enum GameStateEnum
    {
        STARTED, PAUSED, STOPPED, OVER
    }

    // Update is called once per frame
    void Update()
    { 
        if (Input.GetKeyDown(KeyCode.Return) && currentState == GameStateEnum.STOPPED) //Se apertar Enter e o jogo não começou
        {
            currentState = GameStateEnum.STARTED; //Altere o estado atual
            Camera.main.GetComponent<CanvasController>().setMessage(""); //Mude a mensagem do rodapé
        } 
    }
}
