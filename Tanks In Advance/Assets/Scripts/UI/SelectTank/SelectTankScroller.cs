using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectTankScroller : MonoBehaviour
{
    private int CENTER;

    public List<GameObject> points;

    // [HideInInspector]
    public List<TankSelectBox> tankSelectBoxes;

    private Animator animator;

    private PlayerNum playerNum;
    private int selection = 0;
    private bool scrolling = false;

    public void Initialize()
    {
        CENTER = points.Count / 2;
        if (animator == null)
            animator = GetComponent<Animator>();

        playerNum = tankSelectBoxes[0].playerNum;

        // duplicate buttons for scrolling
        int tsbc = tankSelectBoxes.Count;
        while (tankSelectBoxes.Count < points.Count)
        {
            for (int i = 0; i < tsbc; i++)
                tankSelectBoxes.Add(Instantiate(tankSelectBoxes[i]));
        }

        selection = 0;

        ResetPoints();
        StartCoroutine(DetectKeyPresses());
    }

    private void ResetPoints()
    {
        for (int i = 0; i < tankSelectBoxes.Count; i++)
        {
            int posIndex = (i - selection + tankSelectBoxes.Count) % tankSelectBoxes.Count;
            int pointIndex = (posIndex + CENTER) % tankSelectBoxes.Count;

            TankSelectBox tsb = tankSelectBoxes[i];

            if (pointIndex < points.Count)
            {
                tsb.transform.SetParent(points[pointIndex].transform);
                tsb.transform.position = points[pointIndex].transform.position;
                tsb.transform.localScale = Vector3.one;
            }
            else
                tsb.transform.position = Vector3.right * 10000;
        }

        scrolling = false;
    }

    private IEnumerator DetectKeyPresses()
    {
        string moveString = (playerNum == PlayerNum.Player1) ? "P1" : "P2";

        // wait a small bit: don't want accidental selections!
        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => Input.GetAxis(moveString + "_Move_V") == 0);

        while (true) {
            yield return new WaitForFixedUpdate();

            if (Input.GetAxis(moveString + "_Move_V") > 0.5)
            {
                selection = (selection - 1 + tankSelectBoxes.Count) % tankSelectBoxes.Count;
                animator.SetTrigger("ScrollDown");
                scrolling = true;
                yield return new WaitUntil(() => !scrolling);
            }
            else if (Input.GetAxis(moveString + "_Move_V") < -0.5)
            {
                selection = (selection + 1 + tankSelectBoxes.Count) % tankSelectBoxes.Count;
                animator.SetTrigger("ScrollUp");
                scrolling = true;
                yield return new WaitUntil(() => !scrolling);
            }
            else if (Input.GetButtonDown(moveString + "_Fire"))
            {
                if (tankSelectBoxes[selection].selectable)
                {
                    FindObjectOfType<SelectTank>().Select(playerNum, tankSelectBoxes[selection].type);
                    break;
                }
            }
        }
    }
}
