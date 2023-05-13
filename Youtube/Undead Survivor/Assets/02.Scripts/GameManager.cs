using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // static -> �������� ����ϰڴ�. �ٷ� �޸𸮿� ����
    public static GameManager instance;

    [Header("# Game Control")]
    public bool isLive;
    public float gameTime;
    public float maxGameTime = 2 * 10f; //20�� 5���� 5 * 60
    [Header("# Player Info")]
    public int playerId; // ĳ���� ����â���� ID����
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
        instance = this; // �ʱ�ȭ
        Application.targetFrameRate = 60;
    }

    public void GameStart(int id)
    {
        playerId = id;
        health = maxHealth; // ���� ���� ���ڸ��� Ǯ��

        player.gameObject.SetActive(true);
        uiLevelUp.Select(playerId % 2); // �������� ������ ���� �Լ�ȣ�⿡�� ���� ���� ĳ����ID�� ����
        //uiLevelUp.Select(0); // �ӽ� ��ũ��Ʈ (ù��° ĳ���� ����)
        Resume();

        AudioManager.instance.PlayBgm(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    public void GameOver()
    {
        // ���� ������ ������ �Ǿ�� �ϹǷ� �ڷ�ƾ�� �̿�
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
        // ���� ������ ������ �Ǿ�� �ϹǷ� �ڷ�ƾ�� �̿�
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
        if (!isLive) // �� ��ũ��Ʈ�� Updata �� ���� �־���
            return;

        gameTime += Time.deltaTime;

        if (gameTime > maxGameTime)
        {
            gameTime = maxGameTime;
            GameVictory(); // ���ӽð��� �ִ�ð��� �ѱ�ٸ�
        }
    }

    public void GetExp()
    {
        if (!isLive)
            return;

        exp++;
        
        if (exp == nextExp[Mathf.Min(level, nextExp.Length-1)]) // �ʿ��� ����ġ�� �����ϸ� ������ ++ ������ �� �ְ� ����ġ�� ��� ���
        {
            level++;
            exp = 0;
            uiLevelUp.Show(); // �������� �� ��� uiLevelUp.Show �Լ� ȣ��
        }
    }

    // �ð� ��Ʈ�� (������ UI â�� ���̸� ���� �ð��� �Ͻ�����)
    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0; // ����Ƽ�� �ð��ӵ�
        uiJoy.localScale = Vector3.zero;
    }

    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1; // 1���� ���� ���� �ָ� �������� �ϴ� ��ó�� ���δ�.
        uiJoy.localScale = Vector3.one;
    }
}
