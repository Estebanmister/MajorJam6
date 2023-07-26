using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableBlock : MonoBehaviour
{
    public GroundProperty properties;
    public MeshRenderer meshRenderer;
    public Material[] types;
    void Start()
    {
        meshRenderer.material = types[Random.Range(0,2)];   
    }
    void Update()
    {
        // update properties constantly while on update loop because the player will be actively editing this

    }
}
