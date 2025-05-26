using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamageTextEffect : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 100f;  // UI용으로 더 큰 값
    [SerializeField] private float lifeTime = 1.5f;   // 텍스트 생명 주기

    private TextMeshProUGUI textMesh;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Color originalColor;
    private Vector2 moveDirection;
    private float timer = 0f;

    private bool isCritical = false;
    private bool isStatusEffect = false;
    private bool useGravity = true;
    private float verticalVelocity = 100f;

    // 초기화 함수
    public void Initialized(bool critical, bool statusEffect)
    {
        isCritical = critical;
        isStatusEffect = statusEffect;

        if (isStatusEffect)
        {
            useGravity = false;
        }

        Start(); // 초기화 즉시 시작
    }

    // Start 함수
    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null && textMesh != null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        if (textMesh != null)
        {
            originalColor = textMesh.color;
            float randomX = Random.Range(-0.5f, 0.5f);
            float randomY = useGravity ? Random.Range(0.5f, 1.0f) : Random.Range(0.8f, 1.5f);
            moveDirection = new Vector2(randomX, randomY).normalized;

            if (rectTransform != null)
            {
                rectTransform.rotation = Quaternion.Euler(0, 0, Random.Range(-10f, 10f));
            }

            if (useGravity)
            {
                verticalVelocity = Random.Range(100f, 200f);
            }

            StartCoroutine(PunchScale(isCritical ? 1.5f : 1.2f));

            if (isCritical)
            {
                StartCoroutine(FlashText());
                StartCoroutine(CreateFlashEffect());
            }
        }
    }

    // Update 함수
    void Update()
    {
        if (rectTransform == null) return;

        if (useGravity)
        {
            verticalVelocity -= 300f * Time.deltaTime;
            rectTransform.anchoredPosition += new Vector2(0, verticalVelocity * Time.deltaTime);
            rectTransform.anchoredPosition += new Vector2(moveDirection.x * moveSpeed * Time.deltaTime, 0);
        }
        else
        {
            rectTransform.anchoredPosition += new Vector2(moveDirection.x, moveDirection.y) * moveSpeed * Time.deltaTime;
        }

        timer += Time.deltaTime;

        if (timer >= lifeTime * 0.5f)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, (timer - lifeTime * 0.5f) / (lifeTime * 0.5f));
            }
            else if (textMesh != null)
            {
                float alpha = Mathf.Lerp(1f, 0f, (timer - lifeTime * 0.5f) / (lifeTime * 0.5f));
                textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            }

            moveSpeed = Mathf.Lerp(moveSpeed, 20f, Time.deltaTime * 2f);
        }

        if ((canvasGroup != null && canvasGroup.alpha <= 0.05f) || (textMesh != null && textMesh.color.a <= 0.05f))
        {
            Destroy(gameObject);
        }
    }

    // 수직 이동 함수
    public void SetVerticalMovement()
    {
        useGravity = false;
    }

    // 스케일 효과 함수
    private IEnumerator PunchScale(float intensity)
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * intensity;

        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }

        transform.localScale = originalScale;
    }

    // 텍스트 깜빡임 효과
    private IEnumerator FlashText()
    {
        if (textMesh == null) yield break;

        Color flashColor = Color.white;
        float flashDuration = 0.2f;

        Color startColor = textMesh.color;
        textMesh.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        textMesh.color = startColor;
    }

    // 텍스트 깜빡임 효과 (알파 값 조정)
    private IEnumerator CreateFlashEffect()
    {
        if (textMesh == null) yield break;

        float interval = 0.05f;
        int flashCount = 3;

        for (int i = 0; i < flashCount; i++)
        {
            textMesh.alpha = 0.5f;
            yield return new WaitForSeconds(interval);
            textMesh.alpha = 1.0f;
            yield return new WaitForSeconds(interval);
        }
    }
}
