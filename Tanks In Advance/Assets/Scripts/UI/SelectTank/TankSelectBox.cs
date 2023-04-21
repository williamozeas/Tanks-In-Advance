using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TankSelectBox : MonoBehaviour
{
    public Image tankImage;
    public TextMeshProUGUI tankName;
    public TextMeshProUGUI tankCount;

    [HideInInspector] public TankType type;
    [HideInInspector] public PlayerNum playerNum;
    [HideInInspector] public bool selectable;

    public void Initialize(TankType t, PlayerNum p, int q)
    {
        type = t;
        playerNum = p;

        Tank tank = GameManager.Instance.TankList.GetTank(playerNum, type).GetComponent<Tank>();
        tankName.text = tank.tankName;
        GetComponentInChildren<Animator>().runtimeAnimatorController = tank.tankUIAnimation;

        if (q > 100)
            tankCount.text = "\u221E";
        else
            tankCount.text = "x" + q;

        selectable = q > 0;

        if (!selectable)
        {
            float TALPHA = 0.1f;

            tankImage.color = new Color(1f, 1f, 1f, TALPHA);

            Color rColor = tankName.color;
            rColor.a = TALPHA;
            tankName.color = rColor;

            rColor = tankCount.color;
            rColor.a = TALPHA;
            tankCount.color = rColor;
        }
    }
}
