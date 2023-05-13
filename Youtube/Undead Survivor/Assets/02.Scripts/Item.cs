using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public itemData data;
    public int level;
    public Weapon weapon;
    public Gear gear;

    Image icon;
    Text textLevel;
    Text textName;
    Text textDesc;

    private void Awake()
    {
        icon = GetComponentsInChildren<Image>()[1]; // ù��°�� �ڱ� �ڽ�
        icon.sprite = data.itemIcon;

        Text[] texts = GetComponentsInChildren<Text>();
        textLevel = texts[0]; // �̷� ���ڷ� ������ �Ǵ°� ���������� ������ ����
        textName = texts[1];
        textDesc = texts[2];
        textName.text = data.itemName;
    }

    private void OnEnable()
    {
        textLevel.text = "Lv." + (level + 1); // 1���� �����ϱ� ����

        switch (data.itemType) // ������ Ÿ�Կ� ���� ǥ�õǴ� ���� �и��ϴ� ����
        {
            case itemData.ItemType.Melee:
            case itemData.ItemType.Range:
                textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100, data.counts[level]); // ������ %����� 100�� ����
                break;
            case itemData.ItemType.Glove:
            case itemData.ItemType.Shoe:
                textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100);
                break;
            default:
                textDesc.text = string.Format(data.itemDesc);
                break;
        }
        
    }

    /*
    private void LateUpdate()
    {
        //if (level != data.damages.Length)
        //{
        //    textLevel.text = "Lv." + (level + 1); // 1���� �����ϱ� ����
        //}
        //else
        //{
        //    textLevel.text = "Lv.Max";
        //}
    }
    */

    public void OnClick()
    {
        switch(data.itemType)
        {
            case itemData.ItemType.Melee:
            case itemData.ItemType.Range:
                if (level == 0)
                {
                    GameObject newWeapon = new GameObject();
                    weapon = newWeapon.AddComponent<Weapon>();
                    weapon.Init(data);
                }
                else
                {
                    float nextDamage = data.baseDamage;
                    int nextCount = 0;

                    // ó�� ������ �������� ������ �� Ƚ���� ���
                    nextDamage += data.baseDamage * data.damages[level]; // ������� �������� �����⶧���� ���̽� �������� ����
                    nextCount += data.counts[level];

                    weapon.LevelUp(nextDamage, nextCount); //Weapon �� �ۼ��� LevelUp �Լ��� �״�� Ȱ��
                }

                level++;
                break;
            case itemData.ItemType.Glove:
            case itemData.ItemType.Shoe:
                if (level == 0)
                {
                    // �ʱ�ȭ
                    GameObject newGear = new GameObject();
                    gear = newGear.AddComponent<Gear>();
                    gear.Init(data);
                }
                else
                {
                    float nextRate = data.damages[level];
                    gear.LevelUP(nextRate);
                }

                level++;
                break;
            case itemData.ItemType.Heal:
                // ��ȸ�������� Ƚ���� ������ �����ʱ� ���� level++�� gear,weapon���� ����
                GameManager.instance.health = GameManager.instance.maxHealth;
                break;
        }

        // �ִ� ������ �Ǿ��� �� ������ ����
        if (level == data.damages.Length)
        {
            GetComponent<Button>().interactable = false;
        }
    }
}
