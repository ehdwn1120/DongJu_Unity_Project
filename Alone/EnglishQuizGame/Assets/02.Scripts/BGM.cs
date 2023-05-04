using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGM : MonoBehaviour
{
    public AudioSource bgm;
    public bool isBgmOnOff;
    public bool isBGMStart;

    public Image image;
    public Sprite onIcon;
    public Sprite offIcon;


    private void Awake()
    {
        var Music = FindObjectsOfType<BGM>();
        if (Music.Length == 1)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        bgm.Play();
    }

    public void OptionMusicOnOff()
    {
        Debug.Log("OptionMusicOnOff");
        if (isBgmOnOff == false)
        {
            SoundOffIcon();
            Debug.Log("BGM Stop");
            bgm.Stop();
            isBgmOnOff = true;
        }
        else
        {
            SoundOnIcon();
            Debug.Log("BGM Play");
            bgm.Play();
            isBgmOnOff = false;
        }
    }

    public void SoundOnIcon()
    {
        image.sprite = onIcon;
    }

    public void SoundOffIcon()
    {
        image.sprite = offIcon;
    }
}
