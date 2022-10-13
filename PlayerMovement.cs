using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public Rigidbody2D rb;
    private Vector2 moveDirection;
    public Animator animator;
    private bool startedWalking = false;

    // Update is called once per frame
    void Update(){
        ProcessInputs();
        //In the futer I could set a variable called "motion" to "walking" instead of trying to find the number
        animator.SetFloat("Speed", rb.velocity.magnitude);
        //Debug.Log(rb.velocity.magnitude);
        if(rb.velocity.magnitude != 0 && !startedWalking){
            FindObjectOfType<AudioManager>().Play("Walking");
            startedWalking = true;
        }
        if(rb.velocity.magnitude == 0 && startedWalking){
            FindObjectOfType<AudioManager>().Stop("Walking");
            startedWalking = false;
        }
    }

    void FixedUpdate(){
        faceMouse();
        Move();

    }
    //Finding the move direction and handling player inputs
    void ProcessInputs(){
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(moveX, moveY).normalized;
    }

    //Give the Player (inputted rigid body) velocity
    void Move(){
        rb.velocity = new Vector2(moveDirection.x*moveSpeed*Time.fixedDeltaTime, moveDirection.y*moveSpeed*Time.fixedDeltaTime);
    }
    
    //Get Player to face the correct direction
    void faceMouse(){
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector2 direction = new Vector2(
            mousePosition.x-transform.position.x, mousePosition.y - transform.position.y);
        transform.up = direction;
    }
}
