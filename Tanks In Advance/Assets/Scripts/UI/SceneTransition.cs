using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public void Transition(string scene)
    {
        GameManager.Instance.GameState = GameStates.BetweenRounds;
        SceneManager.LoadScene(scene);
    }
}
