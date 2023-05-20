using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SetSortingOrder : MonoBehaviour
{
    public int order = 10;

    public Renderer Renderer;
    // Start is called before the first frame update
    void Start()
    {
        Renderer.sortingOrder = order;
    }
    
}
