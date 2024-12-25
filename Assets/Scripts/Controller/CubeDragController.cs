using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using Zenject;

public class CubeDragController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Inject] private ITowerService _towerService;
    [Inject] private IHoleService _holeService;
    [Inject] private IMessageService _messageService;
    [Inject] private IAnimationService _animationService;
    [Inject] private ICubeFactory _cubeFactory;
    [Inject] private IGameState _gameState;
    [Inject] private ICubePool _cubePool;
    
    private Canvas _canvas;
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private GameObject _draggedCopy;
    private bool _isDraggingTowerCube;

    [Inject]
    private void Initialize()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _isDraggingTowerCube = _gameState.TowerCubes.Contains(gameObject);
        
        if (_isDraggingTowerCube)
        {
            _draggedCopy = gameObject;
            _towerService.RemoveCube(_draggedCopy);
        }
        else
        {
            _draggedCopy = _cubeFactory.CreateDraggedCube(_canvas.transform, gameObject);
        }
        
        _canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.GetComponent<RectTransform>(),
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint))
        {
            _draggedCopy.GetComponent<RectTransform>().localPosition = localPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;

        if (_towerService.CanAddCube(_draggedCopy))
        {
            _towerService.AddCube(_draggedCopy);
            _messageService.ShowMessage("cube_added");
        }
        else if (_holeService.CanDropCube(_draggedCopy))
        {
            _holeService.DropCube(_draggedCopy);
            _messageService.ShowMessage("cube_dropped");
        }
        else
        {
            _animationService.PlayFailedPlacementAnimation(_draggedCopy, () =>
            {
                _cubePool.Return(_draggedCopy);
            });
            _messageService.ShowMessage("cube_destroyed");
        }
    }
}