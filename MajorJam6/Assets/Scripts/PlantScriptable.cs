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
    public float shadeRadius;
}
