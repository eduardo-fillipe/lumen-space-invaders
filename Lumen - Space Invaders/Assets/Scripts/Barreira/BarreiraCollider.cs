using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarreiraCollider : MonoBehaviour
{

    private int saude = 4; //Quantidades de colisões que a barreira suporta

    [SerializeField]
    private Sprite[] sprites = null; //Array de sprites que são alterados conforme a barreira é danificada
    void Start()
    {
        this.GetComponent<SpriteRenderer>().sprite = sprites[3]; 
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        saude--;
        if (saude > 0)
        {
            this.GetComponent<SpriteRenderer>().sprite = sprites[saude - 1]; //Para cada colisão a sprite da barreira é alterada, e a vida diminuída.
        } else
        {
            Destroy(this.gameObject);
        }
    }
}
