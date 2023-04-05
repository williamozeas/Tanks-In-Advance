using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCircle : MonoBehaviour
{
    private enum CircleColor
    {
        Blue, Pink, White
    }
    
    public int numTanksP1 = 0;
    public int numTanksP2 = 0;
    public float colorFadeTime = 0.8f;

    [Header("Colors")] 
    [ColorUsage(true, true)] public Color blueColor;
    [ColorUsage(true, true)] public Color pinkColor;
    [ColorUsage(true, true)] public Color whiteColor;

    private Coroutine _colorChangeCoroutine;
    private CircleColor _currentColor;
    private Renderer _renderer;
    private Material mat;

    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        mat = _renderer.sharedMaterial;
        _colorChangeCoroutine = StartCoroutine(ChangeColor(whiteColor, 0.01f));
    }

    void Update()
    {
        Debug.Log("Blue: " + numTanksP1 + " Pink: " + numTanksP2);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.parent == null || other.name != "Tank Model") return;
        
        if(other.transform.parent.GetComponent<Tank>()){  //If tank component exists (ie != NULL)
            if(other.transform.parent.GetComponent<Tank>().ownerNum == PlayerNum.Player1){
                numTanksP1++;
            }else{
                numTanksP2++;
            }
        }

        UpdateColor();
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.transform.parent == null || other.name != "Tank Model") return;
        
        if(other.transform.parent.GetComponent<Tank>() ){  //If tank component exists (ie != NULL)
            if(other.transform.parent.GetComponent<Tank>().ownerNum == PlayerNum.Player1){
                numTanksP1--;
            }else{
                numTanksP2--;
            }
        }
        
        UpdateColor();
    }

    private void UpdateColor()
    {
        if (numTanksP1 > numTanksP2 && _currentColor != CircleColor.Blue)
        {
            //set to blue
            _currentColor = CircleColor.Blue;
            if (_colorChangeCoroutine != null)
            {
                StopCoroutine(_colorChangeCoroutine);
            }
            _colorChangeCoroutine = StartCoroutine(ChangeColor(blueColor, colorFadeTime));
        } else if (numTanksP1 < numTanksP2 && _currentColor != CircleColor.Pink)
        {
            //set to pink
            _currentColor = CircleColor.Pink;
            if (_colorChangeCoroutine != null)
            {
                StopCoroutine(_colorChangeCoroutine);
            }
            _colorChangeCoroutine = StartCoroutine(ChangeColor(pinkColor, colorFadeTime));
        } else if (numTanksP1 == numTanksP2 && _currentColor != CircleColor.White)
        {
            //set to pink
            _currentColor = CircleColor.White;
            if (_colorChangeCoroutine != null)
            {
                StopCoroutine(_colorChangeCoroutine);
            }
            _colorChangeCoroutine = StartCoroutine(ChangeColor(whiteColor, colorFadeTime));
        }
    }

    private IEnumerator ChangeColor(Color newColor, float time)
    {
        float timeElapsed = 0;
        Color startColor = mat.color;
        while (timeElapsed < time)
        {
            mat.color = Color.Lerp(startColor, newColor, timeElapsed / time);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        mat.color = newColor;
    }
}
