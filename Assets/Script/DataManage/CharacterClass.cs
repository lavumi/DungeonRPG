using System;
using System.Collections;
using System.Collections.Generic;

public struct StatusData
{
    public static string[] memberList = {
        "STR",
        "INT",
        "PIE",
        "AGI",
        "VIT",
        "LUC"
    };

    public int STR;
    public int INT;
    public int PIE;
    public int AGI;
    public int VIT;
    public int LUC;

    public void SetData(int sTR, int iNT, int pIE, int aGI, int vIT, int lUC)
    {
        STR = sTR;
        INT = iNT;
        PIE = pIE;
        AGI = aGI;
        VIT = vIT;
        LUC = lUC;
    }
}

public class CharacterBase
{
    public readonly string imgFile;
    public readonly string Name;
    public readonly StatusData Stat;
    public readonly int skill;

    public CharacterBase(string imgFile, string name, StatusData stat, int skill)
    {
        this.imgFile = imgFile ?? throw new ArgumentNullException(nameof(imgFile));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Stat = stat;
        this.skill = skill;
    }
}

public class GameCharacter 
{
    public readonly CharacterBase baseInfo;
    public StatusData CurrentStat;
    public int skill;
    public int Level;
    public int Exp;

    public GameCharacter(CharacterBase baseInfo,  int level, int exp)
    {
        this.baseInfo = baseInfo ?? throw new ArgumentNullException(nameof(baseInfo));
        this.CurrentStat = baseInfo.Stat;
        this.skill = baseInfo.skill;
        this.Level = level;
        this.Exp = exp;
    }

}




