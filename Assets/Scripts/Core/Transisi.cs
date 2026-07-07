using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Transisi FADE hitam antar scene. Dibuat otomatis sekali & bertahan (DontDestroyOnLoad).
// Ganti scene lewat Transisi.Pindah("NamaScene") supaya ada efek gelap-terang mulus.
public class Transisi : MonoBehaviour
{
    static Transisi instance;
    Image hitam;
    Coroutine kerja;
    const float LAMA = 0.35f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Buat()
    {
        if (instance != null) return;
        GameObject go = new GameObject("Transisi");
        DontDestroyOnLoad(go);
        instance = go.AddComponent<Transisi>();
        instance.Setup();
    }

    void Setup()
    {
        GameObject cv = new GameObject("Kanvas");
        cv.transform.SetParent(transform, false);
        Canvas c = cv.AddComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        c.sortingOrder = 999;

        GameObject im = new GameObject("Hitam");
        im.transform.SetParent(cv.transform, false);
        hitam = im.AddComponent<Image>();
        hitam.color = Color.black;
        hitam.raycastTarget = false;
        RectTransform rt = hitam.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        SceneManager.sceneLoaded += OnSceneLoaded;
        MulaiFade(FadeMasuk());
    }

    void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        MulaiFade(FadeMasuk());
    }

    void MulaiFade(IEnumerator rutin)
    {
        if (kerja != null) StopCoroutine(kerja);
        kerja = StartCoroutine(rutin);
    }

    // dari hitam -> jernih (awal scene)
    IEnumerator FadeMasuk()
    {
        float t = 0f;
        while (t < LAMA)
        {
            t += Time.unscaledDeltaTime;
            SetAlpha(1f - t / LAMA);
            yield return null;
        }
        SetAlpha(0f);
    }

    // jernih -> hitam lalu load scene
    IEnumerator FadeKeluar(string scene)
    {
        Time.timeScale = 1f;
        float t = 0f;
        while (t < LAMA)
        {
            t += Time.unscaledDeltaTime;
            SetAlpha(t / LAMA);
            yield return null;
        }
        SetAlpha(1f);
        SceneManager.LoadScene(scene);
    }

    void SetAlpha(float a)
    {
        if (hitam == null) return;
        a = Mathf.Clamp01(a);
        hitam.color = new Color(0f, 0f, 0f, a);
    }

    public static void Pindah(string scene)
    {
        if (instance == null || instance.hitam == null)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(scene);
            return;
        }
        instance.MulaiFade(instance.FadeKeluar(scene));
    }
}
