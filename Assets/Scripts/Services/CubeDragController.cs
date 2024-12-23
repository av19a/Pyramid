using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using Zenject;

public class CubeDragController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private ITowerService _towerService;
    private IHoleService _holeService;
    private IMessageService _messageService;
    private IAnimationService _animationService;
    private ICubeFactory _cubeFactory;
    private IGameState _gameState;
    private ICubePool _cubePool;
    
    private Canvas _canvas;
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private GameObject _draggedCopy;
    
    private bool _isDraggingTowerCube;

    [Inject]
    public void Construct(
        ITowerService towerService,
        IHoleService holeService,
        IMessageService messageService,
        IAnimationService animationService,
        ICubeFactory cubeFactory,
        IGameState gameState,
        ICubePool cubePool)
    {
        _towerService = towerService;
        _holeService = holeService;
        _messageService = messageService;
        _animationService = animationService;
        _cubeFactory = cubeFactory;
        _gameState = gameState;
        _cubePool = cubePool;
    }

    private void Awake()
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