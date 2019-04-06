using System;
using System.Collections;
using System.Collections.Generic;

public struct StatusData
{
    public static string[] memberList = {
        "str",
        "def",
        "HP",
        "speed"
        };


    public int str;
    public int def;
    public int HP;
    public int speed;


    public void SetData(int str, int def, int HP, int speed)
    {
        this.str = str;
        this.def = def;
        this.HP = HP;
        this.speed = speed;
    }
}

public class CharacterBase
{
    public readonly string imgFile;
    public readonly string Name;
    public readonly StatusData Stat;

    public CharacterBase(string imgFile, string name, StatusData stat)
    {
        this.imgFile = imgFile ?? throw new ArgumentNullException(nameof(imgFile));
        this.Name = name ?? throw new ArgumentNullException(nameof(name));
        this.Stat = stat;
    }
}


public class GameCharacter 
{
    public readonly CharacterBase baseInfo;
    public  StatusData CurrentStat;
    public  int Level;
    public  int Exp;

    public GameCharacter(CharacterBase baseInfo,  int level, int exp)
    {
        this.baseInfo = baseInfo ?? throw new ArgumentNullException(nameof(baseInfo));
        this.CurrentStat = baseInfo.Stat;
        this.Level = level;
        this.Exp = exp;
    }

}




