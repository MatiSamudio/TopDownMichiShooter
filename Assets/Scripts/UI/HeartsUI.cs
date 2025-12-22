using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HeartsUI : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public Image heartTemplate;
    public int hpPerHeart = 20; // 100 HP / 20 = 5 corazones

    List<Image> hearts = new List<Image>();
    int maxHearts;

    void Start()
    {
        if (!playerHealth)
            playerHealth = FindFirstObjectByType<PlayerHealth>();

        heartTemplate.gameObject.SetActive(false);

        maxHearts = Mathf.CeilToInt(playerHealth.maxHealth / hpPerHeart);
        BuildHearts();
        UpdateHearts();
    }

    void Update()
    {
        UpdateHearts();
    }

    void BuildHearts()
    {
        for (int i = 0; i < maxHearts; i++)
        {
            Image heart = Instantiate(heartTemplate, heartTemplate.transform.parent);
            heart.gameObject.SetActive(true);
            hearts.Add(heart);
        }
    }

    void UpdateHearts()
    {
        float hp = playerHealth.CurrentHealth;

        for (int i = 0; i < hearts.Count; i++)
        {
            float heartMin = i * hpPerHeart;
            float heartMax = (i + 1) * hpPerHeart;

            hearts[i].color = (hp > heartMin) ? Color.white : new Color(1, 1, 1, 0.2f);
        }
    }
}
