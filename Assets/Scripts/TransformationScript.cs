using UnityEngine;

public class TransformationScript : MonoBehaviour
{
    private float minScale = 0.7f;
    private float maxScale = 2.5f;

    void Update()
    {
        if (ObjectScript.lastDragged != null)
        {
            if (Input.GetKey(KeyCode.Z))
            {
                ObjectScript.lastDragged.GetComponent<RectTransform>().transform.Rotate(
                    0, 0, Time.deltaTime * 60f);
            }

            if (Input.GetKey(KeyCode.X))
            {
                ObjectScript.lastDragged.GetComponent<RectTransform>().transform.Rotate(
                    0, 0, -Time.deltaTime * 60f);
            }

            Vector3 currentScale = ObjectScript.lastDragged.GetComponent<RectTransform>().transform.localScale;

            if (Input.GetKey(KeyCode.UpArrow))
            {
                if (currentScale.y < maxScale)
                {
                    ObjectScript.lastDragged.GetComponent<RectTransform>().transform.localScale =
                        new Vector3(currentScale.x, currentScale.y + 0.005f, 1f);
                }
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                if (currentScale.y > minScale)
                {
                    ObjectScript.lastDragged.GetComponent<RectTransform>().transform.localScale =
                        new Vector3(currentScale.x, currentScale.y - 0.005f, 1f);
                }
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                if (currentScale.x > minScale) // Используем minScale вместо 0.3f
                {
                    ObjectScript.lastDragged.GetComponent<RectTransform>().transform.localScale =
                        new Vector3(
                        currentScale.x - 0.005f, // Используем currentScale.x
                        currentScale.y,          // Используем currentScale.y
                        1f);
                }
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                if (currentScale.x < maxScale) // Используем maxScale вместо 0.9f
                {
                    ObjectScript.lastDragged.GetComponent<RectTransform>().transform.localScale =
                        new Vector3(
                        currentScale.x + 0.005f, // Используем currentScale.x
                        currentScale.y,          // Используем currentScale.y
                        1f);
                }
            }
        }
    }
}