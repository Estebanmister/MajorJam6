using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NutrientMap : MonoBehaviour
{
    Ground ground;
    public bool shown = false;
    public Texture2D kmap;
    public Texture2D nmap;
    public Texture2D pmap;
    public GameObject kmapobj;
    public GameObject pmapobj;
    public GameObject nmapobj;

    public bool t1 = false;
    public bool t2 = false;
    public bool t3 = false;

    void Start()
    {
        ground = GameObject.FindGameObjectWithTag("ground").GetComponent<Ground>();
        Vector3 pos = transform.position;
        pos.x = ground.size/2;
        pos.z = ground.size/2;
        transform.position = pos;
    }
    float map(float val, float oldmin, float oldmax, float newmin, float newmax){
        return (val - oldmin) * (newmax - newmin) / (oldmax - oldmin) + newmin;
    }
    public void ShowMap(string nutrient, bool hide = false){
        switch(nutrient){
            case "k":
            kmapobj.SetActive(!hide);
            break;
            case "n":
            nmapobj.SetActive(!hide);
            break;
            case "p":
            pmapobj.SetActive(!hide);
            break;
        }
        shown = true;
    }
    public void UpdateMaps(){
        int size = ground.size;
        transform.localScale = new Vector3(0.1f*size,1,0.1f*size);
        kmap.Reinitialize(size,size);
        pmap.Reinitialize(size,size);
        nmap.Reinitialize(size,size);
        kmap.Apply();
        pmap.Apply();
        nmap.Apply();
        for(int i = 0; i < size; i += 1){
            for(int ii = 0; ii < size; ii += 1){
                RaycastHit hit;
                Vector2Int pos2d = new Vector2Int(i,ii);
                if(Physics.Raycast(new Vector3(i, transform.position.y,ii),Vector3.down, out hit, 40)){
                    Vector3Int blockpos = Vector3Int.FloorToInt(hit.point);
                    
                    Vector2 pixelUV = new Vector2(i,ii);
                    //pixelUV.x *= tex.width;
                    //pixelUV.y *= tex.height;
                    Color color;
                    color = Color.red;
                    color.a = ground.groundBlocksProp[blockpos].K;
                    color.a = Mathf.Round(color.a*20)/20.0f;
                    kmap.SetPixel((int)pixelUV.x, (int)pixelUV.y, color);
                    kmap.Apply();

                    color = Color.blue;
                    color.a = ground.groundBlocksProp[blockpos].N;
                    color.a = Mathf.Round(color.a*20)/20.0f;
                    nmap.SetPixel((int)pixelUV.x, (int)pixelUV.y, color);
                    nmap.Apply();

                    color = Color.white;
                    color.a = ground.groundBlocksProp[blockpos].P;
                    color.a = Mathf.Round(color.a*20)/20.0f;
                    pmap.SetPixel((int)pixelUV.x, (int)pixelUV.y, color);
                    pmap.Apply();
                }
            }
        }
        
        
        
    }
    void Update(){
        if(t1){
            t1 = false;
            ShowMap("k");
        }
        if(t2){
            t2 = false;
            ShowMap("p");
        }
        if(t3){
            t3 = false;
            ShowMap("n");
        }
    }
}
