using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Observer : MonoBehaviour
{
    public Ground ground;
    public World world;
    public TMP_Text k;
    public TMP_Text n;
    public TMP_Text p;
    public TMP_Text humid;
    public TMP_Text airhumid;
    public TMP_Text time;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time.text = "Day " + ((world.TotalTime/world.DayLengthSeconds)%world.WeekLengthDays).ToString("f0") + " Week " + Mathf.Floor((float)(world.TotalTime/world.DayLengthSeconds)/world.WeekLengthDays).ToString("f0");
        airhumid.text = (world.humidity/100).ToString("P1");
        GroundProperty groundProperty = null;
        if(ground.editMode){
            RaycastHit hit;
            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, LayerMask.GetMask("groundBlock"))){
                groundProperty = hit.transform.GetComponent<EditableBlock>().properties;
            }
        } else {
            RaycastHit hit;
            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, LayerMask.GetMask("ground"))){
                Vector3Int pos = Vector3Int.RoundToInt(hit.point);
                pos.y = Vector3Int.CeilToInt(hit.point).y-1;
                if(ground.groundBlocksProp.ContainsKey(pos)){
                    groundProperty = ground.groundBlocksProp[pos];
                }
            }
        }
        if(groundProperty != null){
            k.text = "<mspace=12>"+(groundProperty.K/10).ToString("P1");
            p.text = "<mspace=12>"+(groundProperty.P/10).ToString("P1");
            n.text = "<mspace=12>"+(groundProperty.N/10).ToString("P1");
            humid.text = "<mspace=12>"+(groundProperty.humidity/100).ToString("P1");
        }
    }
}
