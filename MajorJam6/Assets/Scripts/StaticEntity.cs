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
        set { _ghost = value;
               if(!_ghost) MusicStart();}
    }
    [SerializeField] bool _ghost = true;
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
    if(id != ""){
        Debug.Log(FMODUnity.RuntimeManager.StudioSystem.setParameterByName(id, 1));
    }
    
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

            if(id != ""){
                Debug.Log(FMODUnity.RuntimeManager.StudioSystem.setParameterByName(id, 0));
            }
        }
    }
    public string CheckTerrain(Vector3Int positionToCheck){
        if(!ground.groundBlocksProp.ContainsKey(positionToCheck)){
            return "There is nothing there!";
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
        if(ground.plants.ContainsKey(new Vector2Int(positionToCheck.x,positionToCheck.z))){
            return "There is already a plant there!";
        }
        
        if(plantType.shadeRadius != 0){
            bool shaded = false;
            for(int i = positionToCheck.x-plantType.shadeRadius; i <= positionToCheck.x+plantType.shadeRadius; i+=1){
                for(int ii = positionToCheck.z-plantType.shadeRadius; ii <= positionToCheck.z+plantType.shadeRadius; ii+=1){
                    if(!ground.plants.ContainsKey(new Vector2Int(i,ii))){
                        continue;
                    }
                    Debug.Log(ground.plants[new Vector2Int(i,ii)].plantType.plantName);
                    if(ground.plants[new Vector2Int(i,ii)].plantType.plantName.StartsWith("tree_")){
                        shaded = true;
                        break;
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
            if(health < 0){
                ground.plants.Remove(new Vector2Int(mypos.x,mypos.z));
                Destroy(gameObject);
            }
            GroundProperty myblock = ground.groundBlocksProp[mypos+Vector3Int.down];
            cycleAge += Time.deltaTime * world.TimeMultiplier;
            if(plantType.humidityToWater < world.humidity){
                myblock.humidity += (plantType.humidityToWater/100) * Time.deltaTime * world.TimeMultiplier;
                myblock.markedForUpdate = true;
                world.humidity -= (plantType.humidityToWater/100) * Time.deltaTime * world.TimeMultiplier;
            }
            if(plantType.waterToHumidity < myblock.humidity){
                myblock.humidity -= (plantType.waterToHumidity/100) * Time.deltaTime * world.TimeMultiplier;
                myblock.markedForUpdate = true;
                world.humidity += (plantType.waterToHumidity/100) * Time.deltaTime * world.TimeMultiplier;
            }
            if(myblock.humidity < plantType.minHumid){
                health -= Time.deltaTime;
            }
            if(myblock.humidity > plantType.maxHumid){
                health -= Time.deltaTime;
            }
            if(myblock.N > 0){
                myblock.N -= (plantType.N_consumption/((plantType.growthCycle*world.WeekLengthDays*world.DayLengthSeconds))*Time.deltaTime *world.TimeMultiplier);
            } else if(plantType.N_consumption > 0){
                health -= Time.deltaTime;
            }
            if(myblock.K > 0){
                myblock.K -= (plantType.K_consumption/((plantType.growthCycle*world.WeekLengthDays*world.DayLengthSeconds))*Time.deltaTime *world.TimeMultiplier);
            } else if(plantType.K_consumption > 0){
                health -= Time.deltaTime;
            }
            if(myblock.P > 0){
                myblock.P -= (plantType.P_consumption/((plantType.growthCycle*world.WeekLengthDays*world.DayLengthSeconds))*Time.deltaTime *world.TimeMultiplier);
            } else if(plantType.P_consumption > 0){
                health -= Time.deltaTime;
            }
            if(plantType.shadeRadius != 0){
                bool shaded = false;
                for(int i = mypos.x-plantType.shadeRadius; i <= mypos.x+plantType.shadeRadius; i+=1){
                    for(int ii = mypos.z-plantType.shadeRadius; ii <= mypos.z+plantType.shadeRadius; ii+=1){
                        if(ground.plants.ContainsKey(new Vector2Int(i,ii))){
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
                    Debug.Log("REPRODUCE");
                    bool chosen = false;
                    Vector3Int potentialBlock = mypos;
                    for(int i = mypos.x - plantType.reproduction_radius; i < mypos.x + plantType.reproduction_radius;i += 1){
                        for(int ii = mypos.z - plantType.reproduction_radius; ii < mypos.z + plantType.reproduction_radius;ii += 1){
                            potentialBlock = new Vector3Int(i, mypos.y-1,ii);
                            if(ground.plants.ContainsKey(new Vector2Int(i,ii))){
                                continue;
                            }
                            if(ground.groundBlocksProp.ContainsKey(potentialBlock)){
                                if(!ground.groundBlocksProp.ContainsKey(potentialBlock+Vector3Int.up)){
                                    if(ground.groundBlocksProp[potentialBlock].type == 0){
                                        if(Random.Range(0,100)>50){
                                            chosen = true;
                                            break;
                                        }
                                        
                                    }
                                }
                            }
                        }
                        if(chosen){
                            break;
                        }
                    }
                    if(chosen){
                        Debug.Log("bebe");
                        GameObject child = Instantiate(gameObject, potentialBlock + Vector3Int.up, Quaternion.identity);
                        ground.plants.Add(new Vector2Int(potentialBlock.x,potentialBlock.z), child.GetComponent<StaticEntity>());
                        child.GetComponent<StaticEntity>().cycleAge = 0;
                        child.GetComponent<StaticEntity>().mypos = potentialBlock + Vector3Int.up;
                        child.GetComponent<StaticEntity>().health = 10;
                        // unsure if this resets the variables or not
                        //child.GetComponent<StaticEntity>
                    }
                }
            }
        }
    }
}
