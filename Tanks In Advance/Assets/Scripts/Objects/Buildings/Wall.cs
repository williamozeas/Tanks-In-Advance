using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator OnCreate()
    {
        float animationTime = 1.0f;
        Vector3 originalPos = transform.position;
        
        transform.position += new Vector3(0, 20, 0);
        float y = transform.position.y;
        for (float timeElapsed = 0f; timeElapsed < animationTime; timeElapsed += Time.deltaTime)
        {
            y = EasingFunction.EaseOutCubic(originalPos.y + 20, originalPos.y, timeElapsed/animationTime);
            transform.position = new Vector3(originalPos.x, y, originalPos.z);

            //Debug.Log(y);
            yield return null;
        }
        AudioManager.Instance.BlockLay();
        transform.position = originalPos;
    }
}
