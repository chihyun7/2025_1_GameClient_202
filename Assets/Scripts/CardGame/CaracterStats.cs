using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CaracterStats : MonoBehaviour
{
    public string characterName;
    public int maxHealth = 100;
    public int currentHealth;

    // UI 요소
    public Slider healthBar;
    public TextMeshProUGUI healthText;

    // 새로 추가 되는 변수
    public int maxMana = 10;            // 최대 마나
    public int currentMana;                   // 헌재마나
    public Slider ManaBar;              // 마나 바 UI
    public TextMeshProUGUI manaText;    // 마나 텍스트 UI
    // Start is called before the first frame update
    void Start()
    {
        currentMana = maxMana;
        UpdateUI();
    }

    // Update is called once per frame
   public  void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }

    public void Heal(int amount)
    {
        currentHealth += amount;    
    }

    public void UseMana(int amount)
    {
        currentMana -= amount;
        if (currentMana < 0)
        {
            currentMana = 0;
        }
        UpdateUI();
    }

    public void GainMana(int amount)
    {
        currentMana += amount;
        if (currentMana > maxMana)
        {
            currentMana = maxMana;
        }
        UpdateUI();
        }

    private void UpdateUI()
    {
        if (healthBar != null)
        {
            healthBar.value = (float)currentHealth / maxHealth;
        }

        if (healthText != null)
        {
            healthText.text = $"{currentHealth} / {maxHealth}";
        }

        if (ManaBar != null)
        {
            ManaBar.value = (float)currentMana / maxMana;
        }

        if (manaText != null)
        {
            manaText.text = $"{currentMana} / {maxMana}";
        }
    }
}
