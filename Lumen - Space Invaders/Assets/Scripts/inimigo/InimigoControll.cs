using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe responsável pelo controle da colméia de inimigos comuns e por instanciar o boss quando todos tiverem morrido.
/// </summary>
public class InimigoControll : MonoBehaviour
{
    private LinkedList<GameObject> enemies; //Lista para guardar os inimigos, feita numa lista encadeada pois serão poucos acessos à ela e terá cerca de 50 inimigos apenas,
    //essa estrutura deve ser alterada caso muitos inimigos comecem a ser adicionados ao jogo.
    [SerializeField]
    private GameObject[] enemiesPrefabs = null; //Prefab dos inimigos que será instanciados.
    public Vector2 initialPosition; //Posição inicial em que os inimigos começam a ser instanciados, esse ponto é usado como referência para instanciar os demais inimigos
    public float xDiff; //Distância para o qual o próximo inimigo será instanciado no eixo X
    public float yDiff; //Distância para o qual o próximo inimigo será instanciado no eixo Y
    public int enemiesQt; //Quantidade de inmogs que serão instanciados por linha
    private bool isSteppingDown; //Armazena os estado do delocamento dos inimigos em direção ao player
    private bool canShoot = true; //Define se os inimigos podem atirar, ou seja, se não existe nenhum projétil de inimigos comuns em cena
    private float delayBeforeShooting = 2; // Delay inicial que os inimigos toma para começarem a atirar (Apenas no start)
    private float shootingStartTimer = 0; // Variável contadora auxiliar para o atributo delayBeforeShooting
    public GameObject boss; //Prefab do boss que será instanciado
    public GameObject bossInstance; //Boss instanciado no momento.
    public bool IsSteppingDown { get => isSteppingDown; set => isSteppingDown = value; }
    public bool CanShoot { get => canShoot; set => canShoot = value; }

    void Start()
    {
        enemies = new LinkedList<GameObject>();
        instantiateEnemies(); //Instancia os inimigos em cena
    }

    /// <summary>
    /// Instancia os inimigos em cena.
    /// Para cada linha são instanciados 'enemiesQt' inimigos, ao total são 5 linhas de inimigos.
    /// Cada for-loop é responsável por instanciar uma linha de inimigos e adicioná-los à lista.
    /// </summary>
    void instantiateEnemies()
    {

        int j = 0;

        for (int i = 0; i < enemiesQt; i++)
        {
            enemies.AddLast(Instantiate(enemiesPrefabs[0], new Vector2(initialPosition.x + xDiff * i, initialPosition.y), Quaternion.identity));
            enemies.Last.Value.gameObject.GetComponent<Inimigo>().enemyController = this;
            j++;
        }

        for (int i = 0; i < enemiesQt; i++)
        {
            enemies.AddLast(Instantiate(enemiesPrefabs[0], new Vector2(initialPosition.x + xDiff * i, initialPosition.y + yDiff), Quaternion.identity));
            enemies.Last.Value.gameObject.GetComponent<Inimigo>().enemyController = this;
            j++;
        }

        for (int i = 0; i < enemiesQt; i++)
        {
            enemies.AddLast(Instantiate(enemiesPrefabs[1], new Vector2(initialPosition.x + xDiff * i, initialPosition.y + yDiff * 2), Quaternion.identity));
            enemies.Last.Value.gameObject.GetComponent<Inimigo>().enemyController = this;

            j++;
        }

        for (int i = 0; i < enemiesQt; i++)
        {
            enemies.AddLast(Instantiate(enemiesPrefabs[1], new Vector2(initialPosition.x + xDiff * i, initialPosition.y + yDiff * 3), Quaternion.identity));
            enemies.Last.Value.gameObject.GetComponent<Inimigo>().enemyController = this;
            j++;
        }

        for (int i = 0; i < enemiesQt; i++)
        {
            enemies.AddLast(Instantiate(enemiesPrefabs[2], new Vector2(initialPosition.x + xDiff * i, initialPosition.y + yDiff * 4), Quaternion.identity));
            enemies.Last.Value.gameObject.GetComponent<Inimigo>().enemyController = this;
            j++;
        }
    }

    void Update()
    {
        if (Camera.main.GetComponent<GameState>().CurrentState == GameState.GameStateEnum.STARTED) //Se o jogador já começou o jogo
        {
            if (shootingStartTimer < delayBeforeShooting) // se não passou o delay de start
            {
                shootingStartTimer += Time.deltaTime;
                return;
            }

            if (this.GetComponent<PlayerStats>().QtVidas > 0 && canShoot && enemies.Count > 0) // se não, se o jogador está vivo, não existe nenhum projétil em cena e a quantidade de inimigos é > 0
            {
                int r = Random.Range(0, enemies.Count); //Selecione um inimigo aletaório
                shoot(r); //Faça esse inimigo atirar
            }
        }
    }
    /// <summary>
    /// Faz um inimigo no indice informado atirar.
    /// </summary>
    /// <param name="index"></param>
    void shoot(int index)
    {
        
        canShoot = false;

        LinkedListNode<GameObject> n = enemies.First;

        for (int i = 0; i < index; i++) //Encontre o inimigo
        {
            n = n.Next;
        }

        n.Value.GetComponent<Inimigo>().shoot(); //Atire
    }

    /// <summary>
    /// Faz todos os inimigos comuns descerem um nível na cena, se aproximando do jogador e tornando as coisas mais difíceis conforme o tempo passa.
    /// </summary>
    public void stepAllDown()
    {
        isSteppingDown = true;
        foreach (GameObject o in enemies)
        {
            if (o != null)
            {
                o.gameObject.GetComponent<Inimigo>().stepDown(); //Para cada inimigo, faça-o descer um nível
            }
        }
        isSteppingDown = false;
    }

    ///Remove um Objeto t da lista e verifica se ainda existe algum vivo e ativo, se não existir, instancia o BOSS
    public void remove(GameObject t)
    {
        enemies.Remove(t);

        if (enemies.Count == 0)
            instantiateBoss();
    }

    /// <summary>
    /// Retorna se os inimigos podem se mover ou não.
    /// </summary>
    /// <returns></returns>
    public bool canMove()
    {
        // return (se O jogador está vivo e o jogo começou)
        return GetComponent<PlayerStats>().QtVidas > 0 && Camera.main.GetComponent<GameState>().CurrentState == GameState.GameStateEnum.STARTED; 
    }

    /// <summary>
    /// Retorna quantos inimigos estão vivos
    /// </summary>
    /// <returns></returns>
    public int enemiesAlive()
    {
        return enemies.Count;
    }
    /// <summary>
    /// Instancia o boss na cena
    /// </summary>
    public void instantiateBoss()
    {
        if (this.bossInstance == null) {
            this.bossInstance = Instantiate(boss);
            this.gameObject.GetComponent<CanvasController>().setMessage("Brace Yourselses!"); //Altera a mensagem no rodapé da tela.
        }
    }

    /// <summary>
    /// Elimina todos os inimigos da lista, os destruindo da cena também.
    /// Método usado quando é pressinado f10, com cheat para chegar mais rápido ao chefe.
    /// </summary>
    public void killAllEnemies()
    {
        LinkedList<GameObject> tEnemies = enemies;
        enemies = new LinkedList<GameObject>();
        foreach (GameObject enemy in tEnemies)
        {
            Destroy(enemy);
        }
    }
}
