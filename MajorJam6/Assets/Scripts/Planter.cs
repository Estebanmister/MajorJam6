using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planter : MonoBehaviour
{
    public bool placing = false;
    public bool animal = false;
    public bool removing = false;
    public GameObject warning;
    public GameObject prefabToPlace;
    public GameObject ghostPlace;
    public Ground ground;

    void Start()
    {
        ground = GameObject.FindGameObjectWithTag("ground").GetComponent<Ground>();
    }
    public void BeginPlacement(GameObject prefab){
        prefabToPlace = prefab;
        DestroyImmediate(ghostPlace);
        ghostPlace = Instantiate(prefab);
        placing = true;
        animal = false;
        removing = false;
    }
    public void BeginRemoval(){
        placing = false;
        animal = false;
        removing = !removing;
    }
    public void PlaceAnimal(GameObject prefab){
        prefabToPlace = prefab;
        DestroyImmediate(ghostPlace);
        ghostPlace = Instantiate(prefab);
        placing = true;
        animal = true;
        removing = false;
    }

    void Update()
    {
        RaycastHit hit;
        if(removing){
            warning.SetActive(true);
            if (Input.GetMouseButtonDown(0)){
                if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, LayerMask.GetMask("Default"))){
                    if(hit.transform.tag == "plant"){
                        StaticEntity plant = hit.transform.GetComponent<StaticEntity>();
                        if(ground.plants.ContainsValue(plant)){
                            ground.plants.Remove(new Vector2Int(plant.mypos.x,plant.mypos.y));
                        }
                        Destroy(plant.gameObject);
                    }
                }
            }
            if(Input.GetMouseButtonDown(1)){
                warning.SetActive(false);
                removing = false;
            }
        }else if(placing){
            warning.SetActive(false);
            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, LayerMask.GetMask("ground"))){
                Vector3Int pos = Vector3Int.RoundToInt(hit.point);
                pos.y = Vector3Int.CeilToInt(hit.point).y;
                ghostPlace.transform.position = pos;
                if (Input.GetMouseButtonDown(0)){
                    if(animal){
                        Destroy(ghostPlace);
                        GameObject newthing = Instantiate(prefabToPlace, pos, Quaternion.identity);
                        newthing.transform.RotateAround(transform.position,Vector3.up,Random.Range(0,90));
                        newthing.GetComponent<DynamicEntity>().ghost = false;
                        placing = false;
                    } else {
                        if(ground.plants.ContainsKey(new Vector2Int(pos.x,pos.z)) && prefabToPlace.GetComponent<StaticEntity>().plantType.name != "grass"){
                            return;
                        }
                        string res = ghostPlace.GetComponent<StaticEntity>().CheckTerrain(Vector3Int.RoundToInt(hit.point)+Vector3Int.down);
                        Debug.Log(res);
                        if(res == "good"){
                            GameObject newthing = Instantiate(prefabToPlace, pos, Quaternion.identity);
                            //newthing.transform.RotateAround(transform.position,Vector3.up,Random.Range(0,90));
                            Vector3 angles = newthing.transform.rotation.eulerAngles;
                            angles.y = Random.Range(0,90);
                            newthing.transform.rotation = Quaternion.Euler(angles);
                            newthing.GetComponent<StaticEntity>().ghost = false;
                            newthing.GetComponent<StaticEntity>().mypos = pos;
                            if( newthing.GetComponent<StaticEntity>().name != "grass"){
                                ground.plants.Add(new Vector2Int(pos.x,pos.z), newthing.GetComponent<StaticEntity>());
                            }
                        }
                    }
                    
                }
                if(Input.GetMouseButtonDown(1)){
                    placing = false;
                    Destroy(ghostPlace);
                }
            }
        }
    }
}
