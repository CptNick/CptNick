using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    //Most of these variables should be able to be changed as the levels progress
    private float moveSpeed = 100f;
    private Vector2 moveDirection;
    private float minSeparation = 5f;
    private float separation;

    private Transform playerInfo;
    public Rigidbody2D rb;
    public Animator animator;
    public bool agro = false;
    private ProceduralGenerator level;

    //will eventually store all level info in a GameManager class
    void Awake(){
        level = GameObject.Find("Generator").GetComponent<ProceduralGenerator>();
    }

    // Update is called once per frame
    void Update(){
        if(playerInfo == null){
            try{
                playerInfo = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
            }catch{}
        }
        animator.SetFloat("Speed", rb.velocity.magnitude);
    }

    void FixedUpdate(){
        if(playerInfo != null){
            facePlayer();
            //only need to check the rooms if the enemies aren't already agro
            if(!agro){
                CheckRoom();
                CheckCam();
            }
            //don't need to run the movement method if the enemies shouldn't be moving
            if(agro){
                BasicMovement();
            }
            
        }
    }
    
    //Give the Enemy (inputted rigid body) velocity
    void BasicMovement(){
        separation = (playerInfo.position - transform.position).magnitude;
        if(separation < minSeparation){
            rb.velocity = new Vector2(0f,0f);
            return;
        }
        rb.velocity = new Vector2(moveDirection.x*moveSpeed*Time.fixedDeltaTime, moveDirection.y*moveSpeed*Time.fixedDeltaTime);
    }

    //check to see if player has entered the room in which an enemy has spawned
    void CheckRoom(){
        int index = 0;
        foreach(RoomInfo room in level.roomInfoArray){
            if(playerInfo.position.x > room.roomPos.x && playerInfo.position.x < room.roomPos.x + room.roomWidth && playerInfo.position.y > room.roomPos.y && playerInfo.position.y < room.roomPos.y + room.roomHeight){
                if(transform.position.x > room.roomPos.x && transform.position.x < room.roomPos.x + room.roomWidth && transform.position.y > room.roomPos.y && transform.position.y < room.roomPos.y + room.roomHeight){
                    agro = true;
                    break;
                }
            }
            index += 1;
        }
    }

    //sometime we can see the enemy before entering a room and they still dont react
    //so we make this method to agro the enemies on sight too
    void CheckCam(){

        //enemy x and y
        float x = transform.position.x;
        float y = transform.position.y;
        //camera x and y
        float camX = Camera.main.transform.position.x;
        float camY = Camera.main.transform.position.y;
        //camera width and height (orthographic size gives units from centre to top of screen)
        float height = Camera.main.orthographicSize*2;
        float width = height * Screen.width/Screen.height;

        //finding the screen bounds and adding a small buffer to allow the player to react
        float buffer = 2;
        float rightBound = camX + width/2 - buffer;
        float leftBound = camX - width/2 + buffer;
        float topBound = camY + height/2 - buffer;
        float botBound = camY - height/2 + buffer;

        if(x > leftBound && x < rightBound && y > botBound && y < topBound){
            agro = true;
        }
    }
    
    //finding the move direction and handling reactions to the player (this is very basic)
    //will try to include taking cover at higher levels
    void facePlayer(){
        moveDirection = (playerInfo.position - transform.position).normalized;
        transform.up = moveDirection;
    }
}
