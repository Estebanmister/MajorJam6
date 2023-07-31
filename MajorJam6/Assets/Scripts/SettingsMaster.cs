using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SettingsMaster : MonoBehaviour
{
    int size = 25;
    public TMP_Text text;
    public bool debug = false;
    bool check = false;
    void Start()
    {
        Application.targetFrameRate = 60;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        if(debug){
            Ground ground = GameObject.FindGameObjectWithTag("ground").GetComponent<Ground>();
            ground.Generate(Random.Range(0,99999));
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
        Destroy(gameObject);
    }
    void Update(){
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        check = true;
        Ground ground = GameObject.FindGameObjectWithTag("ground").GetComponent<Ground>();
        ground.size = size;
        ground.Generate(Random.Range(0,99999));
    }
}
