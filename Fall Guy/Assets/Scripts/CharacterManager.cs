﻿using UnityEngine;
using System.Collections;
using GamePolygon;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance;

    public static readonly string CURRENT_CHARACTER_KEY = "CURRENT_CHARACTER";

    public int CurrentCharacterIndex
    {
        get
        {
            return PlayerPrefs.GetInt(CURRENT_CHARACTER_KEY, 0);
        }
        set
        {
            PlayerPrefs.SetInt(CURRENT_CHARACTER_KEY, value);
            PlayerPrefs.Save();
        }
    }

    public GameObject[] characters;

    void Awake()
    {
        if (Instance)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    public void AddCoins()
    {
        CoinManager.Instance.AddCoins(100);
    }
}

