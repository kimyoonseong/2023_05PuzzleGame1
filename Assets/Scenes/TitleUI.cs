using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
public class TitleUI : MonoBehaviour
{
    public GameObject Panel;
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
        Panel.SetActive(false);
    }
}
