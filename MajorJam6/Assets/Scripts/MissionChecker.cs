using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionChecker : MonoBehaviour
{

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        foreach(GameObject manager in GameObject.FindGameObjectsWithTag("mission")){
            if (manager == gameObject){
                continue;
            } else {
                Destroy(gameObject);
            }
        }
    }
}
