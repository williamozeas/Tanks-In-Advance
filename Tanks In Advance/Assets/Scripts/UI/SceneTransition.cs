using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    private bool paused = false;

    public void Transition(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    void Update()
    {
        if (Input.GetButtonDown("Pause"))
            TogglePause();
    }

    public void TogglePause()
    {
        if (!paused)
        {
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
            paused = true;
        }
        else
        {
            Time.timeScale = 1f;
            pausePanel.SetActive(false);
            paused = false;
        }

    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
