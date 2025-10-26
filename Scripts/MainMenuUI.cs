using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
    }

    public void StartGameButton()
    {
        SceneManager.LoadScene(1);
    }

    public void ExitGameButton()
    {
        Application.Quit();
    }

    public void LoadLevel1()   { SceneManager.LoadScene("Level 1"); }   
    public void LoadLevel2()   { SceneManager.LoadScene("Level 2"); }
    public void LoadLevel3()   { SceneManager.LoadScene("Level 3"); }
    public void LoadLevel4()   { SceneManager.LoadScene("Level 4"); }
}
