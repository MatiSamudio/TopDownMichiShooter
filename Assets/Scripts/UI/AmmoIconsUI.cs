using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmmoIconsUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Image bulletTemplate;
    [SerializeField] TMP_Text reloadingText;

    [Header("Dim Settings")]
    [Range(0f, 1f)][SerializeField] float spentAlpha = 0.25f;

    readonly List<Image> bullets = new List<Image>();
    int builtSize = -1;

    void Awake()
    {
        if (bulletTemplate) bulletTemplate.gameObject.SetActive(false);
        if (reloadingText) reloadingText.gameObject.SetActive(false);
    }

    public void BuildIfNeeded(int magSize)
    {
        if (!bulletTemplate) return;
        if (magSize == builtSize) return;

        for (int i = bullets.Count - 1; i >= 0; i--)
            if (bullets[i]) Destroy(bullets[i].gameObject);

        // bullets.Clear();

        for (int i = 0; i < magSize; i++)
        {
            var icon = Instantiate(bulletTemplate, bulletTemplate.transform.parent);
            icon.gameObject.name = $"Bullet_{i + 1}";
            icon.gameObject.SetActive(true);          // siempre activo
            icon.color = Color.white;                  // estado inicial
            bullets.Add(icon);
        }

        builtSize = magSize;
    }

    public void SetAmmo(int current, int magSize)
    {
        BuildIfNeeded(magSize);

        current = Mathf.Clamp(current, 0, magSize);

        for (int i = 0; i < bullets.Count; i++)
        {
            var img = bullets[i];
            if (!img) continue;

            img.color = (i < current)
                ? Color.white
                : new Color(1f, 1f, 1f, spentAlpha);
        }
    }

    public void SetReloading(bool active)
    {
        if (reloadingText) reloadingText.gameObject.SetActive(active);
    }
}
