using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using Zenject;

public class CubeDragController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private ITowerService _towerService;
    private Canvas _canvas;
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Vector2 _originalPosition;
    private Transform _originalParent;
    
    private Color _cubeColor;
    
    private GameObject _draggedCopy;

    [Inject]
    public void Construct(ITowerService towerService)
    {
        _towerService = towerService;
    }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Create a copy of the cube
        _draggedCopy = Instantiate(gameObject, _canvas.transform);
        _draggedCopy.transform.SetAsLastSibling();
        
        // Setup copy properties
        var copyCanvasGroup = _draggedCopy.GetComponent<CanvasGroup>();
        copyCanvasGroup.blocksRaycasts = false;

        _originalPosition = _rectTransform.anchoredPosition;
        _originalParent = transform.parent;
        
        // Move to canvas root for proper layering while dragging
        // transform.SetParent(_canvas.transform);
        // transform.SetAsLastSibling();
        
        _canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Convert screen point to local point within canvas
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out localPoint);
        _draggedCopy.GetComponent<RectTransform>().localPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;

        // Check if cube is dropped on tower area or hole
        if (IsOverTowerArea(eventData.position))
        { 
            Debug.Log("IsOverTowerArea");
            // if (_towerService.CanAddCube(_rectTransform.position)) 
            // { 
                _towerService.AddCube(gameObject);
            // }
        }
        else
        {
            Debug.Log("Destroy");
        }
    }

    private bool IsOverTowerArea(Vector2 screenPosition)
    {
        // Implementation to check if position is over tower area
        GameObject towerArea = GameObject.FindGameObjectWithTag("TowerArea");
        return RectTransformUtility.RectangleContainsScreenPoint(
            towerArea.GetComponent<RectTransform>(),
            screenPosition,
            Camera.main);
    }

    private IEnumerator PlayDestroyAnimation()
    {
        Debug.Log("Start Destroy");
        yield return new WaitForSeconds(0.5f);
        Debug.Log("End Destroy");
    }
}