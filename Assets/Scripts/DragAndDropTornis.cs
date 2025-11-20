using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropTornis : MonoBehaviour, IPointerDownHandler, IBeginDragHandler,
    IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGro;
    private RectTransform rectTra;
    public ObjectScript objectScr;
    public ScreenBoundriesScript screenBou;

    private Vector3 dragOffsetWorld;
    private Camera uiCamera;
    private Canvas canvas;


    void Awake()
    {
        canvasGro = GetComponent<CanvasGroup>();
        rectTra = GetComponent<RectTransform>();

        if (objectScr == null)
        {
            objectScr = Object.FindFirstObjectByType<ObjectScript>();
        }

        if (screenBou == null)
        {
            screenBou = Object.FindFirstObjectByType<ScreenBoundriesScript>();
        }

        canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            uiCamera = canvas.worldCamera;

        }
        else
        {
            Debug.LogError("Canvas not found for DragAndDropScript");
        }
    }



    // CHANGES FOR ANDROID
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
        objectScr.effects.PlayOneShot(objectScr.audioCli[0]);

    }

    // CHANGES FOR ANDROID
    public void OnBeginDrag(PointerEventData eventData)
    {
        ObjectScript.drag = true;
        ObjectScript.lastDragged = eventData.pointerDrag;
        canvasGro.blocksRaycasts = false;
        canvasGro.alpha = 0.6f;

        transform.SetAsLastSibling();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTra.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        );

        rectTra.localPosition = localPoint;
    }



    // CHANGES FOR ANDROID
    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTra.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint))
        {
            rectTra.localPosition = localPoint;
        }
    }

    // CHANGES FOR ANDROID
    public void OnEndDrag(PointerEventData eventData)
    {
        objectScr.effects.PlayOneShot(objectScr.audioCli[0]);
        ObjectScript.drag = false;
        canvasGro.blocksRaycasts = true;
        canvasGro.alpha = 1.0f;

        if (objectScr.rightPlace)
        {
            canvasGro.blocksRaycasts = false;
            ObjectScript.lastDragged = null;
        }

        objectScr.rightPlace = false;
    }
}
