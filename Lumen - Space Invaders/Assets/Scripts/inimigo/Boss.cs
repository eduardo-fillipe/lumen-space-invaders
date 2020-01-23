using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public float velocidade; //Velocidade de movimento do boss
    public int vida; // Quantidade de vezes que pode ser atingido
    public GameObject[] weapons; // A rotação desses objetos indicam a direção para onde os tiros vão
    public GameObject projectillePrefab; //Qual projétil será atirado

    //Limites até onde o boss pode se mover
    public Vector2 rightLimit = new Vector2(8f, 4.4f); 
    public Vector2 leftLimit = new Vector2(-8.1f, -1);

    //Posição inicial do boss
    public Vector2 initialPosition = new Vector2(0, 3.5f);

    //Indica o estado atual do boss
    private State state = State.NONE;
    
    //Indica se o boss está vivo
    private bool isAlive = true;

    //Nerf no boss, adiciona um delay entre as jogadas dele.
    public float playTimer = 0.3f;

    //contador auxiliar para o playTimer acima.
    private float timer;

    //Referência para o status do player.
    private PlayerStats playerStats;

    //Jogada atual sendo realizada pelo boss.
    private Coroutine currentPlayCoroutine;

    enum State
    {
        NONE, SHOOTING, TELEPORTING, MOVING, DYING, ENTERING
    }

    //Tipo de tipo que o boss pode executar
    enum ShootType {
        ALL, ONE, HALF
    }

    //Tipo de jogada que o boss pode fazer.
    enum PlayType
    {
        SHOOT, MOVE, TELEPORT, ENTER
    }

    void Start()
    {
        state = State.ENTERING;
        playerStats = Camera.main.GetComponent<PlayerStats>(); 
        currentPlayCoroutine = StartCoroutine(moveTo(initialPosition, 1)); //Move o boss para a posição inicial
    }

    void Update()
    {
        if (Camera.main.GetComponent<GameState>().CurrentState == GameState.GameStateEnum.STARTED) //Se o jogo começou
        {
            timer += Time.deltaTime; 

            if (canPlay()) //Se o boss pode fazer a jogada
            {
                PlayType playType = getRandomPlay(); //Todas as jogadas do boss são aleatórias.
                doPlay(playType); //Executa a jogada.
            }
        }
    }

    //Retorna se o boss pode realizar uma jogada no momento atual
    bool canPlay()
    {
        return this.state == State.NONE && timer > playTimer && isAlive;
    }

    /// <summary>
    /// Executa uma jogada a depender do tipo de jogada.
    /// </summary>
    /// <param name="play"> O tipo de jogada</param>
    void doPlay(PlayType play)
    {
        switch(play)
        {
            case PlayType.MOVE:
                moveTo(getRandomPosition());
                break;
            case PlayType.SHOOT:
                System.Array values = System.Enum.GetValues(typeof(ShootType));
                ShootType shootType = (ShootType) values.GetValue(Random.Range(0, values.Length));
                shoot(shootType, 5, Random.Range(2, 5));
                break;
            case PlayType.TELEPORT:
                teleportTo(getRandomPosition());
                break;
        }
    }

    void shoot(ShootType s, float velocity, int quantity)
    {
        switch (s)
        {
            case ShootType.ALL:
                foreach (GameObject weapon in weapons)
                {
                    currentPlayCoroutine = StartCoroutine(shootXTimes(quantity, velocity, 0.5f, weapon.transform.localRotation)); //Atira com todas as armas do atributo weapons dessa classe
                }
                break;
            case ShootType.HALF: //Tipo de tipo removido para nerfar o boss.
                break;
            case ShootType.ONE:
                currentPlayCoroutine = StartCoroutine(shootXTimes(quantity, velocity, 0.5f, Quaternion.Euler(0,0,-90))); //Atira para baixo n vezes
                break;
        }
    }

    //Método que efetivamente realiza o tiro
    IEnumerator shootXTimes(int t, float velocity, float interval, Quaternion direction)
    {
        this.state = State.SHOOTING;

        for (int i = 0; i < t; i++)
        {
            GameObject obj = Instantiate(projectillePrefab, this.gameObject.transform.position, direction);
            obj.gameObject.GetComponent<Rigidbody2D>().velocity = obj.gameObject.transform.right * velocity;
            if (t == 1) {
                yield return new WaitForSeconds(0);
            } else {
                yield return new WaitForSeconds(interval);
            }
        }

        this.state = State.NONE;
        timer = 0;
    }

    //Teleporta o boss para uma posição
    void teleportTo(Vector2 pos)
    {
        currentPlayCoroutine = StartCoroutine(blinkAndTP(pos));
    }
    /// <summary>
    /// Teleporta o boss após fazer o mesmo piscar 10 vezes.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    IEnumerator blinkAndTP(Vector2 pos) {
        this.state = State.TELEPORTING;
        this.gameObject.GetComponent<Collider2D>().enabled = false;

        for (int i = 0; i < 10; i++) {
            this.gameObject.GetComponent<SpriteRenderer>().enabled = !this.gameObject.GetComponent<SpriteRenderer>().enabled;
            yield return new WaitForSeconds(0.1f);
        }

        this.transform.position = pos;

        this.gameObject.GetComponent<Collider2D>().enabled = true;
        this.state = State.NONE;
        timer = 0;
    }

    void moveTo(Vector2 pos)
    {
        currentPlayCoroutine = StartCoroutine(moveTo(pos, this.velocidade));
    }
    /// <summary>
    /// Move o boss para uma determina posição com determinada velocidade.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="vel"></param>
    /// <returns></returns>

    IEnumerator moveTo(Vector2 pos, float vel)
    {
        this.state = State.MOVING;
        float w = 0.01f;
        float step = vel * w;

        while (this.gameObject.transform.position.x != pos.x || this.gameObject.transform.position.y != pos.y)
        {
            yield return new WaitForSeconds(w);
            transform.position = Vector2.MoveTowards(transform.position, pos, step);
        }
        this.state = State.NONE;
        timer = 0;
    }

    /// <summary>
    /// Retorna uma posição aleatória dentro da área em que o boss pode ser movimentar
    /// </summary>
    /// <returns></returns>
    Vector2 getRandomPosition()
    {
        float x = Random.Range(leftLimit.x, rightLimit.x);
        float y = Random.Range(leftLimit.y, rightLimit.y);
        return new Vector2(x, y);
    }

    /// <summary>
    /// Retorna uma jogada aleatória
    /// </summary>
    /// <returns></returns>
    PlayType getRandomPlay()
    {        
        int shootChance = 4;
        int tpChance = 1;
        int moveChance = 1;
        PlayType playType;

        int i = Random.Range(0, shootChance + tpChance + moveChance);

        if ((i -= shootChance) < 0)
        {
            playType = PlayType.SHOOT;
        } else if ((i -= tpChance) < 0) {
            playType = PlayType.TELEPORT;
        }
        else {
            playType = PlayType.MOVE;
        }
        return playType;
    }

    /// <summary>
    /// Faz o boss piscar após receber danos.
    /// </summary>
    /// <param name="q"></param>
    /// <returns></returns>
    IEnumerator blink(int q)
    {
        for (int i = 0; i < q; i++)
        {
            this.gameObject.GetComponent<SpriteRenderer>().enabled = !this.gameObject.GetComponent<SpriteRenderer>().enabled;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.GetComponent<Projectile>() != null)
        {
            this.vida--;
            playerStats.Score += 10;
            StartCoroutine(blink(2));
            if (this.vida == 0)
            { 
                StartCoroutine(die());
                Camera.main.gameObject.GetComponent<CanvasController>().messageText.text = "CONGRATULATIONS! YOU WON!";
            }
        }
    }

    void stopCurrentPlay() {
        if (state != State.NONE && currentPlayCoroutine != null)
            StopCoroutine(currentPlayCoroutine);
    }

    /// <summary>
    /// Método que mata o boss, reduzindo o tamanho dele em tela.
    /// </summary>
    /// <returns></returns>
    IEnumerator die()
    {
        this.state = State.DYING;
        stopCurrentPlay();
        this.gameObject.GetComponent<Collider2D>().enabled = false;

        float w = 0.01f;
        float vel = 2;
        float step = vel * w;

        while (this.gameObject.transform.localScale.x > 0)
        {
            yield return new WaitForSeconds(w);
            this.gameObject.transform.localScale = new Vector2(this.gameObject.transform.localScale.x - step, this.gameObject.transform.localScale.y - step);
            this.gameObject.GetComponent<SpriteRenderer>().enabled = !this.gameObject.GetComponent<SpriteRenderer>().enabled;
        }

        Camera.main.gameObject.GetComponent<GameState>().CurrentState = GameState.GameStateEnum.OVER;
        Camera.main.gameObject.GetComponent<CanvasController>().showGameOver(true);

        timer = 0;
        isAlive = false;
    }
}
