using Satori;
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
    Pause,
    WinLose
}

public class ScreenManager : MonoBehaviour
{
    // will hold the screens gameobjects 
    [SerializeField] Screens Screens;
    // the main UI game object
    [SerializeField] Transform menusParent;
    //Tower Unlock 
    [SerializeField] Transform TowersUnlockContentParent;
    [SerializeField] ConfirmPanel confirmScreen;
    // the player's tower select UI parent
    [SerializeField] Transform PlayerTowersParent;
    [SerializeField] Transform PlayOfflineButton;
    [SerializeField] TowerButton TowerSelectButtonPrefab;
    [SerializeField] TowerUnlockItem TowerUnlockButtonPrefab;
    [SerializeField] TextMeshProUGUI GoldText;
    [SerializeField] TextMeshProUGUI KillsText;
    [SerializeField] Animator animator;

    Action UnPauseCallback;
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
        GameManager.Instance.OnLevelEnded += ShowWinLoseScreen;

    }

    private void OnDestroy()
    {
        DataManager.Instance.OnDataReady -= StartMainScreen;
        GameManager.Instance.OnPlayerRewarded -= UpdatePlayerHud;
        GameManager.Instance.OnPlayerLivesChanged -= UpdateLivesHud;
        GameManager.Instance.OnLevelEnded -= ShowWinLoseScreen;
    }

    private void ShowWinLoseScreen(bool isWin)
    {
        string winLoseText = isWin ? "You Win" : "Try again";
        Screens[ScreensTitles.WinLose]
            .Transform
            .GetComponent<WinLoseScreen>()
            .UpdateResult(winLoseText, new string[] { GoldText.text, KillsText.text });
        Screens[ScreensTitles.WinLose]
            .Transform.gameObject.SetActive(true);
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

    public void BackToMain()
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
                Screens[ScreensTitles.WinLose].Transform.gameObject.SetActive(false);
                break;
            case ScreensTitles.TowerUnlock:
                if (TowersUnlockContentParent.childCount == 0)
                {
                    // all the towers
                    var data = DataManager.Instance.GetGameData(DataSource.Debug).Towers;
                    // players towers
                    var playerData = GameManager.Instance.GetPlayerUnlockedTowers();
                    //spawn a button for each tower in debug data
                    foreach (var tower in data)
                    {
                        var item = Instantiate(TowerUnlockButtonPrefab, TowersUnlockContentParent);
                        item.SetData(
                            tower.ID
                            , playerData.Any(i => i.ID == tower.ID)
                            , (id) => TryUnlockTower(id, () => item.UpdateUnlocked(true))
                            , DataManager.Instance.GetCameraTexture(tower.ID)
                            );
                    }
                }
                break;
            case ScreensTitles.Levels:
                break;
            case ScreensTitles.Gameplay:
                menusParent.gameObject.SetActive(false);
                Screens[ScreensTitles.WinLose].Transform.gameObject.SetActive(false);
                break;
            case ScreensTitles.Pause:
                break;
        }

        Screens[screen].Transform.gameObject.SetActive(true);
        if (currentScreen?.Transform != null && currentScreen.Title != screen)
        {
            currentScreen.Transform.gameObject.SetActive(false);
        }

        //preiviousScreen = currentScreen;
        currentScreen = Screens[screen];
    }

    public void GoToUnlockScreen() => GoToScreen(ScreensTitles.TowerUnlock);

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
        UpdatePlayerHud(GameManager.Instance.Data.GetGold(), 0);
        GameManager.Instance.StartLevel();
    }

    public void OnUnpauseAnimationEnd()
    {
        // tell the game the game is unpaused
        Time.timeScale = 1;
        // switch to screen 
        if (UnPauseCallback != null)
        {
            UnPauseCallback.Invoke();
            UnPauseCallback = null;
        }
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

    public void OnRestart(bool playAnimation)
    {
        if (playAnimation)
        {
            animator.SetTrigger("resume");

            UnPauseCallback = new Action(() =>
            {
                GameManager.Instance.OnRestart();
                // switch screens
                GoToScreen(ScreensTitles.Gameplay);
                Debug.Log("From Restart");
            });
        }
        else
        {
            GameManager.Instance.OnRestart();
            // switch screens
            GoToScreen(ScreensTitles.Gameplay);
            Debug.Log("From Restart");
        }

    }

    public void OnExit()
    {
        Application.Quit();
    }

    public void OnExitToMainMenu(bool playAnimation)
    {
        if (playAnimation)
        {
            animator.SetTrigger("resume");
            UnPauseCallback = new Action(() =>
            {
                GameManager.Instance.OnBackToMain();
                GoToScreen(ScreensTitles.Main);
                Debug.Log("From Exit To Main");
            });
        }
        else
        {
            GameManager.Instance.OnBackToMain();
            GoToScreen(ScreensTitles.Main);
            Debug.Log("From Exit To Main");
        }
    }

    public void UpdatePlayerHud(int goldTotal, int killsTotal)
    {
        GoldText.text = goldTotal.ToString();
        KillsText.text = killsTotal.ToString();
    }

    public void TryUnlockTower(int id, Action callBack)
    {
        if (GameManager.Instance.CanPurchase(id))
        {
            confirmScreen.Show(() =>
            {
                //ConnectionManager.Instance.UnlockTowerForPlayer(new int[] { id }).Wait();
                GameManager.Instance.UnlockTowerForPlayer(id);
            }, callBack);
        }
        else
        {
            // Show Get more Gold 
        }
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
