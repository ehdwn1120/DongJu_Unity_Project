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
                // �����̴��� ������ ���� �������ġ ������ �ִ����ġ
                float curExp = GameManager.instance.exp;
                float maxExp = GameManager.instance.nextExp[Mathf.Min(GameManager.instance.level, GameManager.instance.nextExp.Length - 1)];
                mySlider.value = curExp / maxExp;
                break;
            case InfoType.Level:
                myText.text = string.Format("Lv.{0:F0}", GameManager.instance.level); // ù��° � ������ �����ΰ� �ι����� �� ���˿� ����Ǵ� ������
                break;
            case InfoType.Kill:
                myText.text = string.Format("{0:F0}", GameManager.instance.kill);
                break;
            case InfoType.Time:
                // �帣�� �ð��� �ƴ� ���� �ð��� ���ϱ�
                float remainTIme = GameManager.instance.maxGameTime - GameManager.instance.gameTime;
                // �� �� �ʸ� ���ϱ�
                int min = Mathf.FloorToInt(remainTIme / 60); // 60���� ������ ���� �������� FloorToInt�� ���� �Ҽ����� ����
                int sec = Mathf.FloorToInt(remainTIme % 60); // % ������ �� �Ҽ��� ���� �ʰ� �ȴ�
                myText.text = string.Format("{0:D2}:{1:D2}", min, sec); // D0, D1, D2... : �ڸ����� �����Ѵ� 00:00 �̱⶧���� �ϴ� ��
                break;
            case InfoType.Health:
                float curHealth = GameManager.instance.health;
                float maxHealth = GameManager.instance.maxHealth;
                mySlider.value = curHealth / maxHealth;
                break;
        }
    }
}
