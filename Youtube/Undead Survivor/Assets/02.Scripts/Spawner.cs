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
        level = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / levelTime), spawnData.Length - 1); // �ð��� ���� �ö󰡴� ����

        if (timer > spawnData[level].spawnTime)
        {
            timer = 0;
            Spawn();
        }
    }

    void Spawn()
    {
        GameObject enemy = GameManager.instance.pool.Get(0);
        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position; // 1���� �����ϴ� ������ �ڽŵ� �����̱� ����
        enemy.GetComponent<Enemy>().Init(spawnData[level]);
    }
}

// ����ȭ (Serialization) : ��ü�� ���� Ȥ�� �����ϱ� ���� ��ȯ
// ���� ������ Spawner���� SpawnData �Լ��� �޾ƿ����� �� ���� ������ �������� ���Ѵ�.
[System.Serializable]
public class SpawnData
{
    // Ÿ��, ��ȯ�ð�, ü��, �ӵ�
    public float spawnTime;
    public int spriteType; // ������ Ÿ�� 0 ���� 1 ���̷���
    public int health;
    public float speed;
}
