using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void MainMenuButton()
    {
        switch (this.gameObject.name) 
        {
            case "StartBtn":
                SceneManager.LoadScene("ARScene");
                break;

            case "ExitBtn":
                Application.Quit(); 
                break;
        }

    }

    public void BackButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
