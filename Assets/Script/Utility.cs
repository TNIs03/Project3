using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static int GetRandomIndex(List<float> cntList)
    {
        float sum = 0, divider = 100;
        int n = cntList.Count;
        string log = "Prob: ";
        for (int i = 0; i < n; i++)
        {
            if (cntList[i] > 0)
            {
                cntList[i] = divider / cntList[i];
                sum += cntList[i];
            }
            log += cntList[i];
            log += ' ';
        }
        float rand = Random.Range(0.0f, sum);
        for (int i = 0; i < n; i++)
        {
            if (rand <= cntList[i])
            {
                log += "\nChosen: " + i;
                Debug.Log(log);
                return i;
            }
            rand -= cntList[i];
        }
        Debug.Log("Something is wrong");
        return -1;
    }
}