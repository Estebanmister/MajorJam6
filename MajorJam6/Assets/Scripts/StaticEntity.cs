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
    public bool ghost{
        get { return _ghost;}
        set { _ghost = ghost;
               if(_ghost) MusicStart();}
    }
    bool _ghost = true;
    float cycleAge = 0;
    float updateRate = 10;
    public Vector3Int mypos;
    void MusicStart(){
        string id = "";
        print(plantType.plantName);
        switch (plantType.plantName){
        case "tree_apple":
        id = "ifApple";
        break;
        case "Bluebonnet":
        id = "ifBluebonnet";
        break;
        case "Cabbage":
        id = "ifCabbage";
        break;
        case "Carrot":
        id = "ifCarrot";
        break;
        case "tree_cherry":
        id = "ifCherry";
        break;
        case "Dahlia":
        id = "ifDahlia";
        break;
        case "Heather":
        id = "ifHeather";
        break;
        case "tree_maple":
        id = "ifMaple";
        break;
        case "tree_oak":
        id = "ifOak";
        break;
        case "tree_pine":
        id = "ifPine";
        break;
        case "Poppy":
        id = "ifPoppy";
        break;
        case "Strawberry":
        id = "ifLegume";
        break;
        case "Yam":
        id = "ifYam";
        break;
    }
    print(id);
    Debug.Log(FMODUnity.RuntimeManager.StudioSystem.setParameterByName(id, 1));
    }
    void OnDestroy(){
        if(ghost){
            return;
        }
        Debug.Log("disable");
        // check if any plants of the same type are left
        bool left = false;
        foreach(StaticEntity staticEntity in ground.plants.Values){
            if(staticEntity == this){
                continue;
            }
            if(staticEntity.plantType == plantType){
                left = true;
            }
        }
        if(!left){
            string id = "";
            switch (plantType.plantName){
                case "tree_apple":
                id = "ifApple";
                break;
                case "Bluebonnet":
                id = "ifBluebonnet";
                break;
                case "Cabbage":
                id = "ifCabbage";
                break;
                case "Carrot":
                id = "ifCarrot";
                break;
                case "tree_cherry":
                id = "ifCherry";
                break;
                case "Dahlia":
                id = "ifDahlia";
                break;
                case "Heather":
                id = "ifHeather";
                break;
                case "tree_maple":
                id = "ifMaple";
                break;
                case "tree_oak":
                id = "ifOak";
                break;
                case "tree_pine":
                id = "ifPine";
                break;
                case "Poppy":
                id = "ifPoppy";
                break;
                case "Strawberry":
                id = "ifLegume";
                break;
                case "Yam":
                id = "ifYam";
                break;
            }

            Debug.Log(FMODUnity.RuntimeManager.StudioSystem.setParameterByName(id, 0));
        }
    }
    public string CheckTerrain(Vector3Int positionToCheck){
        if(!ground.groundBlocksProp.ContainsKey(positionToCheck)){
            return "not there";
        }
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
            for(int i = positionToCheck.x-plantType.shadeRadius; i <= positionToCheck.x+plantType.shadeRadius; i+=1){
                for(int ii = positionToCheck.y-plantType.shadeRadius; ii <= positionToCheck.y+plantType.shadeRadius; ii+=1){
                    if(!ground.plants.ContainsKey(new Vector2Int(i,ii))){
                        continue;
                    }
                    if(plantType.requirement != ""){
                        if(ground.plants[new Vector2Int(i,ii)].plantType.plantName.StartsWith(plantType.requirement)){
                            shaded = true;
                            break;
                        }
                    } else{
                        if(ground.plants[new Vector2Int(i,ii)].plantType.plantName.StartsWith("tree_")){
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
                            if(ground.plants[new Vector2Int(i,ii)].plantType.plantName.StartsWith(plantType.requirement)){
                                shaded = true;
                                break;
                            }
                        } else{
                            if(ground.plants[new Vector2Int(i,ii)].plantType.plantName.StartsWith("tree_")){
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
                if(plantType.plantName == "tree_apple"){
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
