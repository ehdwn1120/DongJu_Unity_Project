using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    // .. ��������� ������ ����
    public GameObject[] prefabs;

    // .. Ǯ ����� ���ִ� ����Ʈ�� (������ ����Ʈ�� ������ ���ƾ���)
    List<GameObject>[] pools;

    private void Awake()
    {
        pools = new List<GameObject>[prefabs.Length];

        for (int index = 0; index < pools.Length; index++)
        {
            pools[index] = new List<GameObject>();
        }
    }

    public GameObject Get(int index)
    {
        GameObject select = null;

        // ������ Ǯ�� ���(��Ȱ��ȭ ��) �ִ� ���ӿ�����Ʈ ����
        // foreach = �迭, ����Ʈ���� �����͸� "������" ���� �����ϴ� �ݺ���
        foreach (GameObject item in pools[index])
        {
            if (!item.activeSelf)
            {
                // �߰��ϸ� select ������ �Ҵ�
                select = item;
                select.SetActive(true);
                break;
            }
        }

        // �� ã�Ҵٸ�?
        if (!select)
        {
            // ���Ӱ� �����ϰ� select ������ �Ҵ� // Instantiate : ���� ������Ʈ�� �����Ͽ� ��鿡 ����
            select = Instantiate(prefabs[index], transform); // transform = �ڽĿ�����Ʈ�� �ְڴ�
            pools[index].Add(select);
        }

        return select;
    }
}
