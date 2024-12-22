using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleAreaProvider
{
    public RectTransform HoleArea { get; private set; }

    public HoleAreaProvider(RectTransform holeArea)
    {
        HoleArea = holeArea;
    }
}
