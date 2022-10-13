using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 100;
    private int health;

    public GameObject EnemyDeath;
    private GameManager gameManager;

    public Canvas canvasPrefab;
    private Canvas enemyUI;
    private HealthBar healthBar;

    private Vector3 offset = new Vector3(0f,-0.5f,0f);
    
    void Start(){
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        enemyUI = Instantiate(canvasPrefab, transform.position + offset, Quaternion.Euler(0,0,0));
        healthBar = enemyUI.GetComponentInChildren<HealthBar>();
        health = maxHealth;
        healthBar.SetMaxHealth(health);
    }
    void Update(){
        if(enemyUI != null){
            enemyUI.transform.position = transform.position + offset;
        }
    }

    public void TakeDamage(int damage, GameObject enemy){
        health -= damage;
        healthBar.SetHealth(health);
        gameObject.GetComponent<EnemyMovement>().agro = true;
        if(health <= 0){
            Die(enemy);
        }
    }

    void Die(GameObject enemy){
        gameManager.enemies.Remove(enemy);
        Instantiate(EnemyDeath, transform.position, transform.rotation);
        FindObjectOfType<AudioManager>().Play("Death");
        Destroy(enemy);
        Destroy(enemyUI);
    }
}
