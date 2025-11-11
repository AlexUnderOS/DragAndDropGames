using UnityEngine;

public class FlyingObjectManager : MonoBehaviour
{
    public void DestroyAllFlyingObjects()
    {
        FlyingObjectsControllerScript[] flyingObjectsControllerScripts =
            Object.FindObjectsByType<FlyingObjectsControllerScript>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach (FlyingObjectsControllerScript obj in flyingObjectsControllerScripts)
        {
            if (obj == null)
                continue;

            if (obj.CompareTag("Bomb"))
            {
                obj.TriggerExplosion();
            }
            else
            {
                obj.StartToDestroy();
            }
        }

    }
}
