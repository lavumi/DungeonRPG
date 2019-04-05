using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EncounterManager : MonoBehaviour
{


    BattleManager battleManager;
    public GameObject minimapBG;




    //incount Chanve variable
    int incounterChance = 0;
    float maxCounter = 10;
    float maxChance = 0.3f;


    void Start()
    {
        battleManager = GetComponent<BattleManager>();
    }

    public void Encounter()
    {
        battleManager.EnterBattle(CharacterDataManager.GetInstance().GetMonsterGroup());
    }




    public void IncEncounterChance()
    {

        incounterChance++;

        float red =  ((float)incounterChance / maxCounter );
        float green =(( maxCounter- (float)incounterChance) / maxCounter);

        minimapBG.GetComponent<UnityEngine.UI.Image>().color = new Color(red, green, 0);



        float rnd = Random.Range(0.0f, 1.0f);
        if (rnd < ( incounterChance / maxCounter ) * maxChance )
        {
            incounterChance = 0;
            Encounter();
        }
    }


    public void RefreshEncounter()
    {
        minimapBG.GetComponent<UnityEngine.UI.Image>().color = new Color(0,1,0);
       
    }
}
