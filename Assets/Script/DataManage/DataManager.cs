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
    
    string baseResourcePath = "GameData/";

    void InitData()
    {
        LoadPlayerData();
        LoadEnemyData();
    }

    void LoadEnemyData()
    {
        List<Dictionary<string, object>> data = csvReader.Read(baseResourcePath + "DRPG_DATA_SHEET - Enemy");

        enemyData = new CharacterBase[data.Count];

        for (var i = 0; i < data.Count; i++)
        {
            StatusData stat = new StatusData();
            stat.SetData(
                (int)data[i][StatusData.memberList[0]],
                (int)data[i][StatusData.memberList[1]],
                (int)data[i][StatusData.memberList[2]],
                (int)data[i][StatusData.memberList[3]]
                );

            enemyData[i] = new CharacterBase((string)data[i]["imgFile"], (string)data[i]["charName"], stat);
        }
    }

    void LoadPlayerData()
    {
        List<Dictionary<string, object>> data = csvReader.Read(baseResourcePath + "DRPG_DATA_SHEET - Chara");
        playerData = new CharacterBase[data.Count];
        Debug.Log("LoadPlayerData" + data.Count);
        for (var i = 0; i < data.Count; i++)
        {
            StatusData stat = new StatusData();
            stat.SetData(
                (int)data[i][StatusData.memberList[0]],
                (int)data[i][StatusData.memberList[1]],
                (int)data[i][StatusData.memberList[2]],
                (int)data[i][StatusData.memberList[3]]
                );

            playerData[i] = new CharacterBase((string)data[i]["imgFile"], (string)data[i]["charName"], stat);
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
}
