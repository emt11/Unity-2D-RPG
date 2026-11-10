using System.Collections;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

// 管理不同关卡
// 1. 在不同关卡之间跳转
public class GameManager : MonoBehaviour, ISaveable
{
    public static GameManager instance;
    private Vector3 lastPlayerPosition;
    private string lastScenePlayed;
    private bool dataLoaded;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(instance);
    }

    public void RestartScene()
    {
        SaveManager.instance.SaveGame();
        string sceneName = SceneManager.GetActiveScene().name;
        ChangeScene(sceneName, RespawnType.NonSpecific);
    }

    public void ContinuePlay()
    {
        string sceneName = string.IsNullOrWhiteSpace(lastScenePlayed)
            ? "Level_0"
            : lastScenePlayed;
        ChangeScene(sceneName, RespawnType.NonSpecific);
    }

    public void ChangeScene(string sceneName, RespawnType type)
    {
        SaveManager.instance.SaveGame();
        Time.timeScale = 1; // 接触 options 造成的暂停
        StartCoroutine(ChangeScnenCo(sceneName, type));
    }

    private IEnumerator ChangeScnenCo(string sceneName, RespawnType type)
    {
        UI_FadeScreen fadeScreenUI = GetFadeScreenUI();
        // 先黑屏
        fadeScreenUI.FadeOut();
        yield return fadeScreenUI.fadeEffectCo; // 等待黑屏效果结束

        SceneManager.LoadScene(sceneName);
        // 由于 SaveManager.instance.SaveGame(); 和 该函数是同一帧内执行的，所有需要先等待一帧保证 dataLoaded = false
        dataLoaded = false;
        yield return null;
        // 等待加载完毕之后再变为透明
        while (!dataLoaded)
        {
            yield return null;
        }
        // 加载新场景后原 fadeScreenUI 会失效，需要重新获取
        fadeScreenUI = GetFadeScreenUI();
        fadeScreenUI.FadeIn();

        // 传送到 MainMenu 不能获取到 player，需要判空
        Player player = Player.instance;
        if (player == null)
            yield break;

        Vector3 pos = GetNewPlayerPosition(type);
        if (pos != Vector3.zero)
            player.TeleportPosition(pos);
    }

    private Vector3 GetNewPlayerPosition(RespawnType type)
    {
        if (type == RespawnType.NonSpecific)
        {
            // 寻找离玩家死亡位置最近的 Enter waypoint 或者是 已激活的 checkpoint
            GameData data = SaveManager.instance.GetGameData();
            Object_Checkpoint[] checkpoints = FindObjectsByType<Object_Checkpoint>();
            List<Vector3> unlockedCheckpointsPositions = 
                            checkpoints.Where(cp => data.unlockedCheckpoints.TryGetValue(cp.GetCheckpointId(), out bool unlocked) && unlocked)
                            .Select(cp => cp.GetPosition())
                            .ToList();
            
            Object_Waypoint[] enterWaypoints = FindObjectsByType<Object_Waypoint>();
            List<Vector3> enterWaypointsPositions = enterWaypoints.Where(wp => wp.GetRespawnType() == RespawnType.Enter)
                            .Select(wp => wp.GetPositionAndSetTriggerFalse())
                            .ToList();

            List<Vector3> selectedPositions = unlockedCheckpointsPositions.Concat(enterWaypointsPositions).ToList();
            if (selectedPositions.Count == 0) return Vector3.zero;

            return selectedPositions.OrderBy(position => Vector3.Distance(position, lastPlayerPosition)).First();
        }

        return GetWaypointPosition(type);
    }

    // 拿到待传送关卡的对应 Waypoint 位置
    private Vector3 GetWaypointPosition(RespawnType type)
    {
        var waypoints = FindObjectsByType<Object_Waypoint>();
        foreach (var point in waypoints) {
            if (point.GetRespawnType() == type)
            {
                point.SetCanBeTriggered(false);
                return point.GetPositionAndSetTriggerFalse();
            }
        }

        return Vector3.zero;
    }

    public void SaveData(ref GameData gameData)
    {
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "MainMenu") return;

        gameData.lastScenePlayed = currentScene;
        gameData.lastPlayerPosition = Player.instance.transform.position; // 记录上一次的玩家位置以寻找最近的 waypoint or checkpoint
        dataLoaded = false;
    }

    public void LoadData(GameData gameData)
    {
        lastScenePlayed = gameData.lastScenePlayed;
        lastPlayerPosition = gameData.lastPlayerPosition;
        if (string.IsNullOrEmpty(lastScenePlayed))
        {
            // 第一次游玩
            lastScenePlayed = "Level_0";
        }
        dataLoaded = true;
    }

    public UI_FadeScreen GetFadeScreenUI()
    {
        if (UI.instance != null)
            return UI.instance.fadeScreenUI;
        else 
            return FindAnyObjectByType<UI_FadeScreen>();
    }
}
