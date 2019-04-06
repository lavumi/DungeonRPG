using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//캐릭터 데이터를 읽고 제공해 주는 클래스
public class DataManager : MonoBehaviour
{

    private static DataManager instance = null;        
    void Awake()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
            Destroy(gameObject);



        DontDestroyOnLoad(gameObject);
        InitData();
    }

    public static DataManager GetInstance()
    {
        return instance;
    }


    CharacterBase[] enemyData;
    CharacterBase[] playerData;
    SkillBase[] skillData;
    
    string baseResourcePath = "GameData/DRPG_DATA_SHEET - ";

    void InitData()
    {
        LoadSkillData();

        LoadPlayerData();
        LoadEnemyData();

    }

    void LoadEnemyData()
    {
        List<Dictionary<string, object>> data = csvReader.Read(baseResourcePath + "Enemy");

        enemyData = new CharacterBase[data.Count];

        for (var i = 0; i < data.Count; i++)
        {
            StatusData stat = new StatusData();
            stat.SetData(
                (int)data[i][StatusData.memberList[0]],
                (int)data[i][StatusData.memberList[1]],
                (int)data[i][StatusData.memberList[2]],
                (int)data[i][StatusData.memberList[3]],
                (int)data[i][StatusData.memberList[4]],
                (int)data[i][StatusData.memberList[5]]
                );

            enemyData[i] = new CharacterBase((string)data[i]["imgFile"], (string)data[i]["charName"], stat, (int)data[i]["skillData"]);
        }
    }

    void LoadPlayerData()
    {
        List<Dictionary<string, object>> data = csvReader.Read(baseResourcePath + "Chara");
        playerData = new CharacterBase[data.Count];
        for (var i = 0; i < data.Count; i++)
        {
            StatusData stat = new StatusData();
            stat.SetData(
                (int)data[i][StatusData.memberList[0]],
                (int)data[i][StatusData.memberList[1]],
                (int)data[i][StatusData.memberList[2]],
                (int)data[i][StatusData.memberList[3]],
                (int)data[i][StatusData.memberList[4]],
                (int)data[i][StatusData.memberList[5]]
                );

            playerData[i] = new CharacterBase((string)data[i]["imgFile"], (string)data[i]["charName"], stat,(int)data[i]["skillData"]);
        }
    }

    void LoadSkillData()
    {
        List<Dictionary<string, object>> data = csvReader.Read(baseResourcePath + "Skill");
        skillData = new SkillBase[data.Count];
        for (var i = 0; i < data.Count; i++)
        {
            Debug.Log((string)data[i]["effectFile"] + "//" + Resources.Load<Sprite>((string)data[i]["effectFile"]));
            skillData[i] = new SkillBase(
                (int)data[i]["index"],
                Resources.Load<Sprite>((string)data[i]["effectFile"]),
                (string)data[i]["name"],
                (TargetType)data[i]["targetType"],
                (ElementType)data[i]["elementType"],
                (int)data[i]["power"],
                (int)data[i]["priority"]
                );
        }
    }
        

    public GameCharacter[] GetMonsterGroup()
    {
        int monsterCount = Random.Range(1, 6);
        GameCharacter[] enemyGroupData = new GameCharacter[monsterCount];

        for (int i = 0; i < monsterCount; i++)
        {
            int rndMon = Random.Range(0, enemyData.Length);
            enemyGroupData[i] = new GameCharacter(enemyData[rndMon], 1, 0 );
            ;
        }
        return enemyGroupData;
    }

    public GameCharacter[] GetTestPlayerParty()
    {
        GameCharacter[] party = new GameCharacter[5];

        for (int i = 0; i < 5; i++)
        {
            party[i] = new GameCharacter(playerData[i], 1, 0);
        }
        return party;
    }

    public SkillBase GetSkillData(int index)
    {
        return skillData[index];
    }
}
