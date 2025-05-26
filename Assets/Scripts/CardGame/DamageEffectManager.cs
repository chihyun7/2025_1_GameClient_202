using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamageEffectManager : MonoBehaviour
{
    public static DamageEffectManager Instance { get; private set; }

    [SerializeField] private GameObject textPrefab;  // 텍스트 프리팹
    [SerializeField] private Canvas uiCanvas;        // UI 캔버스 참조

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (uiCanvas == null)
        {
            uiCanvas = FindObjectOfType<Canvas>();
            if (uiCanvas == null)
            {
                Debug.LogError("UI 캔버스를 찾을 수 없습니다.");
            }
        }
    }

    public void ShowDamageText(Vector3 position, string text, Color color, bool isCritical = false, bool isStatusEffect = false)
    {
        if (textPrefab == null || uiCanvas == null) return;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(position);
        if (screenPos.z < 0) return;

        GameObject damageText = Instantiate(textPrefab, uiCanvas.transform);
        RectTransform rectTransform = damageText.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.position = screenPos;
        }

        TextMeshProUGUI tmp = damageText.GetComponent<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.text = text;
            tmp.color = color;
            tmp.outlineColor = new Color(
                Mathf.Clamp01(color.r - 0.3f),
                Mathf.Clamp01(color.g - 0.3f),
                Mathf.Clamp01(color.b - 0.3f),
                color.a
            );

            float scale = 1.0f;
            int numbericValue;
            if (int.TryParse(text.Replace("+", "").Replace("CRIT!", "").Replace("HEAL CRIT!", ""), out numbericValue))
            {
                scale = Mathf.Clamp(numbericValue / 15f, 0.8f, 2.5f);
            }

            if (isCritical) scale = 1.4f;
            if (isStatusEffect) scale *= 0.8f;

            damageText.transform.localScale = new Vector3(scale, scale, scale);

            DamageTextEffect effect = damageText.AddComponent<DamageTextEffect>();
            if (effect != null)
            {
                effect.Initialized(isCritical, isStatusEffect);
                if (isStatusEffect)
                {
                    effect.SetVerticalMovement();
                }
            }
        }
    }

    public void ShowDamage(Vector3 position, int amount, bool isCritical = false)
    {
        string text = amount.ToString();
        Color color = isCritical ? new Color(1f, 0.8f, 0f) : new Color(1f, 0.3f, 0.3f);
        if (isCritical) text = "CRIT! " + text;
        ShowDamageText(position, text, color, isCritical);
    }

    public void ShowHeal(Vector3 position, int amount, bool isCritical = false)
    {
        string text = amount.ToString();
        Color color = isCritical ? new Color(0.4f, 1f, 0.4f) : new Color(0.3f, 0.9f, 0.3f);
        if (isCritical) text = "HEAL CRIT! " + text;
        ShowDamageText(position, text, color, isCritical);
    }

    public void ShowMiss(Vector3 position)
    {
        ShowDamageText(position, "MISS", Color.gray, false);
    }

    public void ShowStatusEffect(Vector3 position, string effectName)
    {
        Color color;

        switch (effectName.ToLower())
        {
            case "poison":
                color = new Color(0.5f, 0.1f, 0.5f); // 보라색
                break;
            case "burn":
                color = new Color(1f, 0.4f, 0f);     // 주황색
                break;
            case "freeze":
                color = new Color(0.5f, 0.8f, 1f);   // 하늘색
                break;
            case "stun":
                color = new Color(1f, 1f, 0f);       // 노란색
                break;
            default:
                color = new Color(1f, 1f, 1f);       // 기본 흰색
                break;
        }

        ShowDamageText(position, effectName.ToLower(), color, false, true);
    }
}
