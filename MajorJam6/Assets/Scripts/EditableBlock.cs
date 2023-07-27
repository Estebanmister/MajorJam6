using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableBlock : MonoBehaviour
{
    public GroundProperty properties;
    public MeshRenderer meshRenderer;
    public Material[] types;
    // 0 is a full block
    // 1 is a wedge block
    // 2 is a corner block
    // 3 are inverted corner blocks
    // only full blocks are important, the others are decoration
    public int type = 0;
    void Start()
    {
        meshRenderer.material = types[Random.Range(0,2)];   
    }
    void Update()
    {
        // update properties constantly while on update loop because the player will be actively editing this

    }
}
