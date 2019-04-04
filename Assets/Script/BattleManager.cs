using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ariadne;


public class BattleManager : MonoBehaviour
{

    //**********************UI Position Data************************************//
    float _uiOffset = 0.02f;
    float _commandBtnHeight = 0.18f;
    float _commandBtnWidth = 0.2f;
    float _uiInterOffset = 0.015f;

    float _characterImgRatio = 0.3f;

    //**************************************************************************//



    public GameObject battleUI;
    public GameObject dungeonUI;

    [SerializeField]
    GameObject characterPanel;
    Image[] characters;


    [SerializeField]
    GameObject enemyPanel;
    Image[] enemies;


    [SerializeField]
    GameObject commandPanel;
    Button[] commands;



    FadeManager fadeManager;



    CharacterBase[] userPartyData;




    string[] commandScript =
    {
        "Attack",
        "Skill",
        "Item",
        "Formation",
        "Run"
    };

    int[] monsterPosArr1 = { 2, 3, 1, 0, 4 };
    int[] monsterPosArr2 = { 2, 0, 1, 3 };


    // Start is called before the first frame update
    void Start()
    {
        characters = new Image[5];
        enemies = new Image[5];
        
        userPartyData = new CharacterBase[5];

        fadeManager = GetComponent<FadeManager>();
        InitUI();


        SetPlayerData();

    }


    //UI Control
    void InitUI()
    {
        RectTransform targetTransform;

        float anchorPosition = 0;

        initCommandUI();
        initCharacterUI();
        initEnemyUI();

        battleUI.SetActive(false);
    }

    void initCommandUI()
    {
        RectTransform targetTransform;

        float anchorPosition = 0;
        //Command Panel
        if (commandPanel != null)
        {
            ((RectTransform)commandPanel.transform).anchorMin = new Vector2(_uiOffset, _characterImgRatio + _uiOffset);
            ((RectTransform)commandPanel.transform).anchorMax = new Vector2(_uiOffset + _commandBtnWidth, 1 - _uiOffset);

            anchorPosition += _uiOffset;
            for (int i = 0; i < 5; i++)
            {
                targetTransform = (RectTransform)commandPanel.transform.GetChild(i).transform;
                targetTransform.anchorMin = new Vector2(0.02f, anchorPosition);
                anchorPosition += _commandBtnHeight;
                targetTransform.anchorMax = new Vector2(0.98f, anchorPosition);
                anchorPosition += _uiInterOffset;

                targetTransform.GetChild(0).GetComponent<Text>().text = commandScript[4 - i];


                switch (i)
                {
                    case 0:
                        targetTransform.GetComponent<Button>().onClick.AddListener(Attack);
                        break;
                    case 1:
                        targetTransform.GetComponent<Button>().onClick.AddListener(Skill);
                        break;
                    case 2:
                        targetTransform.GetComponent<Button>().onClick.AddListener(Item);
                        break;
                    case 3:
                        targetTransform.GetComponent<Button>().onClick.AddListener(Formation);
                        break;
                    case 4:
                        targetTransform.GetComponent<Button>().onClick.AddListener(ExitBattle);
                        break;
                }
               
            }
        }
    }

    void initCharacterUI()
    {
        RectTransform targetTransform;

        float anchorPosition = 0;

        //UserCharacterPanel
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
                    (characterImageSize + characterInterval) * (2 - i),
                    0);

                characters[i] = targetTransform.GetComponent<Image>();
            }


        }
    }

    void initEnemyUI()
    {
        RectTransform targetTransform;

        float anchorPosition = 0;
        var characterImageSize = Screen.height * _characterImgRatio;
        var characterInterval = Screen.height * _uiInterOffset;
        //MonsterCharacterPanel
        if (enemyPanel != null)
        {
            ((RectTransform)enemyPanel.transform).anchorMin = new Vector2(_uiOffset, _uiOffset + _characterImgRatio + _uiOffset);
            ((RectTransform)enemyPanel.transform).anchorMax = new Vector2(1 - _uiOffset, 1 - _uiOffset);

            for (int i = 0; i < 5; i++)
            {
                targetTransform = (RectTransform)enemyPanel.transform.GetChild(i).transform;
                targetTransform.sizeDelta = new Vector2(characterImageSize, characterImageSize);

                targetTransform.anchoredPosition = new Vector2(
                    (characterImageSize + characterInterval) * (2 - i),
                    0);

                enemies[i] = targetTransform.GetComponent<Image>();
            }


        }
    }

    void setEnemyData(int index, CharacterBase data)
    {
        enemies[index].gameObject.SetActive(true);
        enemies[index].sprite = Resources.Load<Sprite>(data.imgFile);
    }

    public void SetPlayerData()
    {
        userPartyData = CharacterDataManager.GetInstance().GetTestPlayerParty();


        for (int i = 0; i < userPartyData.Length; i++)
        {
            characters[i].sprite = Resources.Load<Sprite>((string)userPartyData[i].imgFile); ;
        }
    }

    public void EnterBattle(CharacterBase[] enemyGroupData)
    {
        StartCoroutine(StartBattle(enemyGroupData));
    }

    private IEnumerator StartBattle(CharacterBase[] enemyGroupData)
    {
        fadeManager.FadeOut();
        var waitTime = new WaitForSeconds(fadeManager.fadeTime);
        yield return waitTime;


        for (int i = 0; i < enemyGroupData.Length; i++)
        {
            setEnemyData(monsterPosArr1[i], enemyGroupData[i]);
        }


        battleUI.SetActive(true);
        dungeonUI.gameObject.SetActive(false);

        fadeManager.FadeIn();
        yield return null;
    }
    


    void Attack()
    {

    }
    
    void Skill()
    {

    }

    void Item()
    {

    }

    void Formation()
    {

    }

    void ExitBattle()
    {
        StartCoroutine(FinishBattle());
    }

    IEnumerator FinishBattle()
    {
        fadeManager.FadeOut();
        var waitTime = new WaitForSeconds(fadeManager.fadeTime);
        yield return waitTime;

        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].gameObject.SetActive(false);
        }

        battleUI.SetActive(false);
        dungeonUI.SetActive(true);
        GetComponent<EncounterManager>().RefreshEncounter();

        fadeManager.FadeIn();
        yield return waitTime;
        GetComponent<Ariadne.MoveController>().setMovable(true);
        yield return null;
    }

    
    //BattleProcessing
}

