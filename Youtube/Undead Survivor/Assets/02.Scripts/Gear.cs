using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : MonoBehaviour
{
    public itemData.ItemType type;
    public float rate; // 레벨별

    public void Init(itemData data)
    {
        // Basic Set
        name = "Gear" + data.itemId;
        transform.parent = GameManager.instance.player.transform; // player가 없기 때문에 가져옴
        transform.localPosition = Vector3.zero;

        // Property Set
        type = data.itemType;
        rate = data.damages[0];
        ApplyGear();
    }

    public void LevelUP(float rate)
    {
        this.rate = rate;
        ApplyGear();
    }

    // RateUp함수와 SpeedUp함수의 호출을 담당하는 함수 ( 타입에 따라 적절하게 로직을 적용시켜주는 함수)
    void ApplyGear()
    {
        switch (type)
        {
            case itemData.ItemType.Glove:
                RateUp();
                break;
            case itemData.ItemType.Shoe:
                SpeedUp();
                break;
        }
    }

    // 연사력을 올리는 함수
    void RateUp()
    {
        Weapon[] weapons = transform.parent.GetComponentsInChildren<Weapon>(); // Player 로 올라가서 모든 Weapon을 가져옴

        foreach (Weapon weapon in weapons)
        {
            switch(weapon.id)
            {
                case 0:
                    float speed = 150 * Character.WeaponSpeed;
                    weapon.speed = speed + (speed * rate); // 근접무기
                    break;
                default:
                    speed = 0.5f * Character.WeaponRate;
                    weapon.speed = speed * (1f - rate); // 원거리무기
                    break;
            }
        }
    }

    // 이동속도를 올리는 함수
    void SpeedUp()
    {
        float speed = 3 * Character.Speed;
        GameManager.instance.player.speed = speed + speed * rate;
    }
}
