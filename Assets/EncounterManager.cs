using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterManager : MonoBehaviour
{
    public Canvas dungeonCanvas;
    public BattleManager battleManager;

    GameObject minimapBG;
    // Start is called before the first frame update
    void Start()
    {
        minimapBG = dungeonCanvas.transform.Find("MapParent").Find("MapBackground").gameObject;
       // minimapBG.GetComponent<UnityEngine.UI.Image>().color = new Color(255, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Encounter()
    {
        battleManager.EnteringBattle();
        dungeonCanvas.gameObject.SetActive(false);
    }

    public void BattleFinished()
    {
        dungeonCanvas.gameObject.SetActive(true);
    }
}
