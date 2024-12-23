using UnityEngine;
using DG.Tweening;
using System;
using System.Threading.Tasks;
using Zenject;

public interface IAnimationService
{
    void PlayTowerPlacementAnimation(GameObject cube, Vector2 startPos, Vector2 endPos, Action onComplete = null);
    void PlayFailedPlacementAnimation(GameObject cube, Action onComplete = null);
    void PlayHoleDropAnimation(GameObject cube, Action onComplete = null);
    void PlayCubeDropAnimation(GameObject cube, Vector2 targetPosition, Action onComplete = null);
    void StopAllAnimations(GameObject cube);
}

public class AnimationService : IAnimationService
{
    private readonly IGameConfig _gameConfig;

    public AnimationService(IGameConfig gameConfig)
    {
        _gameConfig = gameConfig;
    }

    public void PlayTowerPlacementAnimation(GameObject cube, Vector2 startPos, Vector2 endPos, Action onComplete = null)
    {
        var rectTransform = cube.GetComponent<RectTransform>();
        var bounceHeight = _gameConfig.CubeSize * 0.3f;
        
        Sequence bounceSequence = DOTween.Sequence();
        
        bounceSequence.Append(
            rectTransform.DOAnchorPos(startPos + Vector2.up * bounceHeight, 0.2f)
                .SetEase(Ease.OutQuad)
        );
        
        bounceSequence.Append(
            rectTransform.DOAnchorPos(startPos, 0.15f)
                .SetEase(Ease.InQuad)
        );
        
        bounceSequence.Join(
            rectTransform.DOScale(
                new Vector3(1.1f, 0.9f, 1f),
                0.15f
            ).SetLoops(1, LoopType.Yoyo)
        );
        
        float jumpHeight = Vector2.Distance(startPos, endPos) * 0.5f;
        Vector2 peakPos = Vector2.Lerp(startPos, endPos, 0.7f) + Vector2.up * jumpHeight;
        
        bounceSequence.Append(
            rectTransform.DOAnchorPos(peakPos, 0.25f)
                .SetEase(Ease.OutQuad)
        );
        
        bounceSequence.Append(
            rectTransform.DOAnchorPos(endPos, 0.15f)
                .SetEase(Ease.InQuad)
        );
        
        bounceSequence.Join(
            rectTransform.DOScale(
                new Vector3(1.1f, 0.9f, 1f),
                0.25f
            ).SetLoops(1, LoopType.Yoyo)
        );

        bounceSequence.OnComplete(() => 
        {
            rectTransform.localScale = Vector3.one;
            onComplete?.Invoke();
        });
    }

    public void PlayFailedPlacementAnimation(GameObject cube, Action onComplete = null)
    {
        var rectTransform = cube.GetComponent<RectTransform>();
        var cubeImage = cube.GetComponent<UnityEngine.UI.Image>();
        
        Sequence failSequence = DOTween.Sequence();

        failSequence.Append(
            rectTransform.DOShakePosition(0.3f, 20f, 20, 90f, false, true)
        );

        failSequence.Join(
            rectTransform.DOScale(Vector3.zero, 0.3f)
            .SetEase(Ease.InBack)
        );
        
        failSequence.Join(
            cubeImage.DOFade(0f, 0.3f)
            .SetEase(Ease.InQuad)
        );

        failSequence.OnComplete(() => 
        {
            rectTransform.localScale = Vector3.one;
            var color = cubeImage.color;
            color.a = 1f;
            cubeImage.color = color;
            
            onComplete?.Invoke();
        });
    }

    public void PlayHoleDropAnimation(GameObject cube, Action onComplete = null)
    {
        var rectTransform = cube.GetComponent<RectTransform>();
        var startPos = rectTransform.anchoredPosition;
        var endPos = startPos + Vector2.down * (_gameConfig.CubeSize * 3);
        
        Sequence dropSequence = DOTween.Sequence();

        dropSequence.Append(
            rectTransform.DOAnchorPos(endPos, 0.4f)
            .SetEase(Ease.InQuad)
        );

        dropSequence.Join(
            rectTransform.DORotate(
                new Vector3(0, 0, UnityEngine.Random.Range(-45f, 45f)),
                0.4f
            ).SetEase(Ease.InQuad)
        );

        dropSequence.Join(
            rectTransform.DOScale(0.8f, 0.4f)
            .SetEase(Ease.InQuad)
        );

        dropSequence.OnComplete(() => 
        {
            rectTransform.localScale = Vector3.one;
            rectTransform.rotation = Quaternion.identity;
            
            onComplete?.Invoke();
        });
    }
    
    public void PlayCubeDropAnimation(GameObject cube, Vector2 targetPosition, Action onComplete = null)
    {
        var rectTransform = cube.GetComponent<RectTransform>();
        
        rectTransform.DOAnchorPos(targetPosition, 0.3f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => onComplete?.Invoke());
    }

    public void StopAllAnimations(GameObject cube)
    {
        var rectTransform = cube.GetComponent<RectTransform>();
        rectTransform.DOKill(true); // Kill all tweens and restore original values
    }
}