using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum ScreensTitles
{
    LogIn,
    Main,
    TowerUnlock,
    Levels,
    Gameplay,
    Pause
}

public class ScreenManager : MonoBehaviour
{
    [SerializeField] Screens Screens;
    [SerializeField] Transform menusParent;
    [SerializeField] Transform PlayerTowersParent;
    [SerializeField] TextMeshProUGUI UserNameInput;
    [SerializeField] TextMeshProUGUI PasswordInput;

    Transform currentScreen;
    Transform preiviousScreen;

    void Start()
    {
        DontDestroyOnLoad(this);
        menusParent.gameObject.SetActive(true);
        GoToScreen(ScreensTitles.LogIn);
    }

    public void TryLogin()
    {
        OnLoginOK();
    }

    public void OnLoginOK()
    {
        GoToScreen(ScreensTitles.Main);
    }

    public void GoToScreen(ScreensTitles screen)
    {
        Screens[screen].gameObject.SetActive(true);
        currentScreen?.gameObject.SetActive(false);
        preiviousScreen = currentScreen;
        currentScreen = Screens[screen];
    }

    public void StartGame()
    {
        menusParent.gameObject.SetActive(false);
        GoToScreen(ScreensTitles.Gameplay);
        GameManager.Instance.OnGameStarted.Invoke(new Level(new int[] { 7, 8, 9, 6, 7, 8, 9, 6, 7, 8, 9, 6, 7, 8, 9, 6, 7, 8, 9, 6, 7, 8, 9, 6 }) { SpawnSpeed = 3.5f });
    }

    public void OnBack()
    {
        if(preiviousScreen == currentScreen)
        {
            return;
        }

        preiviousScreen.gameObject.SetActive(true);
        currentScreen.gameObject.SetActive(false);
        currentScreen = preiviousScreen;
    }
}

[Serializable]
public class Screens
{
    [SerializeField] Transform[] allScreens;

    public Screens()
    {
        allScreens = new Transform[Enum.GetValues(typeof(ScreensTitles)).Length];
    }

    public Transform this [ScreensTitles screenTitle]
    {
        get
        {
            return allScreens[(int)screenTitle];
        }

        set
        {
            allScreens[(int)screenTitle] = value;
        }
    }

  
}
