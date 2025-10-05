using UnityEngine;

public class RandomSPScript : MonoBehaviour
{
    public ObjectScript objScript;
    public GameObject[] dropPlaces; 
    
    public GameObject[] spawnPoints;

    public float minScale = 1f; 
    public float maxScale = 1.5f;
    public float maxRotationDegrees = 365f; 

    void Start()
    {
        if (objScript == null || dropPlaces == null || 
            spawnPoints.Length < (objScript.vehicles.Length + dropPlaces.Length))
        {
            Debug.LogError("RandomSpawner:" + 
                           (objScript.vehicles.Length + dropPlaces.Length));
            return;
        }

        GameObject[] shuffledPoints = ShuffleArray(spawnPoints);

        for (int i = 0; i < objScript.vehicles.Length; i++)
        {
            RectTransform pointRT = shuffledPoints[i].GetComponent<RectTransform>();
            RectTransform vehicleRT = objScript.vehicles[i].GetComponent<RectTransform>();
            
            vehicleRT.anchoredPosition = pointRT.anchoredPosition;
            objScript.startCoor[i] = vehicleRT.localPosition;

            float randomScale = Random.Range(minScale, maxScale);
            vehicleRT.localScale = new Vector3(randomScale, randomScale, 1f);

            float randomRotation = Random.Range(-maxRotationDegrees, maxRotationDegrees);
            vehicleRT.localRotation = Quaternion.Euler(0f, 0f, randomRotation);
        }

        for (int i = 0; i < dropPlaces.Length; i++)
        {
            RectTransform pointRT = shuffledPoints[objScript.vehicles.Length + i].GetComponent<RectTransform>();
            RectTransform dropRT = dropPlaces[i].GetComponent<RectTransform>();
            
            dropRT.anchoredPosition = pointRT.anchoredPosition;
        }
    }

    private GameObject[] ShuffleArray(GameObject[] array)
    {
        GameObject[] shuffled = (GameObject[])array.Clone();
        int n = shuffled.Length;
        for (int i = n - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            GameObject temp = shuffled[i];
            shuffled[i] = shuffled[j];
            shuffled[j] = temp;
        }
        return shuffled;
    }
}