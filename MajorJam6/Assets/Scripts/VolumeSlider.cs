using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.UI;
public class VolumeSlider : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private string busPath = "";
    public Slider slider;
    FMOD.Studio.Bus bus;
    void Start()
    {
        if(busPath != ""){
            bus = RuntimeManager.GetBus(busPath);
        }
        bus.getVolume(out float val);
        slider.value = val;
    }

    public void ValueChange(float val){
        bus.setVolume(slider.value);
    }
}
