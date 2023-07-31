using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPlacer : MonoBehaviour
{
    public Ground ground;
    public List<Vector3Int> occupiedSpace = new List<Vector3Int>();
    public List<Vector3Int> surroundingBlocks = new List<Vector3Int>();
    public GameObject waterPlate;
    public GameObject lakePrefab;
    int surface = 0;
    int depth = 99;
    public bool test= false;
    int counts = 0;
    public void NewFillWater(Vector3Int where){

    }
    public void PlaceWater(Vector3Int where){
        counts = 0;
        PlaceWaterBlock(where);

        GameObject newLake = Instantiate(lakePrefab);
        Lake newLakeCom = newLake.GetComponent<Lake>();
        MeshFilter meshFilter = newLake.GetComponent<MeshFilter>();
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            if(meshFilters[i] == meshFilter){
                i++;
                continue;
            }
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            Destroy(meshFilters[i].gameObject);

            i++;
        }

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.CombineMeshes(combine);
        meshFilter.sharedMesh = mesh;
        newLakeCom.affectedBlocks = surroundingBlocks.ToArray();
        newLakeCom.volume = occupiedSpace.Count;
        newLakeCom.maxVolume = occupiedSpace.Count;
        newLakeCom.level = surface-(depth-1);
        newLakeCom.ground = ground;
        occupiedSpace.Clear();
        surface = 0;
    }
    void PlaceWaterBlock(Vector3Int where){
        if(counts >= 700){
            return;
        }
        counts += 1;
        if(occupiedSpace.Contains(where)){
            counts -=1;
            return;
        }
        if(surface < where.y){
            surface = where.y;
        }
        
        if(ground.groundEditableBlocks.ContainsKey(where)){
            surroundingBlocks.Add(where);
            ground.groundEditableBlocks[where].properties.humidity = 100;
            ground.groundEditableBlocks[where].UpdateHumidity();
            if (ground.groundEditableBlocks[where].type == 0){
                counts -=1;
                return;
                
            }
        }
        
        if(depth > where.y){
            depth = where.y;
        }
        occupiedSpace.Add(where);
        if(where.y == surface){
            GameObject plane = Instantiate(waterPlate, (Vector3)where+(Vector3.up/2.2f), Quaternion.identity);
            plane.transform.parent = transform;
        }
        if(where.y > 0){
            PlaceWaterBlock(where + Vector3Int.down);
        }
        if(where.x > 0){
            PlaceWaterBlock(where + Vector3Int.left);
        }
        if(where.z > 0){
            PlaceWaterBlock(where + Vector3Int.back);
        }
        if(where.x < ground.size-1){
            PlaceWaterBlock(where + Vector3Int.right);
        }
        if(where.z < ground.size-1){
            PlaceWaterBlock(where + Vector3Int.forward);
        }
        counts -=1;
            
        
    }
    void Update(){
        if(test){
            PlaceWater(new Vector3Int(40,2,25));
            test = false;
        }
    }
}
