using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rigidBody = null;
    [SerializeField]
    private float velocidade = 0;

    private PlayerStats playerStats;

    void Awake()
    {
        this.playerStats = Camera.main.gameObject.GetComponent<PlayerStats>();
        this.playerStats.canShoot = false;
        this.rigidBody = this.GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        this.rigidBody.velocity = new Vector2(0, velocidade);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        playerStats.canShoot = true;
    }
}
