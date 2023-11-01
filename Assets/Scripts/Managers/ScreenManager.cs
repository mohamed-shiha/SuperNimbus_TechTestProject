using System;
using System.Linq;
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
    // will hold the screens gameobjects 
    [SerializeField] Screens Screens;
    // the main UI game object
    [SerializeField] Transform menusParent;
    // the player's tower select UI parent
    [SerializeField] Transform PlayerTowersParent;
    [SerializeField] Transform PlayOfflineButton;
    [SerializeField] TowerButton TowerSelectButtonPrefab;
    [SerializeField] TowerButton TowerUnlockButtonPrefab;
    [SerializeField] TextMeshProUGUI GoldText;
    [SerializeField] TextMeshProUGUI KillsText;
    [SerializeField] Animator animator;

    ScreenRef currentScreen;

    private void Start()
    {
        // start the game with the connection screen
        // make sure the main ui is active 
        menusParent.gameObject.SetActive(true);
        PlayOfflineButton.gameObject.SetActive(false);
        // show connection screen
        GoToScreen(ScreensTitles.LogIn);
        // subscribe to the connection events
        DataManager.Instance.OnDataReady += StartMainScreen;
        GameManager.Instance.OnPlayerRewarded += UpdatePlayerHud;
        GameManager.Instance.OnPlayerLivesChanged += UpdateLivesHud;

    }

    private void OnDestroy()
    {
        DataManager.Instance.OnDataReady -= StartMainScreen;
        GameManager.Instance.OnPlayerRewarded -= UpdatePlayerHud;
        GameManager.Instance.OnPlayerLivesChanged -= UpdateLivesHud;
    }

    private void UpdateLivesHud(int newLives)
    {
        // update the lives UI 
        //tent screen
        animator.SetTrigger("tentRed");
    }

    // when we are connected or playing offline move to the main menu
    public void StartMainScreen()
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
        if(currentScreen?.Transform != null)
        {
            currentScreen.Transform.gameObject.SetActive(false);
        }
        
        //preiviousScreen = currentScreen;
        currentScreen = Screens[screen];
    }

    public void StartGame()
    {
        var unlockedTowers = GameManager.Instance.GetPlayerUnlockedTowers();
        if (unlockedTowers.Length > PlayerTowersParent.childCount)
        {
            // spawn buttons
            foreach (SpawnData item in unlockedTowers.Skip(PlayerTowersParent.childCount))
            {
                Instantiate(TowerSelectButtonPrefab, PlayerTowersParent)
                    .SetData(item.ID, item.Value, item.GetIcon());
            }
        }
        // switch screens
        GoToScreen(ScreensTitles.Gameplay);
        // start the game 
        UpdatePlayerHud(GameManager.Instance.Data.GetGold(),0);
        GameManager.Instance.StartLevel();
    }

    Action UnPauseCallback;

    public void OnUnpauseAnimationEnd()
    {
        // tell the game the game is unpaused
        Time.timeScale = 1;
        // switch to screen 
        if(UnPauseCallback != null)
        {
            UnPauseCallback.Invoke();
            UnPauseCallback = null;
        }

        //GoToScreen(afterPauseScreen);
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
        UnPauseCallback = new Action(() => 
        {
            GoToScreen(ScreensTitles.Gameplay);
            Debug.Log("From Resume");
        });
    }

    public void OnRestart()
    {
        // tell the game to restart
        
        animator.SetTrigger("resume");
        UnPauseCallback = new Action(() =>
        {
            GameManager.Instance.OnRestart();
            //currentScreen = null;
            // switch screens
            GoToScreen(ScreensTitles.Gameplay);
            Debug.Log("From Restart");
        });

    }

    public void OnExit()
    {
        Application.Quit();
    }

    public void OnExitToMainMenu()
    {
        animator.SetTrigger("resume");
        UnPauseCallback = new Action(() => 
        {
            GameManager.Instance.OnBackToMain();
            GoToScreen(ScreensTitles.Main);
            Debug.Log("From Exit To Main");
        });
    }

    public void UpdatePlayerHud(int goldTotal, int killsTotal)
    {
        GoldText.text = goldTotal.ToString();
        KillsText.text = killsTotal.ToString();
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
