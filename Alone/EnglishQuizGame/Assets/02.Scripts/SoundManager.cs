using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public SoundManager clickSound;
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}