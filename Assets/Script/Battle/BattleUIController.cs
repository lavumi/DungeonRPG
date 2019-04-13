﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIController : MonoBehaviour
{
    //**********************UI Position Data************************************//
    float _uiOffset = 0.02f;
    float _uiInterOffset = 0.015f;

    float _characterImgRatio = 0.3f;

    float _spriteShakeDist = 30.0f;



    public GameObject battleUI;
    public GameObject dungeonUI;


    GameObject partyPanel;
    GameObject[] partyMenber;
    Slider[] partyHP;
    Text[] partyActionText;
    GameObject[] partyIndicator;
    Image[] partyEffect;
    Image[] partyDead;


    GameObject enemyPanel;
    GameObject[] enemies;
    GameObject[] enemyIndicator;
    Image[] enemyEffect;



    LineRenderer lineRenderer;


    //********************CommandPanel*****************************//
    GameObject commandPanel;
    GameObject AttackPanel;
    GameObject SkillPanel;
    GameObject ItemPanel;

    ActionIconControl[] attackBtns;
    ActionIconControl[] skillBtns;
    ActionIconControl[] itemBtns;

    BattleManager battleManager;


    GameObject gameOver;

    Button startActionQueue;

    int[] monsterPortraitOrder = { 4, 2, 0, 1, 3 };


    //string[] commandScript =
    //{
    //    "Attack",
    //    "Skill",
    //    "Item",
    //    "Formation",
    //    "Run"
    //};

    // Start is called before the first frame update
    void Awake()
    {
        battleManager = GetComponent<BattleManager>();
        partyPanel = battleUI.transform.Find("CharacterPanel").gameObject;
        enemyPanel = battleUI.transform.Find("EnemyPanel").gameObject;
        commandPanel = battleUI.transform.Find("newCommandPanel").gameObject;

        gameOver = battleUI.transform.parent.Find("GameOver").gameObject;
        startActionQueue = battleUI.transform.Find("OKButton").GetComponent<Button>();
        startActionQueue.onClick.AddListener(()=> {
            startActionQueue.interactable = false;
            battleManager.RunQueuedAction();
        });


        partyMenber = new GameObject[5];
        partyHP = new Slider[5];
        partyActionText = new Text[5];
        partyIndicator = new GameObject[5];
        partyEffect = new Image[5];
        partyDead = new Image[5];


        enemies = new GameObject[5];
        enemyIndicator = new GameObject[5];
        enemyEffect = new Image[5];

        lineRenderer = GetComponent<LineRenderer>();


        InitUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InitUI()
    {
        // initCommandUI();
        initNewCommandUI();


        initCharacterUI();
        initEnemyUI();

        battleUI.SetActive(false);
    }

    //void initCommandUI()
    //{
    //    RectTransform targetTransform;

    //    float anchorPosition = 1;
    //    //Command Panel
    //    if (commandPanel != null)
    //    {
    //        //((RectTransform)commandPanel.transform).anchorMin = new Vector2(_uiOffset, _characterImgRatio + _uiOffset);
    //        //((RectTransform)commandPanel.transform).anchorMax = new Vector2(_uiOffset + _commandBtnWidth, 1 - _uiOffset);

    //       // anchorPosition -= _uiOffset;
    //        for (int i = 0; i < 5; i++)
    //        {
    //            targetTransform = (RectTransform)commandPanel.transform.GetChild(i).transform;
    //            //targetTransform.anchorMax = new Vector2(0.98f, anchorPosition);
    //            //anchorPosition -= _commandBtnHeight;
    //            //targetTransform.anchorMin = new Vector2(0.02f, anchorPosition);
    //            //anchorPosition -= _uiInterOffset;

    //            //targetTransform.GetChild(0).GetComponent<Text>().text = commandScript[i];


    //            switch (i)
    //            {
    //                case 0:
    //                    targetTransform.GetComponent<Button>().onClick.AddListener(() => { battleManager.AttackCommand(); });
    //                    break;
    //                case 1:
    //                    targetTransform.GetComponent<Button>().onClick.AddListener(()=> { battleManager.SkillCommand(0); });
    //                    break;
    //                case 2:
    //                    targetTransform.GetComponent<Button>().onClick.AddListener(battleManager.ItemCommand);
    //                    break;
    //                case 3:
    //                    targetTransform.GetComponent<Button>().onClick.AddListener(battleManager.FormationCommand);
    //                    break;
    //                case 4:
    //                    targetTransform.GetComponent<Button>().onClick.AddListener(battleManager.RunCommand);
    //                    break;
    //            }

    //        }

    //        CommandPanelActive(false);
    //    }
    //}

    void initNewCommandUI()
    {
        AttackPanel = commandPanel.transform.Find("AttackContainer").gameObject;
        SkillPanel = commandPanel.transform.Find("SkillContainer").gameObject;
        ItemPanel = commandPanel.transform.Find("ItemContainer").gameObject;

        GameObject iconPrefeb = Resources.Load("Prefebs/SkillIconBase") as GameObject;

        float iconSize = 90.0f;


        attackBtns = new ActionIconControl[2];

        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 1; j++)
            {
                GameObject icon = Instantiate(iconPrefeb);
                icon.transform.SetParent(AttackPanel.transform);
                RectTransform targetTransform = icon.transform.GetComponent<RectTransform>();

                targetTransform.anchoredPosition = new Vector2(
                    10 + (iconSize + 10) * j,
                    10 + (iconSize + 10) * i);
                attackBtns[i + j * 2] = icon.GetComponent<ActionIconControl>();
                attackBtns[i + j * 2].RegisterAction(battleManager.AttackCommand);
            }

        }

        skillBtns = new ActionIconControl[8];

        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                GameObject icon = Instantiate(iconPrefeb);
                icon.transform.SetParent(SkillPanel.transform);
                RectTransform targetTransform = icon.transform.GetComponent<RectTransform>();

                targetTransform.anchoredPosition = new Vector2(
                    10 + (iconSize + 10) * j,
                    10 + (iconSize + 10) * i);
                skillBtns[i + j * 2] = icon.GetComponent<ActionIconControl>();
                skillBtns[i + j * 2].RegisterAction((int a) => { battleManager.SkillCommand(a); });
            }

        }


        itemBtns = new ActionIconControl[8];
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                GameObject icon = Instantiate(iconPrefeb);
                icon.transform.SetParent(ItemPanel.transform);
                RectTransform targetTransform = icon.transform.GetComponent<RectTransform>();

                targetTransform.anchoredPosition = new Vector2(
                    10 + (iconSize + 10) * j,
                    10 + (iconSize + 10) * i);
                itemBtns[i + j * 2] = icon.GetComponent<ActionIconControl>();
            }

        }
    }


    void initCharacterUI()
    {

        //UserCharacterPanel
        var characterImageSize = Screen.height * _characterImgRatio * 0.9f;
        var characterInterval = Screen.height * _uiInterOffset;

        GameObject UIPrefab = Resources.Load("Prefebs/CharacterIconBase") as GameObject;
        Sprite baseImage = Resources.Load("Prefebs/Icon/S_X_chop") as Sprite;

        if (partyPanel != null)
        {
            ((RectTransform)partyPanel.transform).anchorMin = new Vector2(_uiOffset, _uiOffset);
            ((RectTransform)partyPanel.transform).anchorMax = new Vector2(1 - _uiOffset, _uiOffset + _characterImgRatio);

            for (int i = 0; i < 5; i++)
            {
                RectTransform targetTransform;
                GameObject target = Instantiate(UIPrefab);
                target.transform.SetParent( partyPanel.transform);
                targetTransform = target.transform.GetComponent<RectTransform>();

               // GameObject target = partyPanel.transform.GetChild(i).gameObject;
                targetTransform.sizeDelta = new Vector2(characterImageSize, characterImageSize);

                targetTransform.anchoredPosition = new Vector2(
                (characterImageSize + characterInterval) * target.transform.localScale.x * (2 - i),
                0);


                partyMenber[i] = target;
                partyHP[i]          = target.transform.Find("HPSlider").GetComponent<Slider>();
                partyActionText[i]  = target.transform.Find("ActionInfoText").Find("Text").GetComponent<Text>();
                partyIndicator[i]   = target.transform.Find("Indicator").gameObject;
                partyEffect[i] = target.transform.Find("EffectSprite").GetComponent<Image>();
                partyDead[i] = target.transform.Find("DeadPanel").GetComponent<Image>();

                int charIndex = i;
                target.GetComponent<Button>().onClick.AddListener(() => {
                    battleManager.PlayerPortAction(charIndex);
                });
            }


        }
    }

    void initEnemyUI()
    {

        RectTransform targetTransform;
        GameObject UIPrefab = Resources.Load("Prefebs/CharacterIconBase") as GameObject;
        UIPrefab.transform.Find("HPSlider").gameObject.SetActive(false);
        UIPrefab.transform.Find("ActionInfoText").gameObject.SetActive(false);


        var characterImageSize = Screen.height * _characterImgRatio * 0.8f;
        var characterInterval = Screen.height * _uiInterOffset;


        //MonsterCharacterPanel
        if (enemyPanel != null)
        {
            ((RectTransform)enemyPanel.transform).anchorMin = new Vector2(_uiOffset, _uiOffset + _characterImgRatio + _uiOffset);
            ((RectTransform)enemyPanel.transform).anchorMax = new Vector2(1 - _uiOffset, 1 - _uiOffset);

            for (int i = 0; i < 5; i++)
            {
                GameObject target = Instantiate(UIPrefab);
                target.transform.SetParent(enemyPanel.transform);
                targetTransform = target.transform.GetComponent<RectTransform>();

                targetTransform.sizeDelta = new Vector2(characterImageSize, characterImageSize);

                targetTransform.anchoredPosition = new Vector2(
                    (characterImageSize + characterInterval) * target.transform.localScale.x * (2 - i),
                    0);

                enemies[monsterPortraitOrder[i]] = target;//.transform.Find("charSprite").GetComponent<Image>();
                enemyIndicator[monsterPortraitOrder[i]] = target.transform.Find("Indicator").gameObject;
                enemyEffect[monsterPortraitOrder[i]] =  target.transform.Find("EffectSprite").GetComponent<Image>();

                var targetIndex = monsterPortraitOrder[i] + 5; // playerParty 다음부터 카운트되기에 5번부터 인덱싱
                target.GetComponent<CharIconControl>().SetPartyIndex( targetIndex );
                //target.GetComponent<Button>().onClick.AddListener(() => {
                //    battleManager.EnemyPortAction(targetIndex);
                    
                //});
                target.GetComponent<Button>().interactable = false;
                target.SetActive(false);
            }
        }
    }

    public void showBattleUI(bool isShow)
    {
        battleUI.SetActive(isShow);
        dungeonUI.SetActive(!isShow);

        if(isShow == false)
        {
            resetAllUI();
        }
    }

    public void CommandPanelActive(bool isActive)
    {
        commandPanel.SetActive(isActive);
    }



    public void SetActionText(int index, string text)
    {
        partyActionText[index].text = text;
    }


    /// <summary>
    /// Sprite 세팅하기
    /// </summary>
    /// <param name="index"> 0 ~ 4 playerData, 5~ enemyData</param>
    /// <param name="fileName"></param>
    /// <param name="isPlayer"></param>
    public void SetSprite(int index, string fileName) {

       if(index < 5)
        {
            partyMenber[index].SetActive(true);
            partyMenber[index].transform.Find("charSprite").GetComponent<Image>().sprite = Resources.Load<Sprite>(fileName);
        }
        else
        {
            int targetIndex = index - 5;
            enemies[targetIndex].SetActive(true);
            enemies[targetIndex].transform.Find("charSprite").GetComponent<Image>().sprite = Resources.Load<Sprite>(fileName);
        }
    }

    public void showIndicator( bool isShow, int index = 999)
    {
        if( index < 5 )
            partyIndicator[index].SetActive(isShow);
        else
        {
            int targetIndex = index - 5;
            enemyIndicator[targetIndex].SetActive(isShow);
        }
    }

    public void showIndicator(bool isShow, int[] index)
    {
        foreach (int item in index)
        {
            if (item < 5)
                partyIndicator[item].SetActive(isShow);
            else
            {
                int targetIndex = item - 5;
                enemyIndicator[targetIndex].SetActive(isShow);
            }
        }

    }

    public void showEffect(int index, bool isShow, Sprite effectFile = null )
    {
        Sprite effect;
        if (effectFile == null)
            effect = DataManager.GetInstance().GetSkillData(0).effectFile;
        else
            effect = effectFile;

        if ( index < 5)
        {
            partyEffect[index].gameObject.SetActive(isShow);
            partyEffect[index].sprite = effect;
        }
        else
        {
            int targetIndex = index - 5;
            enemyEffect[targetIndex].gameObject.SetActive(isShow);
            enemyEffect[targetIndex].sprite = effect;
        }
    }

    public void setHPGuage(int index, int currentHP, int totalHP)
    {
        partyHP[index].value = (float)currentHP / (float)totalHP;
        partyHP[index].transform.Find("HPText").GetComponent<Text>().text = currentHP.ToString() + " / " + totalHP.ToString();
    }

    public void PortraitShake(int index, bool moveUp)
    {
        if (index < 5)
        {
            StartCoroutine(portMovement(partyMenber[index], moveUp));
        }
        else
        {
            int targetIndex = index - 5;
            StartCoroutine(portMovement(enemies[targetIndex], moveUp));
        }
    }

    IEnumerator portMovement(GameObject target, bool moveup)
    {
        Vector3 pos;
        if (moveup)
        {
            //for (; target.transform.localPosition.y > _spriteShakeDist;)
            //{
            //    pos = target.transform.localPosition;
            //    pos.y += 1;
            //    target.transform.localPosition = pos;
            //    yield return null;
            //}

            pos = ((RectTransform)target.transform).anchoredPosition;
            pos.y = _spriteShakeDist;
            ((RectTransform)target.transform).anchoredPosition = pos;
        }
        else
        {
            //for (; target.transform.localPosition.y < 0;)
            //{
            //    pos = target.transform.localPosition;
            //    pos.y -= 1;
            //    target.transform.localPosition = pos;
            //    yield return null;
            //}

            pos = ((RectTransform)target.transform).anchoredPosition;
            pos.y = 0;
            ((RectTransform)target.transform).anchoredPosition = pos;
        }

        yield return null;
    }

    public void resetAllIndicator()
    {
        foreach (var item in partyIndicator)
        {
            item.SetActive(false);
        }
        foreach (var item in enemyIndicator)
        {
            item.SetActive(false);
        }
    }

    public void EnemyTargetActive(bool isActive)
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].GetComponent<Button>().interactable = isActive;
        }
    }

    public void SetDead(int index , bool isDead)
    {
        if(index < 5)
        {
            if (isDead == true)
            {
                partyDead[index].color = new Color(0, 0, 0, 0.5f);
                partyMenber[index].GetComponent<Button>().interactable = false;
            }
            else
            {
                partyDead[index].color = new Color(0, 0, 0, 0);
                partyMenber[index].GetComponent<Button>().interactable = true;
            }
               
        }
        else
        {
            int targetIndex = index - 5;
            enemies[targetIndex].SetActive(false);
        }
    }

    void resetAllUI()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].gameObject.SetActive(false);
        }
    }

    public void reactiveOkButton()
    {
        startActionQueue.interactable = true;
    }



    public void GameOver()
    {
        StartCoroutine(gameOverFadeIn());
    }

    IEnumerator gameOverFadeIn()
    {

        for (int i = 0; i < 10; i++)
        {
            gameOver.GetComponent<CanvasGroup>().alpha += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
