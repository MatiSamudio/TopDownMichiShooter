using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] GameObject root;
    [SerializeField] Image fillImage; // sprite central

    public void Show(string text)
    {
        root.SetActive(true);
        SetProgress(0f);
    }

    public void Hide()
    {
        root.SetActive(false);
    }

    public void SetProgress(float t)
    {
        fillImage.fillAmount = Mathf.Clamp01(t);
    }
}
