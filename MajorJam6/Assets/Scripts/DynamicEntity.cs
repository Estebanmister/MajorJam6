using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicEntity : MonoBehaviour
{
    public bool ghost = true;
    public float viewRadius = 10;
    public float hungerRate = 0.2f;
    public float health = 10;
    public float hunger = 0;
    public float cycleAge = 0;
    Ground ground;
    World world;
    List<StaticEntity> staticEntities = new List<StaticEntity>();
    StaticEntity targeting;
    Vector3 goingTo;
    bool pursuing = false;
    void Start()
    {
        ground = GameObject.FindGameObjectWithTag("ground").GetComponent<Ground>();
        world = GameObject.FindGameObjectWithTag("world").GetComponent<World>();
    }
    void OnTriggerEnter(Collider other){
        if(other.tag == "plant"){
            StaticEntity plant = other.GetComponent<StaticEntity>();
            if(plant.plantType.edible){
                staticEntities.Add(plant);
            }
        }
    }
    void OnTriggerExit(Collider other){
        if(other.tag == "plant"){
            StaticEntity plant = other.GetComponent<StaticEntity>();
            if(staticEntities.Contains(plant)){
                staticEntities.Remove(plant);
            }
        }
    }
    void Update()
    {
        if(!ghost){
            cycleAge += Time.deltaTime * world.TimeMultiplier;
            hunger += Time.deltaTime * hungerRate;

            if(hunger > 5){
                foreach(StaticEntity plant in staticEntities){
                    if(!targeting){
                        targeting = plant;
                    }
                    if(targeting.plantType.attractivenessMultiplier < plant.plantType.attractivenessMultiplier){
                        targeting = plant;
                    }
                }
                pursuing = true;
            }
            if(pursuing && hunger > 5){
                goingTo = targeting.transform.position;
                transform.position = Vector3.Lerp(transform.position, goingTo, 0.1f);
                if(Vector3.Distance(goingTo, transform.position) <= 1){
                    targeting.health -= Time.deltaTime;
                    hunger -= Time.deltaTime;
                }
            }
            if(hunger > 10){
                health -= Time.deltaTime;
            }
            // animals reproduce after 9 weeks
            if((cycleAge/world.DayLengthSeconds)/world.WeekLengthDays > 9){
                // multiply!!!!!
            }
        }
    }
}
