using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectTank : MonoBehaviour
{
    public GameObject elements;

    public GameObject tankSelectBoxPrefabP1;
    public SelectTankScroller selectTankScrollerP1;
    public GameObject p1Ready;

    public GameObject tankSelectBoxPrefabP2;
    public SelectTankScroller selectTankScrollerP2;
    public GameObject p2Ready;

    private TankManager tankManager;
    private bool p1Selected;
    private bool p2Selected;
    private bool setup;

    public void Start()
    {
        tankManager = FindObjectOfType<TankManager>();
        elements.SetActive(false);
        setup = false;

        Invoke("StartGame", 3f);
    }

    void StartGame()
    {
        if (GameManager.Instance.GameState == GameStates.MainMenu)
            GameManager.Instance.GameState = GameStates.BetweenRounds;
    }

    public void Initialize()
    {
        elements.SetActive(true);

        p1Selected = false;
        p2Selected = false;

        selectTankScrollerP1.gameObject.SetActive(true);
        p1Ready.SetActive(false);
        selectTankScrollerP2.gameObject.SetActive(true);
        p2Ready.SetActive(false);

        if (!setup)
        {
            Debug.Log("Debug: Use RShift to select for P1");

            // Player 1
            foreach ((TankType tankType, int quant) in tankManager._p1AvailableTanks)
            {
                GameObject ts = Instantiate(tankSelectBoxPrefabP1, selectTankScrollerP1.transform);
                TankSelectBox tsb = ts.GetComponent<TankSelectBox>();

                selectTankScrollerP1.tankSelectBoxes.Add(tsb);
                tsb.Initialize(tankType, PlayerNum.Player1, quant);
            }

            // Player 2
            foreach ((TankType tankType, int quant) in tankManager._p2AvailableTanks)
            {
                GameObject ts = Instantiate(tankSelectBoxPrefabP2, selectTankScrollerP2.transform);
                TankSelectBox tsb = ts.GetComponent<TankSelectBox>();

                selectTankScrollerP2.tankSelectBoxes.Add(tsb);
                tsb.Initialize(tankType, PlayerNum.Player2, quant);
            }

            setup = true;
        }
        else
        {
            // Player 1
            int l = 0;
            foreach ((TankType tankType, int quant) in tankManager._p1AvailableTanks)
            {
                for (int i = l; i < selectTankScrollerP1.tankSelectBoxes.Count; i += tankManager._p1AvailableTanks.Count)
                    selectTankScrollerP1.tankSelectBoxes[i].Initialize(tankType, PlayerNum.Player1, quant);
                l++;
            }

            // Player 2
            l = 0;
            foreach ((TankType tankType, int quant) in tankManager._p2AvailableTanks)
            {
                for (int i = l; i < selectTankScrollerP2.tankSelectBoxes.Count; i += tankManager._p2AvailableTanks.Count)
                    selectTankScrollerP2.tankSelectBoxes[i].Initialize(tankType, PlayerNum.Player2, quant);
                l++;
            }
        }

        selectTankScrollerP1.Initialize();
        selectTankScrollerP2.Initialize();
    }

    public void Select (PlayerNum player, TankType tank)
    {
        AudioManager.Instance.Select();
        switch (player) {
            case PlayerNum.Player1:
                p1Selected = true;
                FindObjectOfType<TankManager>().P1selectedType = tank;
                selectTankScrollerP1.gameObject.SetActive(false);
                p1Ready.SetActive(true);
                break;

            case PlayerNum.Player2:
                p2Selected = true;
                FindObjectOfType<TankManager>().P2selectedType = tank;
                selectTankScrollerP2.gameObject.SetActive(false);
                p2Ready.SetActive(true);
                break;
    }
        if (p1Selected && p2Selected)
        {
            Invoke("StartRound", 1f);
        }
    }

    void StartRound()
    {
        elements.SetActive(false);
        GameManager.Instance.SetGameState(GameStates.Playing);
    }
}
