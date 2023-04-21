using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SelectMap : MonoBehaviour
{
    public Image prev_map;
    public TextMeshProUGUI prev_map_txt;
    public Image curr_map;
    public TextMeshProUGUI curr_map_txt;
    public Image next_map;
    public TextMeshProUGUI next_map_txt;
    
    public List<MapSelectBox> mapSelectBoxes;

    [SerializeField] private MapSpawner mapSpawner;

    private int selection = 0;

    private IEnumerator DetectKeyPresses()
    {
        // wait a small bit: don't want accidental selections!
        yield return new WaitForSeconds(0.5f);
        // yield return new WaitUntil(() => Input.GetAxis(moveString + "_Move_V") == 0);

        while (true) {
            yield return new WaitForFixedUpdate();

            if (Input.GetAxis("P1_Move_H") > 0.5 || Input.GetAxis("P2_Move_H") > 0.5)
            {
                AudioManager.Instance.Swipe();
                selection = (selection - 1 + mapSelectBoxes.Count) % mapSelectBoxes.Count;
                UpdateSelectionUI();
            }
            else if (Input.GetAxis("P1_Move_H") < -0.5 || Input.GetAxis("P2_Move_H") < -0.5)
            {
                AudioManager.Instance.Swipe();
                selection = (selection + 1 + mapSelectBoxes.Count) % mapSelectBoxes.Count;
                UpdateSelectionUI();
            }
            else if (Input.GetButtonDown("P1_Fire") || Input.GetButtonDown("P2_Fire"))
            {
                mapSpawner.SpawnMap(mapSelectBoxes[selection].name);
                break;
            }
        }
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
    }
}
