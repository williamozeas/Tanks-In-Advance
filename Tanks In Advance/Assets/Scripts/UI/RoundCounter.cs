using System;
using UnityEngine;
using TMPro;

namespace UI
{
    public class RoundCounter : MonoBehaviour
    {
        private TextMeshProUGUI _tmp;
        
        // Start is called before the first frame update
        void Start()
        {
            _tmp = GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            GameManager.OnRoundEnd += OnRoundEnd;
            GameManager.OnRoundStart += OnRoundStart;
        }

        private void OnDisable()
        {
            GameManager.OnRoundEnd -= OnRoundEnd;
            GameManager.OnRoundStart -= OnRoundStart;
        }

        private void OnRoundEnd()
        {
            _tmp.text = "Round " + GameManager.Instance.RoundNumber;
        }

        private void OnRoundStart(Round round)
        {
            _tmp.text = "Round " + GameManager.Instance.RoundNumber;
        }
    }
}
