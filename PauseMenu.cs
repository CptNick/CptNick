using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{

    public static bool paused = false;
    public GameObject pauseMenuUI;
    //want to hide the player UI on pause
    public GameObject playerUI;
    public GameObject optionsMenu;
    public GameObject pauseMenu;

    private float fadeTime = 1f;
    public Animator transition;
    private AudioManager sound;
    private float volumeScale;
    public bool inOptions = false;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
            if(!paused){
                Pause();
            }else{
                if(!inOptions){
                    Resume();
                }else{
                    LeaveOptions();
                }
            } 
        }
    }

    void Awake(){
        sound = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    public void Resume(){
        pauseMenuUI.SetActive(false);
        playerUI.GetComponent<Canvas>().enabled = true;
        Cursor.visible = false;
        Time.timeScale = 1f;
        paused = false;
    }

    public void Pause(){
        pauseMenuUI.SetActive(true);
        playerUI.GetComponent<Canvas>().enabled = false;
        Cursor.visible = true;
        Time.timeScale = 0f;
        paused = true;
    }

    public void Quit(){
        //DontDestroyOnLoad(GameObject.Find("AudioManager"));
        StartCoroutine(LoadMenu());
    }

    public void LeaveOptions(){
        optionsMenu.SetActive(false);
        pauseMenu.SetActive(true);
        inOptions = false;
    }

    public void EnterOptions(){
        inOptions = true;
    }


    //want to scale all individual sounds by the master volume slider
    public void MasterVolume(){
        volumeScale = GameObject.Find("MasterVolume").GetComponentInChildren<Slider>().value;
        foreach(Sound s in sound.sounds){
            //need to scale this so we dont mess with the equalisation
            //volumeScale is only on a 0-1 scale
            s.source.volume = s.volume*volumeScale;
        }
    }

    //want to scale all individual sounds by the master volume slider
    public void MusicVolume(){
        volumeScale = GameObject.Find("MusicVolume").GetComponentInChildren<Slider>().value;
        foreach(Sound s in sound.sounds){
            if(s.name == "Theme1" || s.name == "Theme2"){
                s.source.volume = s.volume*volumeScale;
            }
        }
    }

    //want to scale all individual sounds by the master volume slider
    public void SFXVolume(){
        volumeScale = GameObject.Find("SFXVolume").GetComponentInChildren<Slider>().value;
        foreach(Sound s in sound.sounds){
            if(s.name != "Theme1" && s.name != "Theme2"){
                s.source.volume = s.volume*volumeScale;
            }
        }
    }

    IEnumerator LoadMenu(){
        //fade to black
        Time.timeScale = 1f;
        transition.SetTrigger("Start");
        //wait til full black
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene("Menu");
        
        //fade into game
        transition.SetTrigger("End");
    }
}
