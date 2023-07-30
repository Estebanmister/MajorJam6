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
    public float TimeMultiplier = 1;
    void Start(){
        // for editor purposes
        Application.targetFrameRate = 60;
    }
}
