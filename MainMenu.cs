using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private float volumeScale;
    private AudioManager sound;
    private float fadeTime = 1f;
    public Animator transition;
    public bool inOptions = false;
    public GameObject optionsMenu;
    public GameObject mainMenu;

    void Awake(){
        Cursor.visible = true;
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            if(inOptions){
                LeaveOptions();;
            } 
        }
    }

    public void PlayGame(){
        DontDestroyOnLoad(GameObject.Find("AudioManager"));
        FindObjectOfType<AudioManager>().Stop("Theme2");
        FindObjectOfType<AudioManager>().Play("Theme1");
        StartCoroutine(LoadLevel());
    }
    
    public void QuitGame(){
        Application.Quit();
    }

    //makes sure only theme2 is playing in the menu
    void Start(){
        FindObjectOfType<AudioManager>().Stop("Theme1");
        sound = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        FindObjectOfType<AudioManager>().Play("Theme2");
    }

    public void LeaveOptions(){
        optionsMenu.SetActive(false);
        mainMenu.SetActive(true);
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

    IEnumerator LoadLevel(){
        //fade to black
        transition.SetTrigger("Start");
        //wait til full black
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene("Generation");
        //fade into game
        transition.SetTrigger("End");
    }

}
