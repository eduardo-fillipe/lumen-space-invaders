using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Resumo de informações sobre um jogador específicos
/// </summary>
public class PlayerStats : MonoBehaviour
{

    public bool canShoot = true; //Guarda se ele pode atirar
    private int score = 0; //Pontuação atual, é acrescida quando um inimigo é derrotado ou quando o boss é atingido
    private int qtVidas = 3; // Quantas vidas o jogador possui
    public int Score { get => score; set => score = value; }
    public int QtVidas { get => qtVidas; set => qtVidas = value; }
    public Player player; //referência para o jogador que possui as estatísticas dessa classe.
}
