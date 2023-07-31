using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsGetter : MonoBehaviour
{
    SettingsMaster master;
    void Start(){
        master = GameObject.FindGameObjectWithTag("manager").GetComponent<SettingsMaster>();
    }
    public void LoadMenu(){
        master.LoadMenuScene();
    }
}
