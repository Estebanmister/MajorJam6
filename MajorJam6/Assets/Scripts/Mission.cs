using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Mission", order = 1)]
public class Mission : ScriptableObject
{
    public string description;
    public string[] plantsrequired;
    public int quantityofPlants;
    public string[] animalsrequired;
    public int quantityofAnimals;
    public float daysRequired;
}
