using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lake : MonoBehaviour
{
    public float volume;
    public float maxVolume;
    public float level;
    public List<Vector3Int> affectedBlocks = new List<Vector3Int>();
    public Ground ground;
    void Start()
    {
        
    }
    float map(float val, float oldmin, float oldmax, float newmin, float newmax){
        return (val - oldmin) * (newmax - newmin) / (oldmax - oldmin) + newmin;
    }
    void Update()
    {
        Vector3 pos = transform.position;
        pos.y = map(volume, 0, maxVolume, -level, 0);
        transform.position = pos;
        if(volume < 0.1f){
            ground.LoadGround();
            foreach(Vector3Int blockpos in affectedBlocks){
                ground.groundEditableBlocks[blockpos].properties.humidity = 0;
            }
            Destroy(gameObject);
        }
    }
}
