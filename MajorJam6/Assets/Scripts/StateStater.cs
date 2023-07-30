using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StateStater : MonoBehaviour
{
    // states the state of the game
    public TMP_Text gameState;
    public Terraformer terraformer;
    public Ground ground;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(terraformer.placingBlock){
            gameState.text = "Placing Blocks";
        } else if(terraformer.deletingBlock){
            gameState.text = "Digging Blocks";
        }
        else if(terraformer.placingWater){
            gameState.text = "Placing Lakes";
        } else {
            gameState.text = "";
        }
        
    }
}
