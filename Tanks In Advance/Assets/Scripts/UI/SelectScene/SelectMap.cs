using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectMap : MonoBehaviour
{
    public Image prev_map;
    public TextMeshProUGUI prev_map_txt;
    public Image curr_map;
    public TextMeshProUGUI curr_map_txt;
    public Image next_map;
    public TextMeshProUGUI next_map_txt;

    public GameObject covering;
    public GameObject selectedTxt;
    
    public List<MapSelectBox> mapSelectBoxes;


    [SerializeField] private SelectSceneManager selectSceneManager;

    public bool active;
    public bool selected;

    private int selection = 0;
    private float duration = 0;

    void Start()
    {
        active = true;
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
            AudioManager.Instance.Select();
            DataManager.Instance().selectedMap = mapSelectBoxes[selection].name;
            DataManager.Instance().selectedMapName = mapSelectBoxes[selection].mapName;
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
            selection = (selection + 1 + mapSelectBoxes.Count) % mapSelectBoxes.Count;
            UpdateSelectionUI();
            duration = 0;
        }
        else if (Input.GetAxis("P1_Move_H") < -0.5 || Input.GetAxis("P2_Move_H") < -0.5)
        {
            AudioManager.Instance.Swipe();
            selection = (selection - 1 + mapSelectBoxes.Count) % mapSelectBoxes.Count;
            UpdateSelectionUI();
            duration = 0;
        }
    }
    
    public void Activate()
    {
        active = true;
    }

    public void Select()
    {
        selectSceneManager.OnMapSelected();
        active = false;
        selected = true;
        UpdateSelectionUI();
    }

    public void Unselect()
    {
        active = true;
        selected = false;
        UpdateSelectionUI();
    }

    private void UpdateSelectionUI()
    {
        int prev_id = (selection - 1 + mapSelectBoxes.Count) % mapSelectBoxes.Count;
        MapSelectBox prev_box = mapSelectBoxes[prev_id];
        prev_map.sprite = prev_box.mapSprite;
        prev_map_txt.text = prev_box.mapName;

        MapSelectBox current_box = mapSelectBoxes[selection];
        curr_map.sprite = current_box.mapSprite;
        curr_map_txt.text = current_box.mapName;
        
        int next_id = (selection + 1 + mapSelectBoxes.Count) % mapSelectBoxes.Count;
        MapSelectBox next_box = mapSelectBoxes[next_id];
        next_map.sprite = next_box.mapSprite;
        next_map_txt.text = next_box.mapName;

        if (active)
        {
            covering.SetActive(false);
        }
        else
        {
            covering.SetActive(true);
        }

        if (selected)
        {
            selectedTxt.SetActive(true);
        }
        else
        {
            selectedTxt.SetActive(false);
        }
    }
}
