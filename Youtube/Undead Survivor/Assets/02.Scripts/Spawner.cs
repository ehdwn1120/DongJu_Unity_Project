using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;
    public float levelTime;

    int level;
    float timer;

    private void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>();
        levelTime = GameManager.instance.maxGameTime / spawnData.Length;
    }

    private void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        timer += Time.deltaTime;
        level = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / levelTime), spawnData.Length - 1); // 시간에 맞춰 올라가는 레벨

        if (timer > spawnData[level].spawnTime)
        {
            timer = 0;
            Spawn();
        }
    }

    void Spawn()
    {
        GameObject enemy = GameManager.instance.pool.Get(0);
        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position; // 1부터 시작하는 이유는 자신도 포함이기 때문
        enemy.GetComponent<Enemy>().Init(spawnData[level]);
    }
}

// 직렬화 (Serialization) : 개체를 저장 혹은 전송하기 위해 변환
// 쓰는 이유는 Spawner에서 SpawnData 함수를 받아오지만 그 안의 정보는 가져오지 못한다.
[System.Serializable]
public class SpawnData
{
    // 타입, 소환시간, 체력, 속도
    public float spawnTime;
    public int spriteType; // 몬스터의 타입 0 좀비 1 스켈레톤
    public int health;
    public float speed;
}
