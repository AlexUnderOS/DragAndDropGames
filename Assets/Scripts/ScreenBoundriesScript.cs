using Unity.VisualScripting;
using UnityEngine;

public class ScreenBoundriesScript : MonoBehaviour
{
    [HideInInspector]
    public Vector3 screenPoint, offset;
        [HideInInspector]
    public float minX, maxX, minY, maxY;
    public float padding = 0.02f;
    public Rect worldBounds = new Rect(-960,-540,1920,1080);
    public Camera targetCam;
    public float maxCamX { get; private set; }
    public float minCamX { get; private set; }
    public float minCamY { get; private set; }
    public float maxCamY { get; private set; }
    float lastOrthoSize;
    float lastAspect;
    Vector3 lastCamPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if(targetCam == null)
        {
            targetCam = Camera.main;
        }
        RecalculateBounds();
    }
    public Vector2 GetClampedPosition(Vector3 position)
    {
        float shrinkW = worldBounds.width * padding;
        float shrinkH = worldBounds.height * padding;
        float wbMinX = worldBounds.xMin + shrinkW;
        float wbMaxX = worldBounds.xMax - shrinkW;
        float wbMinY = worldBounds.yMin + shrinkH;
        float wbMaxY = worldBounds.yMax - shrinkH;
        float cx = Mathf.Clamp(position.x, wbMinX, wbMaxX);
        float cy = Mathf.Clamp(position.y, wbMinY, wbMaxY);
        return new Vector2(cx, cy);
    }
    public Vector3 GetClampedCameraPosition(Vector3 position)
    {
        float cx = Mathf.Clamp(position.x, minCamX, maxCamX);
        float cy = Mathf.Clamp(position.y, minCamY, maxCamY);
        return new Vector3(cx, cy, position.z);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (targetCam == null)
        {
            return;
        }
        bool changes = true;
        if (targetCam.orthographic)
        {
            if (!Mathf.Approximately(targetCam.orthographicSize, lastOrthoSize))
            {
                changes = true;
            }
            if (!Mathf.Approximately(targetCam.aspect, lastAspect))
            {
                changes = true;
            }
            if (targetCam.transform.position != lastCamPosition)
            {
                changes = true;
            }
            if (changes)
                RecalculateBounds();
        }
    }
    public void RecalculateBounds()
    {
        if (targetCam == null)
        {
            return;
        }
        float wbMinX = worldBounds.xMin;
        float wbMaxX = worldBounds.xMax;
        float wbMinY = worldBounds.yMin;
        float wbMaxY = worldBounds.yMax;
        if (targetCam.orthographic)
        {
            float halfH = targetCam.orthographicSize;
            float halfW = halfH * targetCam.aspect;
            if(halfW * 2f >= (wbMaxX - wbMinX))
            {
                minCamX = maxCamX = (wbMinX + wbMaxX) * .5f;
            }
            else
            {
                minCamX = wbMinX + halfW;
                maxCamX = wbMaxX - halfW;
            }

            if ((halfH * 2f >= (wbMaxY - wbMinY)))
            {
                minCamY = maxCamY = (wbMinY + wbMaxY) * .5f;
            }
            else
            {
                minCamY = wbMinY + halfH;
                maxCamY = wbMaxY - halfH;
            }
        }
        lastOrthoSize = targetCam.orthographicSize;
        lastAspect = targetCam.aspect;
        lastCamPosition = targetCam.transform.position;
    }
}