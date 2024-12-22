using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CubeView : MonoBehaviour
{
    private Image _image;
    public Color Color { get; private set; }
    public int OriginalId { get; private set; }

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    public void Initialize(Color color, int originalId = 0)
    {
        Color = color;
        OriginalId = originalId;
        _image.color = color;
    }
}
