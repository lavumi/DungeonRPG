using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//캐릭터 데이터를 읽고 제공해 주는 클래스
public class CharacterDataManager : MonoBehaviour
{

    private static CharacterDataManager instance = null;        
    void Awake()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public static CharacterDataManager GetInstance()
    {
        return instance;
    }


    CharacterBase[] enemyData;
    CharacterBase[] playerData;
    
    string baseResourcePath = "CharacterData/";


    // Start is called before the first frame update
    void Start()
    {
        LoadEnemyData();
        LoadPlayerData();
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

            enemyData[i] = new CharacterBase((string)data[i]["imgFile"], (string)data[i]["CharName"], 1, stat);
        }
    }


    void LoadPlayerData()
    {
        List<Dictionary<string, object>> data = csvReader.Read(baseResourcePath + "DRPG_DATA_SHEET - Chara");
        playerData = new CharacterBase[data.Count];

        for (var i = 0; i < data.Count; i++)
        {
            StatusData stat = new StatusData();
            stat.SetData(
                (int)data[i][StatusData.memberList[0]],
                (int)data[i][StatusData.memberList[1]],
                (int)data[i][StatusData.memberList[2]],
                (int)data[i][StatusData.memberList[3]]
                );

            playerData[i] = new CharacterBase((string)data[i]["imgFile"], (string)data[i]["CharName"], 1, stat);
        }
    }



    public CharacterBase[] GetMonsterGroup()
    {
        int monsterCount = Random.Range(1, 6);
        CharacterBase[] enemyGroupData = new CharacterBase[monsterCount];

        for (int i = 0; i < monsterCount; i++)
        {
            int rndMon = Random.Range(0, enemyData.Length);
            enemyGroupData[i] = new CharacterBase(enemyData[rndMon].imgFile, enemyData[rndMon].Name, enemyData[rndMon].Level, enemyData[rndMon].State);
            ;
        }
        return enemyGroupData;
    }

    public CharacterBase[] GetTestPlayerParty()
    {
        CharacterBase[] party = new CharacterBase[5];

        for (int i = 0; i < 5; i++)
        {
            party[i] = new CharacterBase(playerData[i].imgFile, playerData[i].Name, playerData[i].Level, playerData[i].State);
        }
        return party;
    }
}
