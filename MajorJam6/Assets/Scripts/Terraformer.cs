using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terraformer : MonoBehaviour
{
    public bool placingBlock = false;
    public bool deletingBlock = false;

    public bool placingWater = false;
    GameObject blockGhost;
    public GameObject blockprefab;
    Ground ground;
    WaterPlacer waterPlacer;
    public GameObject waterPlateThing;
    public void BeginPlaceBlock(){
        DestroyImmediate(blockGhost);
        blockGhost = Instantiate(blockprefab);
        placingBlock = !placingBlock;
        deletingBlock = false;
        deletingBlock = false;
    }
    void Start(){
        ground = GameObject.FindGameObjectWithTag("ground").GetComponent<Ground>();
        waterPlacer = GameObject.FindGameObjectWithTag("waterplacer").GetComponent<WaterPlacer>();
    }
    public void BeginDeleteBlock(){
        deletingBlock = !deletingBlock;
        placingWater = false;
        placingBlock = false;
    }
    public void BeginWaterPlace(){
        placingWater = !placingWater;
        deletingBlock = false;
        placingBlock = false;
    }
    void Update()
    {
        RaycastHit hit;
        if(placingBlock){
            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, LayerMask.GetMask("groundBlock"))){
                Vector3Int pos = Vector3Int.RoundToInt(hit.transform.position+Vector3Int.RoundToInt(hit.normal));
                blockGhost.transform.position = pos;
                if(Input.GetMouseButtonDown(0)){
                    
                    if(ground.groundEditableBlocks.ContainsKey(pos)){
                        return;
                    } else {
                        ground.PlaceBlock(pos);
                    }
                    
                }
                if(Input.GetMouseButtonDown(1)){
                    placingBlock = false;
                    Destroy(blockGhost);
                }
            }
        }
        if(deletingBlock){
            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, LayerMask.GetMask("groundBlock"))){
                Vector3Int pos = Vector3Int.RoundToInt(hit.transform.position);
                if(Input.GetMouseButtonDown(0)){
                    Debug.Log(pos);
                    if(ground.groundEditableBlocks.ContainsKey(pos)){
                        Debug.Log("deleted");
                        Destroy(ground.groundEditableBlocks[pos].gameObject);
                        ground.groundEditableBlocks.Remove(pos);
                    }
                }
                if(Input.GetMouseButtonDown(1)){
                    Debug.Log("gone");
                    deletingBlock = false;
                }
            }
        }
        if(placingWater){
            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, LayerMask.GetMask("groundBlock"))){
                Vector3Int pos = Vector3Int.RoundToInt(hit.transform.position+Vector3.up);
                waterPlateThing.SetActive(true);
                waterPlateThing.transform.localScale = new Vector3(ground.size*0.05f, 1, ground.size*0.05f);
                waterPlateThing.transform.position = pos+Vector3.up;
                if(Input.GetMouseButtonDown(0)){
                    if(ground.groundEditableBlocks.ContainsKey(pos)){
                        return;
                    } else {
                        waterPlacer.PlaceWater(pos);
                    }
                }
                if(Input.GetMouseButtonDown(1)){
                    waterPlateThing.SetActive(false);
                    placingWater = false;
                }
            } else {
                waterPlateThing.SetActive(false);
            }
        }
    }
}
