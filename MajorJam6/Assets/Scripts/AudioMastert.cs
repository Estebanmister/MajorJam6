using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioMastert : MonoBehaviour
{
    // Start is called before the first frame update
    public bool loadedonce = false;
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        transform.localPosition = Camera.main.transform.position;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        if(!this){
            return;
        }
        Debug.Log("load");
        if(loadedonce){
            Destroy(gameObject);
        }
        foreach(GameObject manager in GameObject.FindGameObjectsWithTag("audio")){
            if (manager == gameObject){
                continue;
            } else {
                Destroy(gameObject);
            }
        }
        loadedonce = true;
        transform.parent = Camera.main.transform;
        transform.localPosition = Vector3.zero;
    }
}
