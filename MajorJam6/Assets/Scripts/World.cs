using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public string gameState = "editingTerrain";
    public float humidity = 50;
    public float temperature = 21;
    public float DayLengthSeconds = 5*60;
    public float WeekLengthDays = 7;
    public float TimeMultiplier = 0.1f;
    public double TotalTime = 0;
    void Start(){
        // for editor purposes
        Application.targetFrameRate = 60;
    }
    void Update(){
        if(humidity > 100){
            humidity = 0;
        }
        TotalTime += Time.deltaTime * TimeMultiplier;
    }
    public void TimeAccelerate(bool accelerate){
        if(accelerate){
            TimeMultiplier = 60;
        } else {
            TimeMultiplier = 0.1f;
        }
    }
}
