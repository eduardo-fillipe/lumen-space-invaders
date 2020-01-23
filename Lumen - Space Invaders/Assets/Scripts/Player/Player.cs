using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe que controla um determinado player.
/// </summary>
public class Player : MonoBehaviour
{
    private Rigidbody2D rigidBody = null;

    //Limite de movimentação do personagem
    public  float rightLimit = 8.2f; 
    public float leftLimit = -8.2f;

    [SerializeField]
    private float velocidade = 0; //Regula a velocidade de ele atinge

    public GameObject projectile; //Prefab do projétil que ele atira

    public PlayerStats playerStats; //Referência para as estatísticas desse player.

    public Sprite playerDeadSprite; //Sprete utilziada para indicar que o player usou uma vida ou que ele morreu.

    void Start()
    {
        this.rigidBody = this.GetComponent<Rigidbody2D>();
    }


    void Update()
    {
        if (Camera.main.GetComponent<GameState>().CurrentState == GameState.GameStateEnum.STARTED) { //Se o jogo começou
            if (playerStats.QtVidas > 0) //Se o jogador está vivo
            {
                //Verifica os Inputs e move o jogador conforme
                if (Input.GetKey(KeyCode.RightArrow) && rigidBody.transform.position.x < rightLimit) 
                {
                    rigidBody.AddForce(Vector2.right * this.velocidade, ForceMode2D.Force);
                }

                if (Input.GetKey(KeyCode.LeftArrow) && rigidBody.transform.position.x > leftLimit)
                {
                    rigidBody.AddForce(Vector2.left * this.velocidade, ForceMode2D.Force);
                }

                //Se o jogador apertou espaço e pode atirar
                if (Input.GetKey(KeyCode.Space) && canShoot())
                {
                    shoot(); //Atire
                }
            }
        }
    }
    /// <summary>
    /// Realiza a ação de atirar
    /// </summary>
    void shoot() {
        Instantiate(projectile, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 0.3f), Quaternion.identity); //Instancia um novo projétil
    }

    /// <summary>
    /// Retorna se o jogador pode atirar
    /// </summary>
    /// <returns></returns>
    bool canShoot() {
        return this.playerStats.canShoot && playerStats.QtVidas > 0; 
    }

    /// <summary>
    /// A cada colisão, verifica se ele foi atingido por algum tipo de inimigo e diminui 1 na quantidade de vidas.
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.tag == "EnemyProjectile") //Foi atingido por um inimigo?
        {
            playerStats.QtVidas --; //Reduza a quantidade de vidas
            if (playerStats.QtVidas == 0) //A quantidade de vidas é 0? 
            {
                this.gameObject.GetComponent<SpriteRenderer>().sprite = playerDeadSprite; //Altere a sprite para morto.
                this.gameObject.GetComponent<Collider2D>().enabled = false; //Desabilite o collider.
            }
        }
    }
}
