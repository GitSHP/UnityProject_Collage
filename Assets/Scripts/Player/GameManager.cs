using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI currentEnemyCountUI, survivalTimeUI, killCountUI, levelUI, startGameUI;
    public GameObject restartButton, quitButton, resumeButton, gameOverPanel, gameClearPanel, enemyCommingPanel, startGamePanel, bossAlertPanel, clearPanel;
    // TextMeshPro 타입의 UI 화면이 띄우는 법 - GameObject 패널을 만들고 UI text를 그 자식으로 두고 패널을 SetActive로 활성화 - 제미나이
    public float currentTime = 0;
    public int currentMin;
    public int killCount = 0;
    public static GameManager instance;
    bool isGameClear = false;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(StartGame());
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        currentMin = (int)currentTime / 60;
        CheckCurrentEnemy();
        CheckSurvivalTime();
        CheckKillCount();
        CheckLevel();
    }

    void CheckCurrentEnemy()
    {
        currentEnemyCountUI.text = "현재 적 : " + EnemyGenerator.instance.currentEnemyCount;
    }

    void CheckSurvivalTime()
    {
        survivalTimeUI.text = "생존 시간 : " + currentMin + "분 " + (int)currentTime % 60 + "초"; 
    }
    void CheckKillCount()
    {
        killCountUI.text = "처치한 적 : " + killCount;   
    }

    void CheckLevel()
    {
        levelUI.text = "Level " + PlayerManager.instance.level;
    }

    public void EndGame()
    {
        Time.timeScale = 0;
        restartButton.SetActive(true);
        quitButton.SetActive(true);
        gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("GameScene");
        Time.timeScale = 1;
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void PressPause()
    {
        Time.timeScale = 0;
        resumeButton.SetActive(true);
        restartButton.SetActive(true);
        quitButton.SetActive(true);
    }

    public void PressResume()
    {
        Time.timeScale = 1;
        resumeButton.SetActive(false);
        restartButton.SetActive(false);
        quitButton.SetActive(false);
    }

    public void ShowEnemyCommingPanel()
    {
        StartCoroutine(EnemyComming());
    }

    public void ClearGame()
    {
        isGameClear = true;

        clearPanel.SetActive(true);
        restartButton.SetActive(true);
        quitButton.SetActive(true);
        
        Time.timeScale = 0;
    }

    private IEnumerator EnemyComming()
    {
        enemyCommingPanel.SetActive(true);
        yield return new WaitForSeconds(1f);
        enemyCommingPanel.SetActive(false);
        yield return new WaitForSeconds(1f);
        enemyCommingPanel.SetActive(true);
        yield return new WaitForSeconds(1f);
        enemyCommingPanel.SetActive(false);
        yield return new WaitForSeconds(1f);
        enemyCommingPanel.SetActive(true);
        yield return new WaitForSeconds(1f);
        enemyCommingPanel.SetActive(false);
    }

    private IEnumerator StartGame()
    {
        Time.timeScale = 0; 
        // 코루틴의 WaitForSeconds는 게임 내 시간에 영향을 받는다.
        // 따라서 게임 내 시간이 멈춘 상태에서 카운트다운하기 위해서는 실제 세계의 시간을 사용하는 WaitForSecondsRealtime 사용한다
        startGamePanel.SetActive(true);
        yield return new WaitForSecondsRealtime(1f); 
        startGameUI.text = "3";
        yield return new WaitForSecondsRealtime(1f);    
        startGameUI.text = "2";
        yield return new WaitForSecondsRealtime(1f);
        startGameUI.text = "1";
        yield return new WaitForSecondsRealtime(1f);
        startGameUI.text = "Survive !";
        yield return new WaitForSecondsRealtime(1f);
        startGamePanel.SetActive(false);
        Time.timeScale = 1;
    }

    public IEnumerator BossAlert()
    {
        bossAlertPanel.SetActive(true);
        yield return new WaitForSecondsRealtime(1f);
        bossAlertPanel.SetActive(false);
        yield return new WaitForSecondsRealtime(1f);
        bossAlertPanel.SetActive(true);
        yield return new WaitForSecondsRealtime(1f);
        bossAlertPanel.SetActive(false);
    }
}
