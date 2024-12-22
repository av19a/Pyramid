using UnityEngine;
using DG.Tweening;
using System;
using System.Linq;
using Zenject;
using Random = UnityEngine.Random;

public interface IHoleService
{
    bool CanDropCube(GameObject cube);
    void DropCube(GameObject cube);
    event Action<GameObject> OnCubeDropped;
}

public class HoleService : IHoleService
{
    private readonly ICubePool _cubePool;
    
    private HoleAreaProvider _holeAreaProvider;

    public event Action<GameObject> OnCubeDropped;

    [Inject]
    public void Construct(
        HoleAreaProvider holeAreaProvider)
    {
        _holeAreaProvider = holeAreaProvider;
    }

    public bool CanDropCube(GameObject cube)
    {
        Vector3[] corners = new Vector3[4];
        cube.GetComponent<RectTransform>().GetWorldCorners(corners);
    
        foreach(Vector3 corner in corners)
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(_holeAreaProvider.HoleArea, corner))
                return false;
        }
        return true;
    }

    public void DropCube(GameObject cube)
    {
        var rectTransform = cube.GetComponent<RectTransform>();
        
        var startPosition = rectTransform.anchoredPosition;
        var endPosition = startPosition + Vector2.down * Screen.height;

        Sequence dropSequence = DOTween.Sequence();

        Vector2[] path = new Vector2[]
        {
            startPosition,
            endPosition
        };

        dropSequence.Append(
            rectTransform.DOPath(
                path.Select(p => (Vector3)p).ToArray(),
                2f,
                PathType.CatmullRom
            ).SetEase(Ease.InQuad)
        );

        dropSequence.Join(
            rectTransform.DORotate(
                new Vector3(0, 0, Random.Range(-360f, 360f)),
                2f
            ).SetEase(Ease.OutQuad)
        );

        dropSequence.Join(
            rectTransform.DOScale(
                Vector3.one * 0.8f,
                2f
            ).SetEase(Ease.InQuad)
        );

        dropSequence.OnComplete(() =>
        {
            OnCubeDropped?.Invoke(cube);
            
            rectTransform.localScale = Vector3.one;
            rectTransform.rotation = Quaternion.identity;
            
            _cubePool.Return(cube);
        });

        dropSequence.Play();
    }
}