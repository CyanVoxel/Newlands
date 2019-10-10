using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardUtility
{
    // Returns a formatted object name based on type and coordinate info
    public static string CreateCardObjectName(string type, int x, int y)
    {
        string xZeroes = "0";
        string yZeroes = "0";
        char xChar = 'x';
        char yChar = 'y';

        if (type == "GameCard")
        {
            xChar = 'p';
            yChar = 'i';
        }

        if (x >= 10)
            xZeroes = "";
        else
            xZeroes = "0";

        if (y >= 10)
            yZeroes = "";
        else
            yZeroes = "0";

        return (xChar + xZeroes + x + "_" + yChar + yZeroes + y + "_" + type);
    }
}
