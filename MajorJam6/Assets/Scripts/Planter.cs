using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planter : MonoBehaviour
{
    public bool placing = false;
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
    }
    void Update()
    {
        RaycastHit hit;
        if(placing){
            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100)){
                Vector3Int pos = Vector3Int.RoundToInt(hit.point);
                pos.y = Vector3Int.CeilToInt(hit.point).y;
                ghostPlace.transform.position = pos;
                if (Input.GetMouseButtonDown(0)){
                    string res = ghostPlace.GetComponent<StaticEntity>().CheckTerrain(Vector3Int.RoundToInt(hit.point)+Vector3Int.down);
                    Debug.Log(res);
                    if(res == "good"){
                        Destroy(ghostPlace);
                        GameObject newthing = Instantiate(prefabToPlace, pos, Quaternion.identity);
                        newthing.transform.RotateAround(transform.position,Vector3.up,Random.Range(0,90));
                        newthing.GetComponent<StaticEntity>().ghost = false;
                        newthing.GetComponent<StaticEntity>().mypos = pos;
                        ground.plants.Add(new Vector2Int(pos.x,pos.z), newthing.GetComponent<StaticEntity>());
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
