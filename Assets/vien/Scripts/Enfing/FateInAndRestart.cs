// ...existing code...
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class FateInAndRestart : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI textToFade;

    [Header("Timing")]
    public float fadeDuration = 1.5f;        // durata della dissolvenza in secondi
    public float waitBeforeRestart = 3f;     // tempo da aspettare dopo la dissolvenza

    [Header("Scene")]
    public bool loadByName = true;
    public string sceneName = "";            // se loadByName = true
    public int sceneIndex = 0;               // se loadByName = false

    [Header("Behaviour")]
    public bool autoStart = true;            // avvia automaticamente in Start

    void Start()
    {
        if (autoStart) StartFadeSequence();
    }

    // chiamare da altri script per avviare la sequenza
    public void StartFadeSequence()
    {
        if (textToFade == null)
        {
            Debug.LogWarning("FateInAndRestart: textToFade non assegnato.");
            // comunque proviamo a procedere con restart dopo wait
            StartCoroutine(WaitAndLoad(waitBeforeRestart));
            return;
        }

        // assicuriamoci che abbia alpha iniziale 0
        Color c = textToFade.color;
        textToFade.color = new Color(c.r, c.g, c.b, 0f);
        textToFade.gameObject.SetActive(true);

        StartCoroutine(FadeInAndRestartCoroutine());
    }

    IEnumerator FadeInAndRestartCoroutine()
    {
        float safeFade = Mathf.Max(0.0001f, fadeDuration);
        float t = 0f;
        Color startColor = textToFade.color;
        while (t < safeFade)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(0f, 1f, t / safeFade);
            textToFade.color = new Color(startColor.r, startColor.g, startColor.b, a);
            yield return null;
        }
        textToFade.color = new Color(startColor.r, startColor.g, startColor.b, 1f);

        yield return new WaitForSeconds(Mathf.Max(0f, waitBeforeRestart));

        // carica la scena desiderata
        if (loadByName)
        {
            if (!string.IsNullOrEmpty(sceneName))
                SceneManager.LoadScene(sceneName);
            else
                Debug.LogWarning("FateInAndRestart: sceneName vuoto, nessuna scena caricata.");
                Application.Quit();
        }
        else
        {
            SceneManager.LoadScene(sceneIndex);
        }
    }

    IEnumerator WaitAndLoad(float delay)
    {
        yield return new WaitForSeconds(Mathf.Max(0f, delay));
        if (loadByName && !string.IsNullOrEmpty(sceneName)) SceneManager.LoadScene(sceneName);
        else SceneManager.LoadScene(sceneIndex);
    }
}