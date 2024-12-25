using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAreaProvider
{
    public RectTransform TowerArea { get; private set; }

    public TowerAreaProvider(RectTransform towerArea)
    {
        TowerArea = towerArea;
    }
}
