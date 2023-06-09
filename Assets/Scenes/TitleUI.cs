using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
public class TitleUI : MonoBehaviour
{
    public GameObject Panel;
    public GameObject Panel2;
    public GameObject Panel3;
    public void StartButton()
    {
        SceneManager.LoadScene("mainstage1");

    }
    public void OptionBution()
    {
        Panel.SetActive(true);
    }
    public void CloseOptionBution()
    {
        Panel3.SetActive(false);
        Panel2.SetActive(false);
    }
    public void nextButton()
    {
        Panel.SetActive(false);
        Panel2.SetActive(true);
    }
    public void nextButton2()
    {
        Panel2.SetActive(false);
        Panel3.SetActive(true);
    }
}
