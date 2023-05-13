using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // static -> 정적으로 사용하겠다. 바로 메모리에 얹음
    public static GameManager instance;

    [Header("# Game Control")]
    public bool isLive;
    public float gameTime;
    public float maxGameTime = 2 * 10f; //20초 5분은 5 * 60
    [Header("# Player Info")]
    public int playerId; // 캐릭터 선택창에서 ID선언
    public float health;
    public float maxHealth = 100;
    public int level;
    public int kill;
    public int exp;
    public int[] nextExp = { 3, 5, 10, 100, 150, 210, 280, 360, 450, 600 };
    [Header("# GameObject")]
    public PoolManager pool;
    public Player player;
    public LevelUp uiLevelUp;
    public Result uiResult;
    public Transform uiJoy;
    public GameObject enemyCleaner;

    private void Awake()
    {
        instance = this; // 초기화
        Application.targetFrameRate = 60;
    }

    public void GameStart(int id)
    {
        playerId = id;
        health = maxHealth; // 게임 시작 하자마자 풀피

        player.gameObject.SetActive(true);
        uiLevelUp.Select(playerId % 2); // 기존무기 지급을 위한 함수호출에서 인자 값을 캐릭터ID로 변경
        //uiLevelUp.Select(0); // 임시 스크립트 (첫번째 캐릭터 선택)
        Resume();

        AudioManager.instance.PlayBgm(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    public void GameOver()
    {
        // 비석이 나오고 정지가 되어야 하므로 코루틴을 이용
        StartCoroutine(GameOverRoutine());
    }

    IEnumerator GameOverRoutine()
    {
        isLive = false;

        yield return new WaitForSeconds(0.5f);

        uiResult.gameObject.SetActive(true);
        uiResult.Lose();
        Stop();

        AudioManager.instance.PlayBgm(false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Lose);
    }

    public void GameVictory()
    {
        // 비석이 나오고 정지가 되어야 하므로 코루틴을 이용
        StartCoroutine(GameVictoryRoutine());
    }

    IEnumerator GameVictoryRoutine()
    {
        isLive = false;
        enemyCleaner.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        uiResult.gameObject.SetActive(true);
        uiResult.Win();
        Stop();

        AudioManager.instance.PlayBgm(false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Win);
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(0);
    }

    public void GameQuit()
    {
        Application.Quit();
    }

    private void Update()
    {
        if (!isLive) // 각 스크립트의 Updata 문 마다 넣어줌
            return;

        gameTime += Time.deltaTime;

        if (gameTime > maxGameTime)
        {
            gameTime = maxGameTime;
            GameVictory(); // 게임시간이 최대시간을 넘긴다면
        }
    }

    public void GetExp()
    {
        if (!isLive)
            return;

        exp++;
        
        if (exp == nextExp[Mathf.Min(level, nextExp.Length-1)]) // 필요한 경험치에 도달하면 레벨업 ++ 만렙을 찍어도 최고 경험치를 계속 사용
        {
            level++;
            exp = 0;
            uiLevelUp.Show(); // 레벨업을 할 경우 uiLevelUp.Show 함수 호출
        }
    }

    // 시간 컨트롤 (레벨업 UI 창이 보이면 게임 시간을 일시정지)
    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0; // 유니티의 시간속도
        uiJoy.localScale = Vector3.zero;
    }

    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1; // 1보다 높은 값을 주면 빨리감기 하는 것처럼 보인다.
        uiJoy.localScale = Vector3.one;
    }
}
