using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CubeView : MonoBehaviour
{
    private RectTransform _rectTransform;
    private Image _image;
    private CubeDragController _dragController;
    // private CubeAnimationController _animController;

    [Inject]
    public void Construct(Color color)
    {
        _rectTransform = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
        _dragController = GetComponent<CubeDragController>();
        // _animController = GetComponent<CubeAnimationController>();
        
        _image.color = color;
        // SetupCube();
    }

    // private void SetupCube()
    // {
    //     // Add shine effect
    //     var shine = new GameObject("Shine").AddComponent<Image>();
    //     shine.transform.SetParent(transform);
    //     shine.rectTransform.anchorMin = Vector2.zero;
    //     shine.rectTransform.anchorMax = Vector2.one;
    //     shine.rectTransform.offsetMin = Vector2.zero;
    //     shine.rectTransform.offsetMax = Vector2.zero;
    //     
    //     // Set shine gradient
    //     shine.material = new Material(Shader.Find("UI/Default"));
    //     shine.material.SetFloat("_Angle", 45f);
    //     shine.color = new Color(1, 1, 1, 0.2f);
    // }
}
