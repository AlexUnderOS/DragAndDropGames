using UnityEngine;
using UnityEngine.EventSystems;

public class DropPlaceScript : MonoBehaviour, IDropHandler
{
    private float placeZRot, vehicleZRot, rotDiff;
    private Vector3 placeSiz, vehicleSiz;
    private float xSizeDiff, ySizeDiff;
    public ObjectScript objScript;

    private void Start()
    {
        if (objScript == null)
        {
            objScript = Object.FindFirstObjectByType<ObjectScript>();
        }
    }
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        if (eventData.pointerDrag != null) {

            if (!eventData.pointerDrag.tag.Equals(tag))
            {
                Debug.Log("Wrong tag. Expected: " + tag + ", Got: " + eventData.pointerDrag.tag);
                objScript.rightPlace = false;
                objScript.effects.PlayOneShot(objScript.audioCli[1]);
                ResetVehiclePosition(eventData.pointerDrag.tag);
                return;
            }

            placeZRot = eventData.pointerDrag.GetComponent<RectTransform>().transform.eulerAngles.z;
            vehicleZRot = GetComponent<RectTransform>().transform.eulerAngles.z;
            rotDiff = Mathf.Abs(placeZRot - vehicleZRot);
            Debug.Log("Rotation difference: " + rotDiff);

            placeSiz = eventData.pointerDrag.GetComponent<RectTransform>().localScale;
            vehicleSiz = GetComponent<RectTransform>().localScale;
            xSizeDiff = Mathf.Abs(placeSiz.x - vehicleSiz.x);
            ySizeDiff = Mathf.Abs(placeSiz.y - vehicleSiz.y);
            Debug.Log("X size difference: " + xSizeDiff);
            Debug.Log("Y size difference: " + ySizeDiff);

            if ((rotDiff <= 5 || (rotDiff >= 355 && rotDiff <= 360)) &&
                xSizeDiff <= 0.1 && ySizeDiff <= 0.1)
            {
                Debug.Log("Correct place - vehicle placed successfully");
                objScript.rightPlace = true;
                eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
                eventData.pointerDrag.GetComponent<RectTransform>().localRotation = GetComponent<RectTransform>().localRotation;
                eventData.pointerDrag.GetComponent<RectTransform>().localScale = GetComponent<RectTransform>().localScale;

                objScript.placedCount++; 

                PlayCorrectSound(eventData.pointerDrag.tag);
            }
            else
            {
                Debug.Log("Right tag but wrong rotation/scale. Vehicle stays where user placed it.");
                objScript.rightPlace = false;
            }
        }
    }

    private void PlayCorrectSound(string tag)
    {
        switch (tag)
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
            case "Spoon":
                objScript.effects.PlayOneShot(objScript.audioCli[10]);
                break;
            case "Farmer":
                objScript.effects.PlayOneShot(objScript.audioCli[11]);
                break;
            case "Police":
                objScript.effects.PlayOneShot(objScript.audioCli[12]);
                break;
            case "Ferrari":
                objScript.effects.PlayOneShot(objScript.audioCli[13]);
                break;
            case "KillerMashine":
                objScript.effects.PlayOneShot(objScript.audioCli[14]);
                break;
            case "Mazda":
                objScript.effects.PlayOneShot(objScript.audioCli[15]);
                break;
        }
    }

    private void ResetVehiclePosition(string tag)
    {
        switch (tag)
        {
            case "Garbage":
                objScript.vehicles[0].GetComponent<RectTransform>().localPosition = objScript.startCoor[0];
                break;
            case "Medicine":
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
            case "Spoon":
                objScript.vehicles[6].GetComponent<RectTransform>().localPosition = objScript.startCoor[6];
                break;
            case "Farmer":
                objScript.vehicles[7].GetComponent<RectTransform>().localPosition = objScript.startCoor[7];
                break;
            case "Police":
                objScript.vehicles[8].GetComponent<RectTransform>().localPosition = objScript.startCoor[8];
                break;
            case "Ferrari":
                objScript.vehicles[9].GetComponent<RectTransform>().localPosition = objScript.startCoor[9];
                break;
            case "KillerMashine":
                objScript.vehicles[10].GetComponent<RectTransform>().localPosition = objScript.startCoor[10];
                break;
            case "Mazda":
                objScript.vehicles[11].GetComponent<RectTransform>().localPosition = objScript.startCoor[11];
                break;
            default:
                Debug.Log("Unknown tag: " + tag);
                break;
        }
    }
}