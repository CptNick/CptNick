using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int maxHealth = 100;
    public int health;
    public GameObject playerDeath;
    private HealthBar healthBar;

    void Start(){
        healthBar = GameObject.Find("HealthBar").GetComponent<HealthBar>();
        health = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(int damage, GameObject player){
        health -= damage;
        healthBar.SetHealth(health);
        if(health <= 0){
            Die(player);
            FindObjectOfType<GameManager>().EndGame();
        }
    }

    void Die(GameObject player){
        Instantiate(playerDeath, transform.position, transform.rotation);
        FindObjectOfType<AudioManager>().Play("Death");
        FindObjectOfType<AudioManager>().Stop("Walking");
        Destroy(player);
    }
}
