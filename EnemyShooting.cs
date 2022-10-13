using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Difference between shooting and enemy shooting is I need to add realistic ai (not just spam clicking)
public class EnemyShooting : MonoBehaviour
{
    private Transform playerInfo;
    public Transform enemyPos;
    public Transform firePoint;
    public GameObject bulletPrefab;
    public GameObject muzzleFlashPrefab;
    private Vector3 correction = new Vector3(1.2f,1.2f,1.2f);

    private bool allowShoot = true;
    public float bulletForce = 20f;
    public float fireRate = 0.5f;
    private bool obstructed = false;
    private float reactionTime = 0.5f;
    private bool waiting = false;

    private EnemyMovement someInfo;

    // Update is called once per frame
    void Awake(){
        //Find the player information through scripts and not pulling a transform in the inspector
        //This means I can have it as a prefab
        someInfo = this.GetComponentInParent<EnemyMovement>();
    }

    void Update(){
        if(playerInfo == null){
            try{
                playerInfo = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
            }catch{}
        }
    }

    void FixedUpdate()
    {
        //check to see if player still exists in the scene
        if(playerInfo != null){
            //only check for LOS and ablilty to shoot if not already waiting to account for reaction time
            if(!waiting){
                LOS();
            }
            if(allowShoot && !obstructed && !waiting && someInfo.agro){
                Shoot();
            }
            obstructed = false;
        }
        
    }

    void LOS(){
        //Fire a ray in shooting direction
        RaycastHit2D hitInfo = Physics2D.Raycast(firePoint.position, playerInfo.position - firePoint.position);
        //say that if the player is not in the LOS then don't bother shooting
        //can add in reaction time here so the player can do good peeks
        if (hitInfo.transform.tag != "Player" && hitInfo.transform.tag != "Bullet"){
            obstructed = true;
            waiting = true;
            StartCoroutine(ReactionTime(reactionTime));
        }
    }

    void Shoot(){
        //can't shoot again until allowShoot is true
        allowShoot = false;

        //Instantiate goes (prefab, position, rotation, parent gameObject,...)
        //Need to rotate the prefabs by 90 for some reason but this is how it's done
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation*Quaternion.Euler(0, 0, -90));
        //Vectort "correction" is used to move the muzzle flash to the end of the barrel by scaling the vector for firePoint and playerPos and adding it on to firePoint
        GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, Vector3.Scale(firePoint.position - enemyPos.position, correction) + firePoint.position, firePoint.rotation*Quaternion.Euler(0, 0, -90), enemyPos);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(firePoint.up*bulletForce, ForceMode2D.Impulse);
        FindObjectOfType<AudioManager>().Play("Shoot");

        //should go into FireRate function to limit shooting speed by waiting before setting allowShoot to true again
        //from this I gather IEnumerators need to be called through StartCoroutines rather than the normal function calling
        StartCoroutine(FireRate(fireRate));
        //had to put allowShoot = true in the coroutine for some reason. It seems like coroutines run along side the program, which I guess makes a lot of sense
    }

    IEnumerator FireRate(float fireRate){
        yield return new WaitForSeconds(fireRate);
        allowShoot = true;
    }
    
    IEnumerator ReactionTime(float reactionTime){
        yield return new WaitForSeconds(reactionTime);
        waiting = false;
    }
}
