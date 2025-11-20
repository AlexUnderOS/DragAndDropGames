using UnityEngine;

public class ObjectTornisScript : MonoBehaviour
{
    [HideInInspector] public Vector2[] startCoor;
    public Canvas can;
    public AudioSource effects;
    public AudioClip[] audioCli;
    [HideInInspector] public bool rightPlace = false;
    public static GameObject lastDragged = null;
    public static bool drag = false;
    public int placedCount = 0;


}
