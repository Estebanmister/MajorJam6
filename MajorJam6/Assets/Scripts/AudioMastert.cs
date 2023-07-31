using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioMastert : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        transform.localPosition = Camera.main.transform.position;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        foreach(GameObject manager in GameObject.FindGameObjectsWithTag("audio")){
            if (manager == gameObject){
                continue;
            } else {
                DestroyImmediate(gameObject);
            }
        }
        transform.parent = Camera.main.transform;
        transform.localPosition = Vector3.zero;
    }
}
