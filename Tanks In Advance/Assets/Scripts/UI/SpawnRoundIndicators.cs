using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnRoundIndicators : MonoBehaviour
{
    public GameObject roundIndicator;

    void Start()
    {
        for (int i = 0; i < GameManager.Instance.maxRounds; i++)
        {
            GameObject rgo = Instantiate(roundIndicator, transform);

            if (rgo.TryGetComponent<RoundIndicator>(out RoundIndicator ri))
            {
                ri.index = i + 1;
            }
        }

        GetComponent<HorizontalLayoutGroup>().spacing = Mathf.Max(
            10, 55 - 12 * Mathf.Sqrt(GameManager.Instance.maxRounds));
    }
}
