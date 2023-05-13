using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchiveManager : MonoBehaviour
{
    public GameObject[] lockCharacter;
    public GameObject[] unlockCharacter;
    public GameObject uiNotice; // ���ȴٴ� �������� �г��� ����

    enum Achive { UnlockPotato, UnlockBean }
    Achive[] achives; // ���������͵��� ����
    WaitForSecondsRealtime wait;

    private void Awake()
    {
        achives = (Achive[])Enum.GetValues(typeof(Achive)); // �־��� �������� �����͸� ��� �������� �Լ�
        wait = new WaitForSecondsRealtime(5);
        if (!PlayerPrefs.HasKey("MyData")) // �����Ͱ� ���ٸ� �ʱ�ȭ
        {
            Init();
        }
    }

    void Init()
    {
        PlayerPrefs.SetInt("MyData", 1); // ������ ���� ����� �����ϴ� ����Ƽ ���� Ŭ����

        foreach (Achive achive in achives) // ������ 3���� �ƴ϶� �� �̻��̶�� ������ ����� ������ ���� ������ ����
        {
            PlayerPrefs.SetInt(achive.ToString(), 0);
        }
    }

    private void Start()
    {
        UnlockCharacter();
    }

    void UnlockCharacter() // ������ �޼��ϸ� ��� ���ִ� ����
    {
        for (int index = 0; index < lockCharacter.Length; index++) // ��� ��ư �迭�� ��ȸ�ϸ鼭 �ε����� �ش��ϴ� �����̸� ������
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

    void CheckAchive(Achive achive) // ���� �޼� ����
    {
        bool isAchive = false;

        switch (achive) // ���� �޼� ����
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

        // ���� �޼�
        if (isAchive && PlayerPrefs.GetInt(achive.ToString()) == 0)
        {
            PlayerPrefs.SetInt(achive.ToString(), 1);

            // � ������ ��������
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
        //yield return new WaitForSeconds(5); // �ڿ�����
    }
}
