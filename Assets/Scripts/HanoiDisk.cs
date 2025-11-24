using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HanoiDisk : MonoBehaviour
{
    public int sizeIndex = 0;

    private bool isDragging = false;
    private Vector3 dragOffset;
    private float dragZ;

    private void OnMouseDown()
    {
        if (HanoiGameManager.Instance == null ||
            !HanoiGameManager.Instance.gameRunning)
            return;

        int pegIndex;
        if (!HanoiGameManager.Instance.IsTopDisk(this, out pegIndex))
            return;

        isDragging = true;

        dragZ = transform.position.z;
        var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = dragZ;
        dragOffset = transform.position - worldPos;

        transform.position = new Vector3(transform.position.x, transform.position.y, dragZ - 0.1f);
    }

    private void OnMouseDrag()
    {
        if (!isDragging)
            return;

        var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = dragZ;
        transform.position = worldPos + dragOffset;
    }

    private void OnMouseUp()
    {
        if (!isDragging)
            return;

        isDragging = false;
        transform.position = new Vector3(transform.position.x, transform.position.y, dragZ);

        if (HanoiGameManager.Instance != null)
        {
            HanoiGameManager.Instance.TryDropDisk(this, transform.position);
        }
    }
}
