using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Classe que controla os projéteis lançados por inimigos comuns.
/// </summary>
public class EnemyShoot : MonoBehaviour
{
    private InimigoControll inimigoControll; //Referência para o objeto que controla os inimigos da cena.
    private Rigidbody2D rigidBody;
    public float velocidade; //Velocidade desse projétil

    private void Awake()
    {
        this.inimigoControll = Camera.main.gameObject.GetComponent<InimigoControll>(); //Inicializa o componente controller
        //(Os objetos de controle foram centralizadas na câmera, pois ela é um objeto que não é alterada no jogo.)
    }

    /// <summary>
    /// A cada colisão, o projétil é destruído e a classe de controle é avisa, pois pode haver somente 1 projétil instanciado
    /// por vez na cena, a mesma restrição acontece para projéteis do jogador.
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(this.gameObject);
        inimigoControll.CanShoot = true;
    }
}
