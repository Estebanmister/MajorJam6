using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SettingsMaster : MonoBehaviour
{
    int size = 10;
    public TMP_Text text;
    public bool debug = false;
    void Start()
    {
        Application.targetFrameRate = 60;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        if(debug){
            Ground ground = GameObject.FindGameObjectWithTag("ground").GetComponent<Ground>();
            ground.Generate();
        }
    }

    public void ChangeSize(float newsize){
        size = (int)newsize;
        if(text){
            text.text = size.ToString()+"x"+size.ToString()+"x6";
        }
    }
    public void LoadMainScene(){
        SceneManager.LoadScene("main");
    }
    public void LoadMenuScene(){
        SceneManager.LoadScene("menu");
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        foreach(GameObject manager in GameObject.FindGameObjectsWithTag("manager")){
            if (manager == gameObject){
                continue;
            } else {
                Destroy(gameObject);
            }
        }
        Ground ground = GameObject.FindGameObjectWithTag("ground").GetComponent<Ground>();
        ground.size = size;
        ground.Generate();
    }
}
