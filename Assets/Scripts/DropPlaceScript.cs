using UnityEngine;
using UnityEngine.EventSystems;

public class DropPlaceScript : MonoBehaviour, IDropHandler
{
    private float placeZRot, vehicleZRot, rotDiff;
    private Vector3 placeSiz, vehicleSiz;
    private float xSizeDiff, ySizeDiff;
    public ObjectScript objScript;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnDrop(PointerEventData eventData)
    {
        if ((eventData.pointerDrag != null) &&
            Input.GetMouseButtonUp(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2))
        {
            if (eventData.pointerDrag.tag.Equals(tag))
            {
                placeZRot =
                     eventData.pointerDrag.GetComponent<RectTransform>().transform.eulerAngles.z;

                vehicleZRot =
                    GetComponent<RectTransform>().transform.eulerAngles.z;

                rotDiff = Mathf.Abs(placeZRot - vehicleZRot);
                Debug.Log("Rotation difference: " + rotDiff);

                placeSiz = eventData.pointerDrag.GetComponent<RectTransform>().localScale;
                vehicleSiz = GetComponent<RectTransform>().localScale;
                xSizeDiff = Mathf.Abs(placeSiz.x - vehicleSiz.x);
                ySizeDiff = Mathf.Abs(placeSiz.y - vehicleSiz.y);
                Debug.Log("X size difference: " + xSizeDiff);
                Debug.Log("Y size difference: " + ySizeDiff);

                if ((rotDiff <= 5 || (rotDiff >= 355 && rotDiff <= 360)) &&
                    (xSizeDiff <= 0.05 && ySizeDiff <= 0.05))
                {
                    Debug.Log("Correct place");
                    objScript.rightPlace = true;
                    eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;

                    eventData.pointerDrag.GetComponent<RectTransform>().localRotation = GetComponent<RectTransform>().localRotation;
                    
                    eventData.pointerDrag.GetComponent<RectTransform>().localScale = GetComponent<RectTransform>().localScale;

                    switch (eventData.pointerDrag.tag)
                    {
                        case "Garbage":
                            objScript.effects.PlayOneShot(objScript.audioCli[2]);
                            break;

                        case "Medicine":
                            objScript.effects.PlayOneShot(objScript.audioCli[3]);
                            break;
                        case "Fire":
                            objScript.effects.PlayOneShot(objScript.audioCli[4]);
                            break;
                        case "Bus":
                            objScript.effects.PlayOneShot(objScript.audioCli[5]);
                            break;
                        case "Lamborgini":
                            objScript.effects.PlayOneShot(objScript.audioCli[6]);
                            break;
                        case "Clay":
                            objScript.effects.PlayOneShot(objScript.audioCli[7]);
                            break;
                    }

                }

            }
            else
            {
                objScript.rightPlace = false;
                objScript.effects.PlayOneShot(objScript.audioCli[1]);

                switch (eventData.pointerDrag.tag)
                {
                    case "Garbage":
                        objScript.vehicles[0].GetComponent<RectTransform>().localPosition = objScript.startCoor[0];
                        break;
                    case "Ambulance":
                        objScript.vehicles[1].GetComponent<RectTransform>().localPosition = objScript.startCoor[1];
                        break;
                    case "Fire":
                        objScript.vehicles[2].GetComponent<RectTransform>().localPosition = objScript.startCoor[2];
                        break;
                    case "Bus":
                        objScript.vehicles[3].GetComponent<RectTransform>().localPosition = objScript.startCoor[3];
                        break;
                    case "Lamborgini":
                        objScript.vehicles[4].GetComponent<RectTransform>().localPosition = objScript.startCoor[4];
                        break;
                    case "Clay":
                        objScript.vehicles[5].GetComponent<RectTransform>().localPosition = objScript.startCoor[5];
                        break;
                    default:
                        Debug.Log("Unknown tag");
                        break;
                }
            }
        }
    }

}
