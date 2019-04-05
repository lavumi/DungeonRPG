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
    public CharacterBase(string img, string name, int level, StatusData stat)
    {
        this.name = name;
        this.imgFile = img;
        this.level = level;
        this.state = stat;
        this.curStat = stat;
    }


    public string Name
    {
        protected set { name = value; }
        get{ return name; }
    }

    public int Level
    {
        protected set { level = value; }
        get { return level; }
    }

    public StatusData State
    {
        protected set { state = value; }
        get { return state; }
    }

    public string imgFile
    {
        protected set { imageFile = value; }
        get { return imageFile; }
    }

    public StatusData CurrentState
    {
        set { curStat = value; }
        get { return curStat; }
    }

    string name;
    int level;
    StatusData state;
    StatusData curStat;
    string imageFile;

}




