using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TMPFadeIn : MonoBehaviour
{
    private TextMeshProUGUI text;

    [SerializeField] private float _time = 4f;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
    }

    public void PlayAnim()
    {
        StartCoroutine(FadeIn(_time));
    }

    public IEnumerator FadeIn(float time)
    {
        float timeElapsed = time / 2;
        while (true)
        {
            float percent = Mathf.Cos(Mathf.PI * 2 * (timeElapsed / time)) / 2 + 0.5f;;
            text.color = new Color(text.color.r, text.color.g, text.color.b, percent);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
}
