using System;
using System.Linq;
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
    // will hold the screens gameobjects 
    [SerializeField] Screens Screens;
    // the main UI game object
    [SerializeField] Transform menusParent;
    // the player's tower select UI parent
    [SerializeField] Transform PlayerTowersParent;
    [SerializeField] TowerButton TowerSelectButtonPrefab;
    [SerializeField] TowerButton TowerUnlockButtonPrefab;
    [SerializeField] Animator animator;

    ScreenRef currentScreen;
    ScreenRef preiviousScreen;
    ScreensTitles afterPauseScreen;

    void Start()
    {
        // start the game with the connection screen
        // make sure the main ui is active 
        menusParent.gameObject.SetActive(true);
        // show connection screen
        GoToScreen(ScreensTitles.LogIn);
        // subscribe to the connection events
        ConnectionManager.Instance.OnConnected += OnLoginOK;
        ConnectionManager.Instance.OnStartOffLine += OnLoginOK;
    }

    private void OnDestroy()
    {
        ConnectionManager.Instance.OnConnected -= OnLoginOK;
        ConnectionManager.Instance.OnStartOffLine -= OnLoginOK;
    }

    // when we are connected or playing offline move to the main menu
    public void OnLoginOK()
    {
        GoToScreen(ScreensTitles.Main);
    }

    // a help method to switch between screens
    public void GoToScreen(ScreensTitles screen)
    {
        if (Screens == null)
        {
            return;
        }

        switch (screen)
        {
            case ScreensTitles.LogIn:
                break;
            case ScreensTitles.Main:
                Time.timeScale = 1;
                menusParent.gameObject.SetActive(true);
                break;
            case ScreensTitles.TowerUnlock:
                break;
            case ScreensTitles.Levels:
                break;
            case ScreensTitles.Gameplay:
                menusParent.gameObject.SetActive(false);
                break;
            case ScreensTitles.Pause:
                break;
        }

        Screens[screen].Transform.gameObject.SetActive(true);
        currentScreen?.Transform.gameObject.SetActive(false);
        preiviousScreen = currentScreen;
        currentScreen = Screens[screen];
    }

    public void StartGame()
    {
        var unlockedTowers = GameManager.Instance.GetPlayerUnlockedTowers();
        if (unlockedTowers.Length > PlayerTowersParent.childCount)
        {
            // spawn buttons
            foreach (SpawnData item in GameManager.Instance.GetPlayerUnlockedTowers())
            {
                Instantiate(TowerSelectButtonPrefab, PlayerTowersParent).SetData(item.ID, item.Value, item.Icon);
            }
        }
        // switch screens
        GoToScreen(ScreensTitles.Gameplay);
        // start the game 
        // TODO: need to make sure a level is selected for now use a debug level
        GameManager.Instance.StartLevel(null);
    }

    public void OnUnpauseAnimationEnd()
    {
        // tell the game the game is unpaused
        Time.timeScale = 1;
        // switch to screen 
        GoToScreen(afterPauseScreen);
    }

    public void OnPauseGame()
    {
        Time.timeScale = 0;
        GoToScreen(ScreensTitles.Pause);
        animator.SetTrigger("pause");
    }

    public void OnResumeGame()
    {
        animator.SetTrigger("resume");
        afterPauseScreen = ScreensTitles.Gameplay;
    }

    public void OnRestart()
    {
        // tell the game to restart
        OnResumeGame();
    }

    public void OnExit()
    {
        Application.Quit();
    }

    public void OnExitToMainMenu()
    {
        animator.SetTrigger("resume");
        afterPauseScreen = ScreensTitles.Main;
    }
}

[Serializable]
public class ScreenRef
{
    public Transform Transform;
    public ScreensTitles Title;
}

[Serializable]
public class Screens
{
    [SerializeField] ScreenRef[] allScreens;

    public ScreenRef this[ScreensTitles screenTitle]
    {
        get
        {
            return allScreens.FirstOrDefault(s => s.Title == screenTitle);
        }
    }
}
