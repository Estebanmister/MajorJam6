using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MissionChecker : MonoBehaviour
{
    Ground ground;
    public TMP_Text desrciption;
    Mission mission;
    public Mission[] tochoose;
    int plantsPresent = 0;
    double timeSince=0;

    void Start()
    {
        ground = GameObject.FindGameObjectWithTag("ground").GetComponent<Ground>();
        mission = tochoose[Random.Range(0,tochoose.Length)];
    }
    void Update(){
        desrciption.text = mission.description;
        if(mission.plantsrequired){
            foreach(StaticEntity staticEntity in ground.plants.Values){
                if(staticEntity.plantType == mission.plantsrequired){
                    plantsPresent += 1;
                }
            }
            if(plantsPresent >= mission.quantityofPlants){
                timeSince += Time.deltaTime;
            }
        }
    }
    
}
