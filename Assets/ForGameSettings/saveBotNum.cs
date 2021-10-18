using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class saveBotNum : MonoBehaviour
{
    public int botNum;
    public static saveBotNum instance;


    void Awake()
    {
        botNum = 1;
        if (instance == null)
            instance = this;
        else
        { Destroy(gameObject); };

        DontDestroyOnLoad(gameObject);

    }

    public void setBotNum(int num)
    {
        botNum = num;
    }
}
