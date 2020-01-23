using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Classe que controla os projéteis lançados pelo Boss.
/// </summary>
public class BossShoot : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(this.gameObject); 
    }
}
