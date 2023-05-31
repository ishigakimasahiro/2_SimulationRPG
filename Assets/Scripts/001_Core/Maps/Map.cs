using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public List<TileObj> maps = new List<TileObj>();

    void Start()
    {
        GetComponentsInChildren(maps);
    }
}
