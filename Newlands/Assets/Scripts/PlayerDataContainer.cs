using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataContainer : MonoBehaviour
{
    private static string username;

    public static string Username { get { return username; } set { username = value; } }

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
