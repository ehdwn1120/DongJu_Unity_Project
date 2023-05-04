using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public GameObject MainPanel;
    public GameObject OptionPanel;
    public GameObject PatchPanel;

    private void Start()
    {
        OptionPanel.SetActive(false);
    }
    public void GameStart()
    {
        //DontDestroyOnLoad(transform.gameObject);
        SceneManager.LoadScene("GameScene");

    }

    public void Option()
    {
        MainPanel.SetActive(false);
        OptionPanel.SetActive(true);
    }

    public void OptionBack()
    {
        OptionPanel.SetActive(false);
        MainPanel.SetActive(true);
    }

    public void Patch()
    {
        MainPanel.SetActive(false);
        PatchPanel.SetActive(true);
    }

    public void PatchBack()
    {
        PatchPanel.SetActive(false);
        MainPanel.SetActive(true);
    }



    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Exit");
    }
}
