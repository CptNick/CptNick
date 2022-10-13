using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float deathDelay = 2f;
    
    public int numLevel;
    private int numRooms;
    
    private ProceduralGenerator level;
    public GameObject Camera;
    public GameObject playerPrefab;
    public GameObject player;
    public GameObject enemyPrefab;
    private GameObject enemy;
    public List<GameObject> enemies = new List<GameObject>();
    public Animator transition;

    private bool doorOpen = false;
    private bool levelCleared = false;
    [HideInInspector]
    public bool endGame = false;
    private bool generated = false;
    private bool fdown = false;

    private float fadeTime = 1f;

    [HideInInspector]
    public int playerDamage = 40;
    [HideInInspector]
    public int enemyDamage = 5;
    
    void Awake(){
        level = GameObject.Find("Generator").GetComponent<ProceduralGenerator>();
        GenerateLevel();
    }


    void Update(){
        //check how many enemies are left
        //if no enemies left, open door
        //do something like call a method that just says level.openDoor();
        if(player != null){
            if(enemies.Count == 0 && !doorOpen && generated){
                level.OpenDoor();
                FindObjectOfType<AudioManager>().Play("Gate");
                doorOpen = true;
            }
            //fdown is to stop players from spamming the f key which can cause a massive level jump
            if(doorOpen && (player.transform.position - level.doorPos).magnitude < 3 && Input.GetKeyDown("f") && !levelCleared && !fdown){
                levelCleared = true;
                fdown = true;
                StartCoroutine(StopSpam());
            }
            Camera.GetComponent<CameraFollow>().target = player.transform;
        }  
    }
    
    void FixedUpdate(){
        if(levelCleared){
            StartCoroutine(LoadLevel());
            levelCleared = false;
        }
    }

    void GenerateLevel(){
        //number of rooms generated will start off small and slowly increase with levels up to say 20
        //changed this to 10 rooms since it takes a long time to progress through them
        //will add more enemy damage as levels progress instead
        if(numLevel < 10){
            numRooms = 1 + numLevel;
        }
        if(numLevel >= 10){
            numRooms = 10;
        }
        level.Generate(numRooms);
        SpawnEntities();
        generated = true;
    }

    public void EndGame(){
        endGame = true;
        //save level reached here
        Invoke("LoadMainMenu", deathDelay);
    }

    void LoadMainMenu(){
        SceneManager.LoadScene("Menu");
    }

    public void Restart(){
        endGame = false;
        numLevel = 1;
        SceneManager.LoadScene("Generator");
    }

    void SpawnEntities(){
        //player spawn position is in the center of the first room
        Vector3Int playerPos = level.roomInfoArray[0].roomPos;
        playerPos.x += level.roomInfoArray[0].roomWidth/2;
        playerPos.y += level.roomInfoArray[0].roomHeight/2;
        player = Instantiate(playerPrefab, playerPos, Quaternion.Euler(0f,0f,0f));

        //spawn in the enemies
        for(int i = 1; i < level.roomInfoArray.Count; i++){
            //two enemies per room
            Vector3Int enemyPos1 = level.roomInfoArray[i].roomPos + new Vector3Int(level.roomInfoArray[i].roomWidth/2, level.roomInfoArray[i].roomHeight/2, 0);
            Vector3Int enemyPos2 = enemyPos1  + new Vector3Int(2,0,0);

            GameObject enemy1 = Instantiate(enemyPrefab, enemyPos1, Quaternion.Euler(0f,0f,0f));
            GameObject enemy2 = Instantiate(enemyPrefab, enemyPos2, Quaternion.Euler(0f,0f,0f));

            enemies.Add(enemy1);
            enemies.Add(enemy2);
        }
    }

    IEnumerator LoadLevel(){
        //fade to black
        transition.SetTrigger("Start");
        //wait til full black
        yield return new WaitForSeconds(fadeTime);

        //bump up difficluty up to a oneshot
        numLevel += 1;
        if(enemyDamage < 100){
            enemyDamage += 5;
        }
        //reset level 
        level.ClearLevel();
        Destroy(player);
        doorOpen = false;

        //load the next level
        GenerateLevel();
        //fade into game
        transition.SetTrigger("End");
    }

    IEnumerator StopSpam(){
        //to stop players spamming the f key to skip levels
        yield return new WaitForSeconds(10f);
        fdown = false;
    }
}
