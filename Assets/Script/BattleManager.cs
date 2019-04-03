using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public GameObject gameController;
    public Canvas battleCanvas;


    [SerializeField]
    GameObject fadeMaskPanel;
    Image panel;

    [SerializeField]
    GameObject characterPanel;
    Image[] characters;

    [SerializeField]
    GameObject commandPanel;
    Button[] commands;





    float _uiOffset = 0.02f;
    float _commandBtnHeight = 0.18f;
    float _commandBtnWidth = 0.2f;
    float _uiInterOffset = 0.015f;


    float _characterImgRatio = 0.3f;


    string[] commandScript =
    {
        "Attack",
        "Skill",
        "Item",
        "Formation",
        "Run"
    };

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("start");

        InitUI();

    }




    public void EnteringBattle()
    {
        battleCanvas.gameObject.SetActive(true);
    }


    void InitUI()
    {
        string targetName;


        GameObject targetObject;
        RectTransform targetTransform;

        float anchorPosition = 0;


        if( commandPanel != null)
        {
            ((RectTransform)commandPanel.transform).anchorMin = new Vector2(_uiOffset, _characterImgRatio + _uiOffset);
            ((RectTransform)commandPanel.transform).anchorMax = new Vector2(_uiOffset + _commandBtnWidth, 1 - _uiOffset);

            anchorPosition += _uiOffset;
            for( int i = 0;i < 5; i++)
            {
                targetTransform = (RectTransform)commandPanel.transform.GetChild(i).transform;
                targetTransform.anchorMin = new Vector2(0.02f, anchorPosition);
                anchorPosition += _commandBtnHeight;
                targetTransform.anchorMax = new Vector2(0.98f, anchorPosition);
                anchorPosition += _uiInterOffset;

                targetTransform.GetChild(0).GetComponent<Text>().text = commandScript[4-i];

                targetTransform.GetComponent<Button>().onClick.AddListener(ExitBattle);
            }
        }


        var characterImageSize = Screen.height * _characterImgRatio;
        var characterInterval = Screen.height * _uiInterOffset;
        if (characterPanel != null)
        {
            ((RectTransform)characterPanel.transform).anchorMin = new Vector2(_uiOffset, _uiOffset);
            ((RectTransform)characterPanel.transform).anchorMax = new Vector2(1 - _uiOffset, _uiOffset + _characterImgRatio);

            for (int i = 0; i < 5; i++)
            {
                targetTransform = (RectTransform)characterPanel.transform.GetChild(i).transform;
                targetTransform.sizeDelta = new Vector2(characterImageSize, characterImageSize);

                targetTransform.anchoredPosition = new Vector2(
                    (characterImageSize + characterInterval) * (2 - i ),
                    0);

                targetTransform.GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/testChar0" + i);

            }


        }

        battleCanvas.gameObject.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    void ExitBattle()
    {
        battleCanvas.gameObject.SetActive(false);
        gameController.GetComponent<EncounterManager>().BattleFinished();
    }
}

