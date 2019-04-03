using System.Collections;
using System.Collections.Generic;

public struct StatusData
{
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
    }


    public string Name
    {
        set { name = value; }
        get{ return name; }
    }

    public int Level
    {
         set { level = value; }
        get { return level; }
    }

    public StatusData State
    {
         set { state = value; }
        get { return state; }
    }

    public string imgFile
    {
        set { imageFile = value; }
        get { return imageFile; }
    }


    string name;
    int level;
    StatusData state;
    string imageFile;

}




