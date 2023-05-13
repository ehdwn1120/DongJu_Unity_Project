using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

public class HUD : MonoBehaviour
{
    public enum InfoType { Exp, Level, Kill, Time, Health }
    public InfoType type;

    Text myText;
    Slider mySlider;

    private void Awake()
    {
        myText = GetComponent<Text>();
        mySlider = GetComponent<Slider>();
    }

    private void LateUpdate()
    {
        switch (type)
        {
            case InfoType.Exp:
                // 슬라이더에 적용할 값은 현재경험치 나누기 최대경험치
                float curExp = GameManager.instance.exp;
                float maxExp = GameManager.instance.nextExp[Mathf.Min(GameManager.instance.level, GameManager.instance.nextExp.Length - 1)];
                mySlider.value = curExp / maxExp;
                break;
            case InfoType.Level:
                myText.text = string.Format("Lv.{0:F0}", GameManager.instance.level); // 첫번째 어떤 포맷을 쓸것인가 두번쨰는 그 포맷에 적용되는 데이터
                break;
            case InfoType.Kill:
                myText.text = string.Format("{0:F0}", GameManager.instance.kill);
                break;
            case InfoType.Time:
                // 흐르는 시간이 아닌 남은 시간을 구하기
                float remainTIme = GameManager.instance.maxGameTime - GameManager.instance.gameTime;
                // 분 과 초를 구하기
                int min = Mathf.FloorToInt(remainTIme / 60); // 60으로 나누어 분을 구하지만 FloorToInt로 뒤의 소수점을 버림
                int sec = Mathf.FloorToInt(remainTIme % 60); // % 나머지 후 소수점 빼면 초가 된다
                myText.text = string.Format("{0:D2}:{1:D2}", min, sec); // D0, D1, D2... : 자리수를 고정한다 00:00 이기때문에 하는 것
                break;
            case InfoType.Health:
                float curHealth = GameManager.instance.health;
                float maxHealth = GameManager.instance.maxHealth;
                mySlider.value = curHealth / maxHealth;
                break;
        }
    }
}
