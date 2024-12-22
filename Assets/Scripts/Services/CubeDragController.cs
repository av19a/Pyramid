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
    private ICubeFactory _cubeFactory;
    
    private Canvas _canvas;
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private GameObject _draggedCopy;

    [Inject]
    public void Construct(ITowerService towerService, ICubeFactory cubeFactory)
    {
        _towerService = towerService;
        _cubeFactory = cubeFactory;
    }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _draggedCopy = _cubeFactory.CreateDraggedCube(_canvas.transform, gameObject);
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
        _towerService.AddCube(_draggedCopy);
    }
}