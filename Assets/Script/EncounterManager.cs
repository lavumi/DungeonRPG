using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EncounterManager : MonoBehaviour
{




    BattleManager battleManager;
    public GameObject dungeonUI;



    GameObject minimapBG;



    //incount Chanve variable
    int incounterChance = 0;
    float maxCounter = 10;
    float maxChance = 0.3f;


    string[] monsterSprite =
    {
        "Image/enemyChar00",
        "Image/enemyChar01",
        "Image/enemyChar02",
    };



    void Start()
    {
        battleManager = GetComponent<BattleManager>();
        minimapBG = dungeonUI.transform.Find("MapParent").Find("MapBackground").gameObject;
       // minimapBG.GetComponent<UnityEngine.UI.Image>().color = new Color(255, 0, 0);
    }


    void Update()
    {

    }

    public void Encounter()
    {
        GetComponent<Ariadne.MoveController>().setMovable(false);

        int monsterCount = Random.Range(1, 6);
        CharacterBase[] enemyGroupData = new CharacterBase[monsterCount];

        for (int i = 0; i < monsterCount; i++)
        {
            StatusData tempData = new StatusData();
            tempData.SetData(1, 1, 1, 1);
            int imgRnd = Random.Range(0, monsterSprite.Length);
            enemyGroupData[i] = new CharacterBase(monsterSprite[imgRnd], "testMob", 1, tempData);
        }

        battleManager.EnterBattle(enemyGroupData);
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
