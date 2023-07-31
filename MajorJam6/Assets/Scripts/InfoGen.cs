using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
public class InfoGen : MonoBehaviour
{
    public TMP_Text edible;
    public TMP_Text minerals;
    public TMP_Text mineralsRelease;
    public TMP_Text humidity;
    public TMP_Text cycle;
    public void Hovered(PlantScriptable plant){
        if(plant.edible){
            edible.text = "Edible plant";
        } else {
            edible.text = "Non edible plant";
        }
        cycle.text = plant.growthCycle.ToString() + " Week growth cycle";
        minerals.text = "Consumes ";
        if(plant.N_consumption > 0){
            minerals.text += "N " + plant.N_consumption.ToString()+" ";
        }
        if(plant.P_consumption > 0){
            minerals.text += "P " + plant.P_consumption.ToString()+" ";
        }
        if(plant.K_consumption > 0){
            minerals.text += "K " + plant.K_consumption.ToString()+" ";
        }
        mineralsRelease.text = "Produces ";
        if(plant.N_release > 0){
            mineralsRelease.text += "N " + plant.N_release.ToString()+" ";
        }
        if(plant.P_release > 0){
            mineralsRelease.text += "P " + plant.P_release.ToString()+" ";
        }
        if(plant.K_release > 0){
            mineralsRelease.text += "K " + plant.K_release.ToString()+" ";
        }
        humidity.text = "Min: " + plant.minHumid +" % Max: " + plant.maxHumid + " %";
    }
}
