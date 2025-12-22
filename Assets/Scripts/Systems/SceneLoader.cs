using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    static SceneLoader instance;
    LoadingScreen loading;

    [Header("Loading Timing")]
    [SerializeField] float minLoadTime = 3.0f; // segundos fijos

    void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        loading = FindFirstObjectByType<LoadingScreen>();
    }

    public static void Load(string sceneName)
    {
        Ensure();
        instance.StartCoroutine(instance.LoadRoutine(sceneName));
    }

    static void Ensure()
    {
        if (instance != null) return;
        var go = new GameObject("SceneLoader");
        instance = go.AddComponent<SceneLoader>();
    }

    IEnumerator LoadRoutine(string sceneName)
    {
        if (loading == null)
            loading = FindFirstObjectByType<LoadingScreen>();

        loading?.Show("Loading");
        Time.timeScale = 1f;

        float elapsed = 0f;

        var op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (elapsed < minLoadTime)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / minLoadTime;
            loading?.SetProgress(t);
            yield return null;
        }

        // Espera a que la escena termine de cargar realmente
        while (op.progress < 0.9f)
            yield return null;

        loading?.SetProgress(1f);
        op.allowSceneActivation = true;

        yield return null;

        loading = FindFirstObjectByType<LoadingScreen>();
        loading?.Hide();
    }
}
