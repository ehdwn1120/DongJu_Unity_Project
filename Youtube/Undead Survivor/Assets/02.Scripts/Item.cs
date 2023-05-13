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
        icon = GetComponentsInChildren<Image>()[1]; // 첫번째는 자기 자신
        icon.sprite = data.itemIcon;

        Text[] texts = GetComponentsInChildren<Text>();
        textLevel = texts[0]; // 이런 숫자로 구분이 되는건 계층구조의 순서를 따라감
        textName = texts[1];
        textDesc = texts[2];
        textName.text = data.itemName;
    }

    private void OnEnable()
    {
        textLevel.text = "Lv." + (level + 1); // 1부터 시작하기 위함

        switch (data.itemType) // 아이템 타입에 따라 표시되는 것을 분리하는 로직
        {
            case itemData.ItemType.Melee:
            case itemData.ItemType.Range:
                textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100, data.counts[level]); // 데미지 %상승은 100을 곱함
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
        //    textLevel.text = "Lv." + (level + 1); // 1부터 시작하기 위함
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

                    // 처음 이후의 레벨업은 데미지 와 횟수를 계산
                    nextDamage += data.baseDamage * data.damages[level]; // 백분율로 데미지가 오르기때문에 베이스 데미지와 더함
                    nextCount += data.counts[level];

                    weapon.LevelUp(nextDamage, nextCount); //Weapon 의 작성된 LevelUp 함수를 그대로 활용
                }

                level++;
                break;
            case itemData.ItemType.Glove:
            case itemData.ItemType.Shoe:
                if (level == 0)
                {
                    // 초기화
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
                // 일회성이지만 횟수에 제한을 두지않기 위해 level++를 gear,weapon에만 적용
                GameManager.instance.health = GameManager.instance.maxHealth;
                break;
        }

        // 최대 레벨이 되었을 때 레벨업 방지
        if (level == data.damages.Length)
        {
            GetComponent<Button>().interactable = false;
        }
    }
}
