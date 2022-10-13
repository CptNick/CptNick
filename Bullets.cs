using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class is attatched to the bullet prefab and should register all hits and handle damage to players and enemies
public class Bullets : MonoBehaviour
{
    //need to find the gameManager to find the damage the player and the enemies do
    private GameManager gameInfo;

    private Player player;
    private Enemy enemy;
    private GameObject Entity;
    //both Player and Enemy use this script

    //This finds if the gameObject (i.e. the prefab that we spawn into the scene) has collided with something
    void OnCollisionEnter2D(Collision2D collision){
        //we can then find the gameObject that we collide with which will come in handy
        Entity = collision.gameObject;
        gameInfo = GameObject.Find("GameManager").GetComponent<GameManager>();

        //these statements find the tag of the gameObject we collide with
        if(Entity.tag == "Enemy"){
            enemy = Entity.GetComponent<Enemy>();
            enemy.TakeDamage(gameInfo.playerDamage, Entity);
            Destroy(gameObject);
        }
        if(Entity.tag == "Player"){
            player = Entity.GetComponent<Player>();
            player.TakeDamage(gameInfo.enemyDamage, Entity);
            Destroy(gameObject);
        }
        if(Entity.tag == "Walls"){
            Destroy(gameObject);
        }
        if(Entity.tag == "Barrels"){
            //for now barrels will act as a wall (might make them breakable in the future)
            Destroy(gameObject);
        }
    }

    //destroy the bullets if they havent been destroyed after 3 seconds
    void OnEnter(){
        StartCoroutine(Despawn());
        if(gameObject != null){
            Destroy(gameObject);
        }
    }

    IEnumerator Despawn(){
        yield return new WaitForSeconds(2f);
    }
}

