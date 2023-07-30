using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableBlock : MonoBehaviour
{
    public GroundProperty properties = new GroundProperty();
    public MeshRenderer meshRenderer;
    public Material[] types;
    // 0 is a full block
    // 1 is a wedge block
    // 2 is a corner block
    // 3 are inverted corner blocks
    // only full blocks are important, the others are decoration
    public int type = 0;
    public bool test_thing = false;
    public bool waterSource = false;

    public void UpdateHumidity(){
        if(properties.humidity < 5.0f){
            return;
        }
        Vector3Int[] positions = {Vector3Int.right, Vector3Int.forward, Vector3Int.back, Vector3Int.left, Vector3Int.up, Vector3Int.down};
        Ground ground = transform.parent.GetComponent<Ground>();
        foreach(Vector3Int offset in positions){
            Vector3Int pos = Vector3Int.FloorToInt(transform.position) + offset;
            if(ground.groundEditableBlocks.ContainsKey(pos)){
                if(ground.groundEditableBlocks[pos].properties.humidity < properties.humidity){
                    // abs(ln(x+1))+1
                    ground.groundEditableBlocks[pos].properties.humidity = properties.humidity/(Mathf.Abs(Mathf.Log((properties.humidity/100)+1))+1);
                    ground.groundEditableBlocks[pos].test_thing = true;
                }
            }
        }
    }
    void Start()
    {
          
    }
}
