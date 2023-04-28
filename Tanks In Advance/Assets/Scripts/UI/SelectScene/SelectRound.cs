using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectRound : MonoBehaviour
{
    public TextMeshProUGUI prev2_txt;
    public TextMeshProUGUI prev1_txt;
    public TextMeshProUGUI curr_txt;
    public TextMeshProUGUI next1_txt;
    public TextMeshProUGUI next2_txt;

    // public GameObject covering;
    // public GameObject selectedTxt;

    [SerializeField] private SelectSceneManager selectSceneManager;

    public bool active;
    public bool selected;

    private int selection = 5;
    private float duration = 0;

    private const int MAX_ROUND = 9;

    void Start()
    {
        active = false;
        selected = false;
        UpdateSelectionUI();
    }

    void Update()
    {
        if (!active)
        {
            return;
        }
        
        if (Input.GetButtonDown("P1_Fire") || Input.GetButtonDown("P2_Fire"))
        {
            DataManager.Instance().roundCnt = selection;
            Select();
            return;
        }
        
        duration += Time.deltaTime;
        if (duration < 0.3)
        {
            return;
        }
        
        if (Input.GetAxis("P1_Move_H") > 0.5 || Input.GetAxis("P2_Move_H") > 0.5)
        {
            AudioManager.Instance.Swipe();
            if (selection < MAX_ROUND)
            {
                selection += 1;
            }
            UpdateSelectionUI();
            duration = 0;
        }
        else if (Input.GetAxis("P1_Move_H") < -0.5 || Input.GetAxis("P2_Move_H") < -0.5)
        {
            AudioManager.Instance.Swipe();
            if (selection > 1)
            {
                selection -= 1;
            }
            UpdateSelectionUI();
            duration = 0;
        }
    }

    public void UnActivate()
    {
        active = false;
        selected = false;
        UpdateSelectionUI();
    }
    
    public void Activate()
    {
        active = true;
        selected = false;
        UpdateSelectionUI();
    }

    public void Select()
    {
        selectSceneManager.OnRoundSelected();
        active = false;
        selected = true;
        UpdateSelectionUI();
    }

    private void UpdateSelectionUI()
    {
        int prev2_id = selection - 2;
        if (prev2_id > 0)
        {
            prev2_txt.text = prev2_id.ToString();
        }
        else
        {
            prev2_txt.text = "";
        }
        
        int prev1_id = selection - 1;
        if (prev1_id > 0)
        {
            prev1_txt.text = prev1_id.ToString();
        }
        else
        {
            prev1_txt.text = "";
        }
        
        curr_txt.text = selection.ToString();

        int next1_id = selection + 1;
        if (next1_id <= MAX_ROUND)
        {
            next1_txt.text = next1_id.ToString();
        }
        else
        {
            next1_txt.text = "";
        }
        
        int next2_id = selection + 2;
        if (next2_id <= MAX_ROUND)
        {
            next2_txt.text = next2_id.ToString();
        }
        else
        {
            next2_txt.text = "";
        }

        // if (active)
        // {
        //     covering.SetActive(false);
        // }
        // else
        // {
        //     covering.SetActive(true);
        // }
        //
        // if (selected)
        // {
        //     selectedTxt.SetActive(true);
        // }
        // else
        // {
        //     selectedTxt.SetActive(false);
        // }
    }
}
