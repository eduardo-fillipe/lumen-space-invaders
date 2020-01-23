using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe responsável por animar um determinado objeto, invertendo a orientação do sprite do mesmo.
/// </summary>
public class ProjectFlipper : MonoBehaviour
{
    //Variáveis auxiliares para regular o tempo
    private float timer = 0;
    public float delay;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer > delay)
        {
            this.gameObject.GetComponent<SpriteRenderer>().flipY = !this.gameObject.GetComponent<SpriteRenderer>().flipY; // Inverte a orientação Y do sprite.
            timer = 0;
        }
    }
}
