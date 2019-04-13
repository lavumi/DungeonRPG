using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharIconControl : MonoBehaviour
{
    Image highlight;
    Image charSprite;
    Image indicator;
    Slider HPSlider;
    Text actionText;
    Image effectSprite;
    Image deadPanel;

    int index;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPartyIndex( int index )
    {
        this.index = index;
    }

    public int GetPartyIndex()
    {
        return index;
    }
}
