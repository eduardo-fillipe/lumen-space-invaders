using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Classe que controla inimigos de forma individual na cena.
/// </summary>
public class Inimigo : MonoBehaviour
{
    public Sprite[] sprites; //Sprites desse inimigo, elas são trocadas conforme ele se movimenta, para dar o efeito de animação do arcade
    public Sprite[] destroyed; //Sprites que indicam que ele foi destruído
    public float moveDelay; //Delay entre cada passo dado
    public int pointsNumber; //Número de pontos que esse inimigo concede ao jogador quando é destruído
    [SerializeField]
    private PlayerStats playerStats = null; //Referência para as estatísticas do jogador
    private int spriteNumber = 0; //Indica a sprite inicial do objeto (a sprite é trocada conforme ele se movimenta)
    public float timer; //Variável auxiliar para contar de quanto em quanto tempo deve ser mover
    public int isBackStep = 1; //Indica se deve andar para a esquerda ou para a direita
    private bool isAlive = true; //Está vivo e ativo?
    public InimigoControll enemyController; //Controlador de inimigos
    public GameObject shootPrefab; //Objeto que esse inimigo é capaz de lançar.

    void Start()
    {
        this.GetComponent<SpriteRenderer>().sprite = sprites[0]; 
        playerStats = Camera.main.GetComponent<PlayerStats>(); 
    }

    void FixedUpdate()
    {
        timer += Time.deltaTime; //Guarda a passagem de tempo entre um frame e outro
        if (timer > moveDelay && !enemyController.IsSteppingDown) { //Se já pode realizar uma ação

            if (isAlive) // Está vivo?
            {
                if (spriteNumber == 0)
                {
                    this.GetComponent<SpriteRenderer>().sprite = sprites[1]; //Troque as sprites
                    spriteNumber = 1;
                }
                else
                {
                    this.GetComponent<SpriteRenderer>().sprite = sprites[0]; //Troque as sprites
                    spriteNumber = 0;
                }
            }

            if (enemyController.canMove()) //Pode se mover?
            {
                this.gameObject.transform.position = new Vector2(this.gameObject.transform.position.x + 0.3f * isBackStep, this.gameObject.transform.position.y); //Ande 0.3 unidades para direita ou para a esquerda
                //a depender do atributo isBackStep

                if (this.gameObject.transform.position.x >= 8.1f || this.gameObject.transform.position.x <= -8.1f) //Chegou no limite em que o inimigo pode se mover?
                {
                    Debug.Log("Step all down");
                    enemyController.stepAllDown(); //Avise ao controlador geral, que irá fazer todos os inimigos descerem um nível e caminhar na direção oposta
                }
            }

            timer = 0f; //Reset o timer
        }
    }

    /// <summary>
    /// Move este inimigo para baixo
    /// </summary>
    public void stepDown() {
        this.isBackStep *= -1; //Inverte a direção em que deve andar (-1: esquerda, 1: direita)
        this.gameObject.transform.position = new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y - 0.3f); //Move este inimigo 0.3 unidades para baixo
    }


    /// <summary>
    /// Quando ocorre uma colisão, e foi feita por um projétil do player:
    /// Destrói este inimigo, e adiciona a quantidade de pontos que e o mesmo vale a pontuação do player
    /// se o player ainda estiver vivo, pois o projétil pode ter alcançado este inimigo depois que o player morreu.
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.GetComponent<Projectile>() != null) { //Foi atingido por um player?
            isAlive = false; //Não está mais vivo.
            this.gameObject.GetComponent<Collider2D>().enabled = false; //Desabilite o colisor
            this.GetComponent<SpriteRenderer>().sprite = destroyed[0]; //Mude a sprite para destruído
            enemyController.remove(this.gameObject);
            Destroy(this.gameObject, 1); //Destrua esse objeto depois de 1 segundo para dar tempo de exibir a sprite de 
            if (playerStats.QtVidas > 0) // Se o player está vivo, adicione a potuação que esse inimigo vale à pontuação do player
            {
                enemyController.remove(this.gameObject);
                this.playerStats.Score += this.pointsNumber;
            }
        }
    }

    /// <summary>
    /// Lança o objeto indicado no atributo shootPrefab
    /// </summary>
    public void shoot() {
        GameObject obj = Instantiate(shootPrefab, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 1), Quaternion.identity); //Instancia o projétil
        obj.GetComponent<Rigidbody2D>().velocity = new Vector2(0, obj.GetComponent<EnemyShoot>().velocidade * -1); //Aplica velocidade no projétil recém instanciado, 90 graus para baixo.
    }
}
