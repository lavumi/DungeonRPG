using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Ariadne;


public class BattleManager : MonoBehaviour
{
    //***********************UI Object & Data**********************************//


    public GameObject gameController;

    BattleUIController uiController;





    FadeManager fadeManager;

    GameCharacter[] userPartyData;
    GameCharacter[] enemyPartyData;






    int currentPlayerIndex = -1;
    int[] targetIndices = null;
   // int currentTargetIndex = 5;

    public enum BtAction
    {
        Attack,Skill, Item, Formation, Run, NULL = 999
    }
    BtAction currentAction = BtAction.NULL;
    int curActionIndex = -1;




    //*************************BattleAction INFO*************************************//

    private delegate void BattleAction(GameCharacter[] from, int[] fromIndex, GameCharacter[] to, int[] toIndex, int actionIndex);

    struct BattleActionData
    {
        BattleAction action;
        GameCharacter[] from;
        GameCharacter[] to;
        int[] fromIndex;
        int[] toIndex;
        int actionIndex;
        public int actionDelay;
        public int priority;


        public BattleActionData(BattleAction action,
                GameCharacter[] from, int[] fromIndex,
                GameCharacter[] to, int[] toIndex,
                int actionIndex,
                int actionDelay, 
                int priority)
        {
            this.action = action;
            this.priority = priority;
            this.from = from;
            this.to = to;
            this.actionIndex = actionIndex;
            this.actionDelay = actionDelay;
            this.fromIndex = fromIndex;
            this.toIndex = toIndex;

        }


        public bool runAction()
        {
            foreach (var item in from)
            {
                if (item.CurrentStat.VIT < 1)
                {
                    return false;
                }
                   
            }

            bool targetAllDead = true;
            foreach (var item in to)
            {
                targetAllDead = targetAllDead & (item.CurrentStat.VIT < 1);
            }
            if(targetAllDead == true && to.Length != 0)
            {
                return false;
            }

            this.action(from, fromIndex, to, toIndex, actionIndex);
            return true;
        }
    }

    List<BattleActionData> battleActionQueue;
    BattleActionData[] characterBattleData;


    //********************************************************************************//



    void Start()
    {
        uiController = GetComponent<BattleUIController>();

        characterBattleData = new BattleActionData[5];
        userPartyData = new GameCharacter[5];
        enemyPartyData = null;
        
        fadeManager = gameController.GetComponent<FadeManager>();

        InitPartyData();

        battleActionQueue = new List<BattleActionData>();

    }


    #region "[UI CONTROL]"

    public void AttackCommand()
    {
        
        currentAction = BtAction.Attack;
        uiController.showIndicator(true, targetIndices);
        uiController.EnemyTargetActive(true);
        uiController.SetActionText(currentPlayerIndex, "Attack");

    }

    public void SkillCommand(int index)
    {
        int skillIndex = userPartyData[currentPlayerIndex].skill;
        curActionIndex = skillIndex;
        currentAction = BattleManager.BtAction.Skill;
        SkillBase curSkill = DataManager.GetInstance().GetSkillData(skillIndex);

        uiController.SetActionText(currentPlayerIndex, curSkill.name);

        switch (curSkill.targetType)
        {
            case TargetType.SingleEnemy:
                uiController.showIndicator(true, targetIndices);
                uiController.EnemyTargetActive(true);
                break;
            case TargetType.AllEnemy:
                targetIndices = new int[enemyPartyData.Length];
                for (int i = 0; i < enemyPartyData.Length; i++)
                {
                    targetIndices[i] = i + 5;
                }
                uiController.CommandPanelActive(false);
                makeBattleAction();

                resetActionData();
                break;
            case TargetType.SingleAlli:
                targetIndices = new int[1] { 0 };
                break;
            case TargetType.AllAlli:
                break;
        }



    }

    public void ItemCommand()
    {
        currentAction = BattleManager.BtAction.Item;
        uiController.EnemyTargetActive(true);
        uiController.SetActionText(currentPlayerIndex, "Item");
    }

    public void FormationCommand()
    {
        currentAction = BattleManager.BtAction.Formation;
        uiController.EnemyTargetActive(true);
        uiController.SetActionText(currentPlayerIndex, "Formation");
    }

    public void RunCommand()
    {
        currentAction = BattleManager.BtAction.Run;
        uiController.SetActionText(currentPlayerIndex, "Run");

        uiController.showIndicator(false);
        uiController.EnemyTargetActive(false);
        uiController.CommandPanelActive(false);

        targetIndices = null;
        makeBattleAction();

        resetActionData();
    }




    void setEnemyData(int index, GameCharacter data)
    {
        uiController.SetSprite(index, data.baseInfo.imgFile);
    }

    void InitPartyData()
    {
        userPartyData = DataManager.GetInstance().GetTestPlayerParty();

        for (int i = 0; i < userPartyData.Length; i++)
        {
            uiController.SetSprite( i, userPartyData[i].baseInfo.imgFile);
        }
        refreshPartyData();
    }

    void refreshData()
    {
        refreshPartyData();
        refreshUI();
    }

    void refreshUI()
    {
        for (int i = 0; i < userPartyData.Length; i++)
        {
            if (isDead(i))
            {
                uiController.SetDead(i, true);
            }
        }
        for (int i = 0; i < enemyPartyData.Length; i++)
        {
            if (isDead(i + 5))
            {
                uiController.SetDead(i + 5, true);
            }
        }
    }

    void refreshPartyData()
    {
        for (int i = 0; i < userPartyData.Length; i++)
        {
            int currentHP = userPartyData[i].CurrentStat.VIT;
            int totalHP = userPartyData[i].baseInfo.Stat.VIT;
            uiController.setHPGuage(i, currentHP, totalHP);
            

        }
    }

    public void PlayerPortAction(int index)
    {

        if(currentAction != BtAction.NULL)
        {
            makeBattleAction();

        }


        //----readyTo Make Action
        resetActionData();
        currentPlayerIndex = index;
        uiController.showIndicator(  true, currentPlayerIndex );
        uiController.CommandPanelActive(true);
        //------
    }   

    void resetActionData()
    {
        uiController.resetAllIndicator();
        currentPlayerIndex = -1;
        targetIndices = new int[1] { targetPicker(true) };
        curActionIndex = -1;
        currentAction = BtAction.NULL;
        uiController.CommandPanelActive(false);
    }


    public void EnemyPortAction(int index)
    {
        uiController.showIndicator(false, targetIndices);
        targetIndices = null;
        targetIndices = new int[1] { index };
        uiController.showIndicator(false, targetIndices);
        uiController.EnemyTargetActive(false);
        uiController.CommandPanelActive(false);
        makeBattleAction();
        resetActionData();
    }



    #endregion


    #region "[Battle Actions]"


    BattleActionData makeBattleActionData(int[] fromIndex, int[] toIndex)
    {
        GameCharacter[] from = new GameCharacter[fromIndex.Length];
        int priorityCount = 0;
        for (int i = 0; i < fromIndex.Length; i++)
        {
            if(fromIndex[i] < 5)
            {
                from[i] = userPartyData[fromIndex[i]];
            }
            else
            {
                int targetIndex = fromIndex[i] - 5;
                from[i] = enemyPartyData[targetIndex];
            }
            priorityCount += from[i].CurrentStat.AGI;
        }
        priorityCount /= fromIndex.Length;

        GameCharacter[] to = new GameCharacter[toIndex.Length];
        for (int i = 0; i < toIndex.Length; i++)
        {
            if (toIndex[i] < 5)
            {
                to[i] = userPartyData[toIndex[i]];
            }
            else
            {
                int targetIndex = toIndex[i] - 5;
                to[i] = enemyPartyData[targetIndex];
            }

        }


        BattleAction btAction;
        switch ( currentAction)
        {
            case BtAction.Attack:
                btAction = AttackAction;
                break;
            case BtAction.Skill:
                btAction = SkillAction;
                break;
            case BtAction.Item:
                btAction = ItemAction;
                break;
            case BtAction.Formation:
                btAction = AttackAction;
                break;
            case BtAction.Run:
                btAction = RunAction;
                break;
            default:
                btAction = AttackAction;
                break;
        }



        BattleActionData data = new BattleActionData(btAction, from, fromIndex, to, toIndex, curActionIndex, 1, priorityCount);

        return data;
    }

    void setCharacterAction(int charIndex, BattleActionData action)
    {
        characterBattleData[charIndex] = action;
    }

    public void RunQueuedAction()
    {
        foreach (var item in characterBattleData)
        {
            battleActionQueue.Add(item);
        }
        SetEnemyActionData();
        StartCoroutine(runQueuedAction());
    }

    IEnumerator runQueuedAction()
    {
        battleActionQueue.Sort(
            delegate (BattleActionData data1, BattleActionData data2)
            {
                return  data2.priority - data1.priority;
            }
        );

        foreach (BattleActionData item in battleActionQueue)
        {
            bool played = item.runAction();
            if (played)
                yield return new WaitForSeconds( item.actionDelay );
        }

        FinishTurn();
        CheckBattleisEnded();
        yield return null;
    }
    
    void AttackAction(GameCharacter[] from, int[] fromIndex, GameCharacter[] to, int[] toIndex, int actionIndex)
    {
        StartCoroutine(attackRoutine( from, fromIndex, to, toIndex, actionIndex));
    }

    IEnumerator attackRoutine(GameCharacter[] from, int[] fromIndex, GameCharacter[] to, int[] toIndex, int actionIndex)
    {
        foreach (var fromItem in from)
        {
            foreach (var toItem in to)
            {
                StatusData data = toItem.CurrentStat;
                data.VIT -= fromItem.CurrentStat.STR;
                toItem.CurrentStat = data;
            }
        }

        foreach (var item in fromIndex)
        {
            uiController.PortraitShake(item, true);
        }
        foreach (var item in toIndex)
        {
            uiController.showEffect(item, true);
        }

        yield return new WaitForSeconds(0.5f);
        refreshData();


        foreach (var item in fromIndex)
        {
            uiController.PortraitShake(item, false);
        }
        foreach (var item in toIndex)
        {
            uiController.showEffect(item, false);
        }
        yield return null;
    }

    void SkillAction(GameCharacter[] from, int[] fromIndex, GameCharacter[] to, int[] toIndex, int actionIndex)
    {
        StartCoroutine(skillRoutine(from, fromIndex, to, toIndex, actionIndex));
    }

    IEnumerator skillRoutine(GameCharacter[] from, int[] fromIndex, GameCharacter[] to, int[] toIndex, int actionIndex)
    {

        foreach (var fromItem in from)
        {
            foreach (var toItem in to)
            {
                StatusData data = toItem.CurrentStat;
                int damage = fromItem.CurrentStat.INT + DataManager.GetInstance().GetSkillData(actionIndex).power;
                data.VIT -= damage;
                toItem.CurrentStat = data;
            }
        }


        foreach (var item in fromIndex)
        {
            uiController.PortraitShake(item, true);
        }
        foreach (var item in toIndex)
        {
            uiController.showEffect(item, true, DataManager.GetInstance().GetSkillData(actionIndex).effectFile);
        }

        yield return new WaitForSeconds(0.5f);
        refreshData();
         

        foreach (var item in fromIndex)
        {
            uiController.PortraitShake(item, false);
        }
        foreach (var item in toIndex)
        {
            uiController.showEffect(item, false);
        }
        yield return null;
    }

    void ItemAction(GameCharacter[] from, int[] fromIndex, GameCharacter[] to, int[] toIndex, int actionIndex)
    {
        StartCoroutine(itemRoutine(from, fromIndex, to, toIndex, actionIndex));
    }

    IEnumerator itemRoutine(GameCharacter[] from, int[] fromIndex, GameCharacter[] to, int[] toIndex, int actionIndex)
    {

        foreach (var item in fromIndex)
        {
            uiController.PortraitShake(item, true);
        }
        foreach (var item in toIndex)
        {
            uiController.showEffect(item, true);
        }

        yield return new WaitForSeconds(0.5f);
        refreshData();


        foreach (var item in fromIndex)
        {
            uiController.PortraitShake(item, false);
        }
        foreach (var item in toIndex)
        {
            uiController.showEffect(item, false);
        }
        yield return null;
    }

    void RunAction(GameCharacter[] from, int[] fromIndex, GameCharacter[] to, int[] toIndex, int actionIndex)
    {
        StopAllCoroutines();
        ExitBattle();
    }







    int targetPicker( bool isUser)
    {
        if( isUser == true )
        {
            for (int i = 0; i < enemyPartyData.Length; i++)
            {
                if(isDead(i + 5) == false)
                {
                    return i + 5;
                }
            }
            return 5;
        }
        else
        {
            for (int i = 0; i < userPartyData.Length; i++)
            {
                if (isDead(i) == false)
                {
                    return i;
                }
            }
            return 0;
        }
    }

    void makeBattleAction()
    {
        int[] from = new int[1];
        from[0] = currentPlayerIndex;

        BattleActionData data = makeBattleActionData(from, targetIndices);

        setCharacterAction(currentPlayerIndex, data);
    }

    void SetEnemyActionData()
    {
        for (int i = 0; i < enemyPartyData.Length; i++)
        {
            int[] from = new int[1];
            from[0] = i+5;
            int[] to = new int[1];
            to[0] = targetPicker(false);
            BattleActionData data = makeBattleActionData(from, to);

            battleActionQueue.Add(data);
        }
    }

    void setAllBaseActions()
    {
        resetActionData();
        for (int i = 0; i < 5; i++)
        {
            int[] from = new int[1];
            from[0] = i;

            BattleActionData data = makeBattleActionData(from, targetIndices);

            setCharacterAction(i, data);

            uiController.SetActionText(i, "Attack");
        }
    }

    void FinishTurn()
    {
        battleActionQueue.Clear();
        setAllBaseActions();
        refreshData();
        uiController.reactiveOkButton();
    }


    void CheckBattleisEnded()
    {
        bool eleminated = true;
        foreach (var item in userPartyData)
        {
            eleminated = eleminated & (item.CurrentStat.VIT < 1);
        }
        if (eleminated == true)
        {
            StartCoroutine(gameOver());
        }

        bool allEnemyDead = true;
        foreach (var item in enemyPartyData)
        {
            allEnemyDead = allEnemyDead & (item.CurrentStat.VIT < 1);
        }
        if (allEnemyDead == true)
        {
            ExitBattle();
        }
    }


    public void EnterBattle(GameCharacter[] enemyGroupData)
    {
        enemyPartyData = null;
        enemyPartyData = enemyGroupData;
        Debug.Log("EnterBattle " + enemyPartyData.Length);
        StartCoroutine(StartBattle());
    }

    private IEnumerator StartBattle()
    {
        gameController.GetComponent<Ariadne.MoveController>().setMovable(false);
        fadeManager.FadeOut();
        var waitTime = new WaitForSeconds(fadeManager.fadeTime);
        yield return waitTime;


        for (int i = 0; i < enemyPartyData.Length; i++)
        {
            setEnemyData(i + 5, enemyPartyData[i]);
        }

        uiController.showBattleUI(true);

        fadeManager.FadeIn();
        yield return waitTime;

        FinishTurn();
        yield return null;
    }
    
    void ExitBattle() => StartCoroutine(FinishBattle());

    IEnumerator FinishBattle()
    {
        fadeManager.FadeOut();
        var waitTime = new WaitForSeconds(fadeManager.fadeTime);
        yield return waitTime;

        FinishTurn();
        uiController.showBattleUI(false);

        GetComponent<EncounterManager>().RefreshEncounter();

        fadeManager.FadeIn();
        yield return waitTime;

        gameController.GetComponent<Ariadne.MoveController>().setMovable(true);
        yield return null;
    }


    IEnumerator gameOver()
    {
        uiController.showBattleUI(false);
        fadeManager.FadeOut();
        yield return new WaitForSeconds(fadeManager.fadeTime);
        uiController.GameOver();
    }
       
    #endregion


    bool isDead( int index )
    {
        if(index < 5)
        {
            return userPartyData[index].CurrentStat.VIT < 1;
        }
        else
        {
            int targetIndex = index - 5;
            return enemyPartyData[targetIndex].CurrentStat.VIT < 1;
        }
    } 
}

