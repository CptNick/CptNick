using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public Transform playerPos;
    //finds end of gun to shoot from
    public Transform firePoint;
    public GameObject bulletPrefab;
    public GameObject muzzleFlashPrefab;
    private Vector3 correction = new Vector3(1.2f,1.2f,1.2f);

    private bool allowShoot = true;
    public float bulletForce = 20f;
    public float fireRate = 0.2f;
    // Update is called once per frame

    void Update()
    {   
        if(playerPos != null){
            if((Input.GetMouseButton(0)) && (allowShoot)){
                Shoot();
            }
        }
    }

    void Shoot(){
        //can't shoot again until allowShoot is true
        allowShoot = false;

        //Instantiate goes (prefab, position, rotation, parent gameObject,...)
        //Need to rotate the prefabs by 90 for some reason but this is how it's done
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation*Quaternion.Euler(0, 0, -90));
        //Vectort "correction" is used to move the muzzle flash to the end of the barrel by scaling the vector for firePoint and playerPos and adding it on to firePoint
        GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, Vector3.Scale(firePoint.position - playerPos.position, correction) + firePoint.position, firePoint.rotation*Quaternion.Euler(0, 0, -90), playerPos);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(firePoint.up*bulletForce, ForceMode2D.Impulse);
        FindObjectOfType<AudioManager>().Play("Shoot");

        //should go into FireRate function to limit shooting speed by waiting before setting allowShoot to true again
        //from this I gather IEnumerators need to be called through StartCoroutines rather than the normal function calling
        StartCoroutine(FireRate(fireRate));
        //had to put allowShoot = true in the coroutine for some reason. It seems like coroutines run along side the program, which I guess makes a lot of sense
    }

    IEnumerator FireRate(float fireRate){
        //Debug.Log("Started Coroutine at timestamp : " + Time.time);
        yield return new WaitForSeconds(fireRate);
        //Debug.Log("Finished Coroutine at timestamp : " + Time.time);
        allowShoot = true;
    }
}
