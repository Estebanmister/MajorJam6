using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticEntity : MonoBehaviour
{
    public float health = 10;
    public PlantScriptable plantType;
    public Ground ground;
    public World world;
    public Animator growth;
    public bool ghost = true;
    float cycleAge = 0;
    float updateRate = 10;
    public Vector3Int mypos;
    public string CheckTerrain(Vector3Int positionToCheck){
        if(ground.groundBlocksProp[positionToCheck].N < plantType.N_consumption){
            return "Not enough nitrogen!";
        }
        if(ground.groundBlocksProp[positionToCheck].P < plantType.P_consumption){
            return "Not enough phosphorous!";
        }
        if(ground.groundBlocksProp[positionToCheck].K < plantType.K_consumption){
            return "Not enough potassium!";
        }
        if(ground.groundBlocksProp[positionToCheck].humidity < plantType.minHumid ||
            ground.groundBlocksProp[positionToCheck].humidity > plantType.maxHumid){
            return "Humidity is not right!";
        }
        
        if(plantType.shadeRadius != 0){
            bool shaded = false;
            for(int i = 0; i < plantType.shadeRadius; i+=1){
                for(int ii = 0; ii < plantType.shadeRadius; ii+=1){
                    if(plantType.requirement != ""){
                        if(ground.plants[new Vector2Int(i,ii)].plantType.name.StartsWith(plantType.requirement)){
                            shaded = true;
                            break;
                        }
                    } else{
                        if(ground.plants[new Vector2Int(i,ii)].plantType.name.StartsWith("tree_")){
                            shaded = true;
                            break;
                        }
                    }
                }
                if(shaded){
                    break;
                }
            }
            if(!shaded){
                return "Not enough trees nearby!";
            }
        }

        return "good";
    }
    void Start()
    {
        ground = GameObject.FindGameObjectWithTag("ground").GetComponent<Ground>();
        world = GameObject.FindGameObjectWithTag("world").GetComponent<World>();
    }

    void Update()
    {
        if(growth){
            //animate
        }
        if(!ghost){
            GroundProperty myblock = ground.groundBlocksProp[mypos+Vector3Int.down];
            cycleAge += Time.deltaTime * world.TimeMultiplier;
            if(plantType.humidityToWater < world.humidity){
                myblock.humidity = plantType.humidityToWater;
                myblock.markedForUpdate = true;
                world.humidity -= plantType.humidityToWater;
            }
            if(plantType.waterToHumidity < myblock.humidity){
                myblock.humidity -= plantType.waterToHumidity;
                myblock.markedForUpdate = true;
                world.humidity += plantType.waterToHumidity;
            }
            if(myblock.humidity < plantType.minHumid){
                health -= Time.deltaTime;
            }
            if(myblock.humidity > plantType.maxHumid){
                health -= Time.deltaTime;
            }
            if(myblock.N > 0){
                myblock.N -= plantType.N_consumption/(plantType.growthCycle*world.WeekLengthDays*world.DayLengthSeconds);
            } else if(plantType.N_consumption > 0){
                health -= Time.deltaTime;
            }
            if(myblock.K > 0){
                myblock.K -= plantType.K_consumption/(plantType.growthCycle*world.WeekLengthDays*world.DayLengthSeconds);
            } else if(plantType.K_consumption > 0){
                health -= Time.deltaTime;
            }
            if(myblock.P > 0){
                myblock.P -= plantType.P_consumption/(plantType.growthCycle*world.WeekLengthDays*world.DayLengthSeconds);
            } else if(plantType.P_consumption > 0){
                health -= Time.deltaTime;
            }
            if(plantType.shadeRadius != 0){
                bool shaded = false;
                for(int i = 0; i < plantType.shadeRadius; i+=1){
                    for(int ii = 0; ii < plantType.shadeRadius; ii+=1){
                        if(plantType.requirement != ""){
                            if(ground.plants[new Vector2Int(i,ii)].plantType.name.StartsWith(plantType.requirement)){
                                shaded = true;
                                break;
                            }
                        } else{
                            if(ground.plants[new Vector2Int(i,ii)].plantType.name.StartsWith("tree_")){
                                shaded = true;
                                break;
                            }
                        }
                        
                    }
                    if(shaded){
                        break;
                    }
                }
                if(!shaded){
                    health -= Time.deltaTime;
                }
            }
            if((cycleAge/world.DayLengthSeconds)/world.WeekLengthDays > plantType.growthCycle){
                cycleAge = 0;
                myblock.N += plantType.N_release;
                myblock.P += plantType.P_release;
                myblock.K += plantType.K_release;
                if(plantType.name == "tree_apple"){
                    plantType.edible = true;
                }
                if(plantType.reproduces){
                    bool chosen = false;
                    Vector3Int potentialBlock = mypos;
                    while(!chosen){
                        int x = mypos.x + Random.Range(1,plantType.reproduction_radius+1);
                        int y = mypos.z + Random.Range(1,plantType.reproduction_radius+1);
                        potentialBlock = new Vector3Int(x, mypos.y,y);
                        if(ground.groundBlocksProp.ContainsKey(potentialBlock)){
                            if(!ground.groundBlocksProp.ContainsKey(potentialBlock+Vector3Int.up)){
                                chosen = true;
                                break;
                            }
                        }
                    }
                    if(chosen){
                        GameObject child = Instantiate(gameObject, potentialBlock + Vector3Int.up, Quaternion.identity);
                        ground.plants.Add(new Vector2Int(potentialBlock.x,potentialBlock.z), child.GetComponent<StaticEntity>());
                        // unsure if this resets the variables or not
                        //child.GetComponent<StaticEntity>
                    }
                }
            }
        }
    }
}
