using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchiveManager : MonoBehaviour
{
    public GameObject[] lockCharacter;
    public GameObject[] unlockCharacter;
    public GameObject uiNotice; // 열렸다는 공지사항 패널을 저장

    enum Achive { UnlockPotato, UnlockBean }
    Achive[] achives; // 업적데이터들을 저장
    WaitForSecondsRealtime wait;

    private void Awake()
    {
        achives = (Achive[])Enum.GetValues(typeof(Achive)); // 주어진 열거형의 데이터를 모두 가져오는 함수
        wait = new WaitForSecondsRealtime(5);
        if (!PlayerPrefs.HasKey("MyData")) // 데이터가 없다면 초기화
        {
            Init();
        }
    }

    void Init()
    {
        PlayerPrefs.SetInt("MyData", 1); // 간단한 저장 기능을 제공하는 유니티 제공 클래스

        foreach (Achive achive in achives) // 업적이 3개가 아니라 그 이상이라면 관리가 힘들기 때문에 관리 로직을 만듬
        {
            PlayerPrefs.SetInt(achive.ToString(), 0);
        }
    }

    private void Start()
    {
        UnlockCharacter();
    }

    void UnlockCharacter() // 업적을 달성하면 언락 해주는 로직
    {
        for (int index = 0; index < lockCharacter.Length; index++) // 잠금 버튼 배열을 순회하면서 인덱스에 해당하는 업적이름 가져옴
        {
            string achiveName = achives[index].ToString();
            bool isUnlock = PlayerPrefs.GetInt(achiveName) == 1;
            lockCharacter[index].SetActive(!isUnlock);
            unlockCharacter[index].SetActive(isUnlock);
        }
    }

    void LateUpdate()
    {
        foreach (Achive achive in achives)
        {
            CheckAchive(achive);
        }
    }

    void CheckAchive(Achive achive) // 업적 달성 로직
    {
        bool isAchive = false;

        switch (achive) // 업적 달성 조건
        {
            case Achive.UnlockPotato:
                if (GameManager.instance.isLive)
                {
                    isAchive = GameManager.instance.kill >= 300;
                }
                break;
            case Achive.UnlockBean:
                isAchive = GameManager.instance.gameTime == GameManager.instance.maxGameTime;
                break;
        }

        // 업적 달성
        if (isAchive && PlayerPrefs.GetInt(achive.ToString()) == 0)
        {
            PlayerPrefs.SetInt(achive.ToString(), 1);

            // 어떤 내용을 보여줄지
            for (int index = 0; index < uiNotice.transform.childCount; index++)
            {
                bool isActive = index == (int)achive;
                uiNotice.transform.GetChild(index).gameObject.SetActive(isActive);
            }

            StartCoroutine(NoticeRoutine());
        }
    }

    IEnumerator NoticeRoutine()
    {
        uiNotice.SetActive(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);


        yield return wait;

        uiNotice.SetActive(false);
        //yield return new WaitForSeconds(5); // 자원낭비
    }
}
