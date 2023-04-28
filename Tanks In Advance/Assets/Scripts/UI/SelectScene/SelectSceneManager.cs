using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectSceneManager : MonoBehaviour
{
    enum SelectState
    {
        NOTHING,
        MAP_SELECTED,
        ROUND_SELECTED,
    }

    private SelectState selectState;
    [SerializeField] private SelectMap selectMap;
    [SerializeField] private SelectRound selectRound;
    [SerializeField] private GameObject confirmSelection;
    [SerializeField] private List<GameObject> hideOnSelected;

    // Start is called before the first frame update
    void Start()
    {
        confirmSelection.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            confirmSelection.SetActive(false);
            switch (selectState)
            {
                case SelectState.NOTHING:
                    break;
                case SelectState.MAP_SELECTED:
                    selectMap.Unselect();
                    selectRound.UnActivate();
                    selectState = SelectState.NOTHING;
                    break;
                case SelectState.ROUND_SELECTED:
                    selectRound.Activate();
                    foreach (GameObject gameObject in hideOnSelected)
                    {
                        gameObject.SetActive(true);
                    }
                    selectState = SelectState.MAP_SELECTED;
                    break;
            }
        }

        if (Input.GetButtonDown("P1_Fire") || Input.GetButtonDown("P2_Fire"))
        {
            if (selectState == SelectState.ROUND_SELECTED)
            {
                OnConfirmSelected();
            }
        }
    }

    public void OnMapSelected()
    {
        selectState = SelectState.MAP_SELECTED;
        selectRound.Activate();
    }

    public void OnRoundSelected()
    {
        selectState = SelectState.ROUND_SELECTED;
        foreach (GameObject gameObject in hideOnSelected)
        {
            gameObject.SetActive(false);
        }
        confirmSelection.SetActive(true);
    }

    public void OnConfirmSelected()
    {
        SceneManager.LoadScene("Main Scene");
    }
}
