using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIController : MonoBehaviour
{
    //**********************UI Position Data************************************//
    float _uiOffset = 0.02f;
    float _commandBtnHeight = 0.15f;
    float _commandBtnWidth = 0.2f;
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

    GameObject commandPanel;
    Button[] commands;

    BattleManager battleManager;


    GameObject gameOver;

    Button startActionQueue;

    int[] monsterPortraitOrder = { 4, 2, 0, 1, 3 };


    int prevIndicator = -1;

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
        battleManager = GetComponent<BattleManager>();
        partyPanel = battleUI.transform.Find("CharacterPanel").gameObject;
        enemyPanel = battleUI.transform.Find("EnemyPanel").gameObject;
        commandPanel = battleUI.transform.Find("CommandPanel").gameObject;

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

        InitUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InitUI()
    {
        initCommandUI();
        initCharacterUI();
        initEnemyUI();

        battleUI.SetActive(false);
    }

    void initCommandUI()
    {
        RectTransform targetTransform;

        float anchorPosition = 1;
        //Command Panel
        if (commandPanel != null)
        {
            ((RectTransform)commandPanel.transform).anchorMin = new Vector2(_uiOffset, _characterImgRatio + _uiOffset);
            ((RectTransform)commandPanel.transform).anchorMax = new Vector2(_uiOffset + _commandBtnWidth, 1 - _uiOffset);

            anchorPosition -= _uiOffset;
            for (int i = 0; i < 5; i++)
            {
                targetTransform = (RectTransform)commandPanel.transform.GetChild(i).transform;
                targetTransform.anchorMax = new Vector2(0.98f, anchorPosition);
                anchorPosition -= _commandBtnHeight;
                targetTransform.anchorMin = new Vector2(0.02f, anchorPosition);
                anchorPosition -= _uiInterOffset;

                targetTransform.GetChild(0).GetComponent<Text>().text = commandScript[i];


                switch (i)
                {
                    case 0:
                        targetTransform.GetComponent<Button>().onClick.AddListener(battleManager.AttackCommand);
                        break;
                    case 1:
                        targetTransform.GetComponent<Button>().onClick.AddListener(battleManager.SkillCommand);
                        break;
                    case 2:
                        targetTransform.GetComponent<Button>().onClick.AddListener(battleManager.ItemCommand);
                        break;
                    case 3:
                        targetTransform.GetComponent<Button>().onClick.AddListener(battleManager.FormationCommand);
                        break;
                    case 4:
                        targetTransform.GetComponent<Button>().onClick.AddListener(battleManager.RunCommand);
                        break;
                }

            }

            CommandPanelActive(false);
        }
    }

    void initCharacterUI()
    {

        RectTransform targetTransform;

        float anchorPosition = 0;

        //UserCharacterPanel
        var characterImageSize = Screen.height * _characterImgRatio * 0.9f;
        var characterInterval = Screen.height * _uiInterOffset;

        GameObject UIPrefab = Resources.Load("Prefebs/IconBase") as GameObject;

        if (partyPanel != null)
        {
            ((RectTransform)partyPanel.transform).anchorMin = new Vector2(_uiOffset, _uiOffset);
            ((RectTransform)partyPanel.transform).anchorMax = new Vector2(1 - _uiOffset, _uiOffset + _characterImgRatio);

            for (int i = 0; i < 5; i++)
            {
                GameObject target = Instantiate(UIPrefab);
                target.transform.parent = partyPanel.transform;
                targetTransform = target.transform.GetComponent<RectTransform>();
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
        GameObject UIPrefab = Resources.Load("Prefebs/IconBase") as GameObject;
        UIPrefab.transform.Find("HPSlider").gameObject.SetActive(false);
        UIPrefab.transform.Find("ActionInfoText").gameObject.SetActive(false);

        float anchorPosition = 0;
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
                target.transform.parent = enemyPanel.transform;
                targetTransform = target.transform.GetComponent<RectTransform>();

                targetTransform.sizeDelta = new Vector2(characterImageSize, characterImageSize);

                targetTransform.anchoredPosition = new Vector2(
                    (characterImageSize + characterInterval) * target.transform.localScale.x * (2 - i),
                    0);

                enemies[monsterPortraitOrder[i]] = target;//.transform.Find("charSprite").GetComponent<Image>();
                enemyIndicator[monsterPortraitOrder[i]] = target.transform.Find("Indicator").gameObject;
                enemyEffect[monsterPortraitOrder[i]] =  target.transform.Find("EffectSprite").GetComponent<Image>();

                var targetIndex = monsterPortraitOrder[i] + 5; // playerParty 다음부터 카운트되기에 5번부터 인덱싱
                target.GetComponent<Button>().onClick.AddListener(() => {
                    battleManager.EnemyPortAction(targetIndex);
                    
                });
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

    public void showIndicator(int index, bool isShow)
    {
        if( index < 5 )
            partyIndicator[index].SetActive(isShow);
        else
        {
            int targetIndex = index - 5;
            enemyIndicator[targetIndex].SetActive(isShow);
        }

    }

    public void showEffect(int index, bool isShow)
    {
        if( index < 5 )
            partyEffect[index].gameObject.SetActive(isShow);
        else
        {
            int targetIndex = index - 5;
            enemyEffect[targetIndex].gameObject.SetActive(isShow);
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
