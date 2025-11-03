using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FlyingObjectsControllerScript : MonoBehaviour
{
    [HideInInspector]
    public float speed = 1f;
    public float fadeDuration = 1.5f;
    public float waveAmplitude = 25f;
    public float waveFrequency = 1f;
    
    private ObjectScript objectScript;
    private ScreenBoundriesScript screenBoundriesScript;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private bool isFadingOut = false;
    private bool isExploding = false;
    private Image image;
    private Color originalColor;
    private Vector2 basePosition;
    private float startTime;
    
    private bool isPaused = false;
    private float savedSpeed;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        rectTransform = GetComponent<RectTransform>();
        basePosition = rectTransform.anchoredPosition;
        startTime = Time.time;

        image = GetComponent<Image>();
        if (image != null)
        {
            originalColor = image.color;
        }

        objectScript = FindFirstObjectByType<ObjectScript>();
        screenBoundriesScript = FindFirstObjectByType<ScreenBoundriesScript>();
        StartCoroutine(FadeIn());
    }

    void Update()
    {
        if (isPaused) return;
        
        float horizontalMovement = -speed * Time.deltaTime;
        float elapsedTime = Time.time - startTime;
        float waveOffset = Mathf.Sin(elapsedTime * waveFrequency) * waveAmplitude;
        
        Vector2 currentPos = rectTransform.anchoredPosition;
        rectTransform.anchoredPosition = new Vector2(currentPos.x + horizontalMovement, basePosition.y + waveOffset);

        if (speed > 0)
        {
            if (rectTransform.anchoredPosition.x < screenBoundriesScript.minX - 4371f && !isFadingOut)
            {
                StartCoroutine(FadeOutAndDestroy());
                isFadingOut = true;
            }
        }
        else 
        {
            if (rectTransform.anchoredPosition.x > screenBoundriesScript.maxX && !isFadingOut)
            {
                StartCoroutine(FadeOutAndDestroy());
                isFadingOut = true;
            }
        }

        Vector2 inputPos;
        if (!TryGetInputPosition(out inputPos)) return;

        if (CompareTag("Bomb") && !isExploding && Input.GetMouseButtonDown(0))
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, inputPos, Camera.main))
            {
                Debug.Log("Bomb clicked!");
                TriggerExplosion();
            }
        }

        if (ObjectScript.drag && !isFadingOut && !isExploding &&
            RectTransformUtility.RectangleContainsScreenPoint(rectTransform, inputPos, Camera.main))
        {
            Debug.Log("The cursor collided with a flying object!");

            if (ObjectScript.lastDragged != null)
            {
                StartCoroutine(ShrinkAndDestroy(ObjectScript.lastDragged, 0.5f));
                ObjectScript.lastDragged = null;
                ObjectScript.drag = false;
                
                StartCoroutine(ReloadSceneAfterDelay(1f));
            }

            StartToDestroy();
        }
    }

    public bool TryGetInputPosition(out Vector2 position)
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        position = Input.mousePosition;
        return true;

#elif UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            position = Input.GetTouch(0).position;
            return true;
        }
        else
        {
            position = Vector2.zero;
            return false;
        }
#endif
    }
    public void StopAllMovement()
    {
        if (!isPaused)
        {
            isPaused = true;
            savedSpeed = speed;
            speed = 0f;
            StopAllCoroutines();
        }
    }

    public void ResumeMovement()
    {
        if (isPaused)
        {
            isPaused = false;
            speed = savedSpeed;
            if (!isFadingOut && !isExploding)
            {
                StartCoroutine(FadeIn());
            }
        }
    }

    IEnumerator ReloadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void TriggerExplosion()
    {
        if (isExploding) return;
        
        isExploding = true;
        if (objectScript != null && objectScript.effects != null)
        {
            objectScript.effects.PlayOneShot(objectScript.audioCli[9], 5f);
        }

        if (TryGetComponent<Animator>(out Animator animator))
        {
            animator.SetBool("explode", true);
        }

        if (image != null)
        {
            image.color = Color.red;
            StartCoroutine(RecoverColor(0.4f));
        }

        StartCoroutine(Vibrate());
        
        float radius = 200f; 
        ExplodeAndDestroy(radius);
        
        StartCoroutine(DestroyAfterExplosion(1f));
    }

    IEnumerator DestroyAfterExplosion(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    void ExplodeAndDestroy(float radius)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider != null && hitCollider.gameObject != gameObject)
            {
                FlyingObjectsControllerScript obj = 
                    hitCollider.gameObject.GetComponent<FlyingObjectsControllerScript>();
                
                if (obj != null && !obj.isExploding && !obj.CompareTag("Bomb"))
                {
                    obj.StartToDestroy();
                }
            }
        }
    }

    public void StartToDestroy()
    {
        if (!isFadingOut && !isExploding) {
            if (CompareTag("Bomb") && !isExploding)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, Camera.main))
                {
                    TriggerExplosion();
                    StartCoroutine(ReloadSceneAfterDelay(1f));
                }
            }
            else
            {
                StartCoroutine(FadeOutAndDestroy());
                isFadingOut = true;

                if (image != null)
                {
                    image.color = Color.cyan;
                    StartCoroutine(RecoverColor(0.5f));
                }

                if (objectScript != null && objectScript.effects != null)
                {
                    objectScript.effects.PlayOneShot(objectScript.audioCli[8]);
                }

                StartCoroutine(Vibrate());
            }
        }
    }

    IEnumerator Vibrate()
    {
#if UNITY_ANDROID
        Handheld.Vibrate();
#endif
        Vector2 originalPosition = rectTransform.anchoredPosition;
        float duration = 0.3f;
        float elapsed = 0f;
        float intensity = 5f;   

        while (elapsed < duration)
        {
            rectTransform.anchoredPosition = 
                originalPosition + Random.insideUnitCircle * intensity;
            elapsed += Time.deltaTime;
            yield return null;
        }
        rectTransform.anchoredPosition = originalPosition;
    }

    IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    IEnumerator FadeOutAndDestroy()
    {
        float t = 0f;
        float startAlpha = canvasGroup.alpha;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        Destroy(gameObject);
    }

    IEnumerator ShrinkAndDestroy(GameObject target, float duration)
    {
        if (target == null) yield break;

        Vector3 originalScale = target.transform.localScale;
        Quaternion originalRotation = target.transform.rotation;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            target.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t / duration);
            float angle = Mathf.Lerp(0f, 360f, t / duration);
            target.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            yield return null;
        }

        Destroy(target);
    }

    IEnumerator RecoverColor(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (image != null)
        {
            image.color = originalColor;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 200f);
    }
}