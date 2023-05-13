using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    // .. 프리펩들을 보관할 변수
    public GameObject[] prefabs;

    // .. 풀 담당을 해주는 리스트들 (변수와 리스트의 개수는 같아야함)
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

        // 선택한 풀의 놀고(비활성화 된) 있는 게임오브젝트 접근
        // foreach = 배열, 리스트들의 데이터를 "순차적" 으로 접근하는 반복문
        foreach (GameObject item in pools[index])
        {
            if (!item.activeSelf)
            {
                // 발견하면 select 변수에 할당
                select = item;
                select.SetActive(true);
                break;
            }
        }

        // 못 찾았다면?
        if (!select)
        {
            // 새롭게 생성하고 select 변수에 할당 // Instantiate : 원본 오브젝트를 복제하여 장면에 생성
            select = Instantiate(prefabs[index], transform); // transform = 자식오브젝트로 넣겠다
            pools[index].Add(select);
        }

        return select;
    }
}
