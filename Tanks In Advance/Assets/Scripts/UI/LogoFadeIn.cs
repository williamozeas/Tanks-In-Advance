using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogoFadeIn : MonoBehaviour
{
    private Graphic logo;

    [SerializeField] private float _time = 4f;
    // Start is called before the first frame update
    void Start()
    {
        logo = GetComponent<Image>();
        logo.color = new Color(logo.color.r, logo.color.g, logo.color.b, 0);
    }

    public void PlayAnim()
    {
        StartCoroutine(FadeIn(_time));
    }

    public IEnumerator FadeIn(float time)
    {
        float timeElapsed = 0;
        while (timeElapsed < time)
        {
            float percent = EasingFunction.EaseOutQuad(0, 1, timeElapsed / time);
            logo.color = new Color(logo.color.r, logo.color.g, logo.color.b, percent);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        logo.color = new Color(logo.color.r, logo.color.g, logo.color.b, 1);
    }
}
