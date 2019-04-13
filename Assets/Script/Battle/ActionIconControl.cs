using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ActionIconControl : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{

    Vector2 defaultPosition;
    Vector2 centerPos;
    GameObject lastOverlay;

    BattleManager battleManager;

    BattleManager.Command myCmd;


    Image line;

    Image icon;



    public void OnBeginDrag(PointerEventData eventData)
    {
        lastOverlay = null;
        defaultPosition = this.transform.position;
        icon.raycastTarget = false;

       // line.rectTransform.anchoredPosition = defaultPosition;
        line.gameObject.SetActive(true);

        myCmd(0);

    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 currentPos = Input.mousePosition;
        this.transform.position = currentPos - centerPos;

        Vector2 indicatorVector = currentPos - centerPos - defaultPosition;

        float dist =  (indicatorVector + centerPos).magnitude;
        line.rectTransform.sizeDelta = new Vector2(5, dist);

        float angle = Mathf.Atan2(indicatorVector.x, -indicatorVector.y) * Mathf.Rad2Deg ;
        line.rectTransform.rotation = Quaternion.Euler(0, 0, angle);


        if (lastOverlay != eventData.pointerEnter)
        {
            Debug.Log(lastOverlay);
            if (lastOverlay != null && lastOverlay.transform.parent.Find("highlight") != null)
            {
                lastOverlay.transform.parent.Find("highlight").gameObject.SetActive(false);
            }

            lastOverlay = eventData.pointerEnter;
            if (lastOverlay != null && lastOverlay.transform.parent.Find("highlight") != null)
            {
                lastOverlay.transform.parent.Find("highlight").gameObject.SetActive(true);
            }
        }
        
    }
 
    public void OnEndDrag(PointerEventData eventData)
    {
       //Debug.Log("OnDrag \n" + eventData);
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); 
        this.transform.position = defaultPosition;

       // line.rectTransform.anchoredPosition = defaultPosition;
        line.gameObject.SetActive(false);



        if (lastOverlay != null && lastOverlay.transform.parent.Find("highlight") != null)
        {
            int targetPArtyIndex = lastOverlay.transform.parent.GetComponent<CharIconControl>().GetPartyIndex();
            battleManager.EnemyPortAction(targetPArtyIndex);
            lastOverlay.transform.parent.Find("highlight").gameObject.SetActive(false);
        }
        icon.raycastTarget = true;

        

    }

    // Start is called before the first frame update
    void Start()
    {
        battleManager = GameObject.Find("BattleController").GetComponent<BattleManager>();

        lastOverlay = null;
        centerPos = GetComponent<RectTransform>().sizeDelta / 2;
        line = transform.Find("line").GetComponent<Image>();
        line.gameObject.SetActive(false);

        icon = transform.Find("IconImage").GetComponent<Image>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GetDropTarget()
    {

        return lastOverlay;
    }

    public void RegisterAction(BattleManager.Command action)
    {
        Debug.Log(action);
        myCmd = action;
    }
}
