using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Plant", order = 1)]
public class PlantScriptable : ScriptableObject
{
    public string plantName;

    public float K_consumption,P_consumption,N_consumption;
    public float K_release,P_release,N_release;
    public float growthCycle;
    public float minTemp;
    public float maxTemp;
    public float minHumid;
    public float maxHumid;
    public int shadeRadius;
    public string requirement;
    public float humidityToWater;
    public float waterToHumidity;
    public bool reproduces;
    public int reproduction_radius;
    public bool edible;
    public float attractivenessMultiplier = 1;
}
