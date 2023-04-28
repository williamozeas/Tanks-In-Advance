using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapSelectBox", menuName = "ScriptableObjects/MapSelectBox")] [Serializable]
public class MapSelectBox : ScriptableObject
{
    public Sprite mapSprite;
    public string mapName;
    public MapName name;
}
