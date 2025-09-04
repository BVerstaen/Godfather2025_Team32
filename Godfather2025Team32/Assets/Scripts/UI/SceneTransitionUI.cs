using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class SceneTransitionUI : MonoBehaviour
{
public static SceneTransitionUI Instance;

    [Header("UI Elements")]
    public CanvasGroup loadingScreen;
    public TextMeshProUGUI loadingText;

    [Header("Settings")]
    public float fadeDuration = 0.5f;
    public float holdDuration = 3f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        loadingScreen.alpha = 0f;
        loadingScreen.gameObject.SetActive(false);
    }

    public void LoadSceneWithTransition(string sceneName)
    {
        StartCoroutine(Transition(sceneName));
    }

    private IEnumerator Transition(string sceneName)
    {
        loadingScreen.gameObject.SetActive(true);

        yield return
            Fade(0f, 1f);

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            float progress = Mathf.Clamp01(op.progress / 0.9f);
            if (loadingText)
                loadingText.text = "Loading... " + (progress * 100f).ToString("F0") + "%";

            yield return null;
        }

        if (loadingText)
            loadingText.text = "Loading... 100%";

        yield return 
            new WaitForSeconds(holdDuration);

        op.allowSceneActivation = true;

        yield return
            null;

        yield return 
            Fade(1f, 0f);

        loadingScreen.gameObject.SetActive(false);
    }

    private IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            loadingScreen.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }
        
        loadingScreen.alpha = to;
    }
}
