// ...existing code...
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using GLTFast.Schema;

public class EndGame : MonoBehaviour
{
    public RawImage fadeImage;
    public float fadeDuration = 1.5f;
    public bool fadeOnStart = false;
    public bool blockRaycastsWhileFading = true;
    // ...existing code...

    void Start()
    {
        if (fadeOnStart && fadeImage != null)
            StartCoroutine(FadeToAlpha(1f, fadeDuration, "BadEnding"));
    }

    // chiamare per avviare la dissolvenza verso nero e (opzionalmente) caricare una scena
    public void FadeToBlack(string sceneName = null)
    {
        if (fadeImage == null) return;
        StartCoroutine(FadeToAlpha(1f, fadeDuration, sceneName));
    }

    // chiamare per rimuovere il nero (fade out)
    public void FadeFromBlack()
    {
        if (fadeImage == null) return;
        StartCoroutine(FadeToAlpha(0f, fadeDuration, null));
    }

    // ora accetta un nome di scena opzionale: se non null la carica al termine della dissolvenza
    IEnumerator FadeToAlpha(float targetAlpha, float duration, string sceneToLoad = null)
    {
        if (fadeImage == null) yield break;
        if (!fadeImage.gameObject.activeInHierarchy) fadeImage.gameObject.SetActive(true);

        Color startColor = fadeImage.color;
        float startAlpha = startColor.a;
        float t = 0f;

        if (blockRaycastsWhileFading) fadeImage.raycastTarget = true;

        float safeDuration = Mathf.Max(0.0001f, duration);
        while (t < safeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(startAlpha, targetAlpha, t / safeDuration);
            fadeImage.color = new Color(startColor.r, startColor.g, startColor.b, a);
            yield return null;
        }

        fadeImage.color = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);

        if (!blockRaycastsWhileFading) fadeImage.raycastTarget = false;
        if (Mathf.Approximately(targetAlpha, 0f)) fadeImage.gameObject.SetActive(false);

        // se è passata una scena, caricala (async)
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            var op = SceneManager.LoadSceneAsync(sceneToLoad);
            // opzionale: attendi il caricamento completo
            while (!op.isDone) yield return null;
        }
    }
}