using System.Collections;
using System.Collections.Generic;



public class TurnManager 
{
 
    private static TurnManager instance = null;

    public static TurnManager getInstance()
    {
        if( instance == null)
        {
            instance = new TurnManager();
        }

        return instance;
    }



    private int turncounter;
    private TurnManager()
    {
        turncounter = 0;
    }

    public void addTurnCount()
    {
        turncounter++;
    }

    public void resetTurnCount()
    {
        turncounter = 0;
    }


}
