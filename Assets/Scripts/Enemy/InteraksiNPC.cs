using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

// INTERAKSI NPC KECIL: saat pemain dekat, NPC menyapa (teks di bawah layar).
// Tekan E untuk menjawab -> NPC membalas + bonus skor (sekali per NPC). Play-only.
// UI dibagi (satu bubble untuk semua NPC); hanya NPC terdekat yang tampil.
public class InteraksiNPC : MonoBehaviour
{
    public float jarak = 6f;
    string sapa, balas;
    bool sudahJawab = false;
    Transform player;

    static GameObject uiRoot;
    static Text uiTeks;
    static InteraksiNPC pemakai;

    static readonly string[] sapaan = {
        "Hati-hati, Kurir!", "Wah, paketmu penting ya?", "Semangat antar paketnya!",
        "Awas jangan nabrak aku!", "Ayo cepat, waktumu mepet!", "Permisi, aku mau lewat!"
    };
    static readonly string[] balasan = {
        "Siap, terima kasih!", "Doakan sampai finish ya!", "Pasti tepat waktu!",
        "Tenang, aku hati-hati!", "Semangat juga untukmu!"
    };

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
        sapa = sapaan[Random.Range(0, sapaan.Length)];
        balas = balasan[Random.Range(0, balasan.Length)];
        PastikanUI();
    }

    void Update()
    {
        if (!Application.isPlaying || player == null) return;
        float d = Vector3.Distance(transform.position, player.position);

        if (d <= jarak)
        {
            bool boleh = pemakai == null || pemakai == this ||
                         Vector3.Distance(pemakai.transform.position, player.position) > d;
            if (boleh)
            {
                pemakai = this;
                if (!sudahJawab && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
                {
                    sudahJawab = true;
                    if (RunGame.instance != null) RunGame.instance.TambahSkor(25);
                }
                Tampil(sudahJawab
                    ? "NPC: " + balas + "   (+25 skor)"
                    : "NPC: " + sapa + "      [Tekan E untuk menjawab]");
            }
        }
        else if (pemakai == this)
        {
            pemakai = null;
            Sembunyi();
        }
    }

    void Tampil(string t)
    {
        PastikanUI();
        if (uiRoot != null) uiRoot.SetActive(true);
        if (uiTeks != null) uiTeks.text = t;
    }

    void Sembunyi()
    {
        if (uiRoot != null) uiRoot.SetActive(false);
    }

    void PastikanUI()
    {
        if (uiRoot != null) return;
        GameObject cvGo = new GameObject("InteraksiNPC_UI");
        Canvas cv = cvGo.AddComponent<Canvas>();
        cv.renderMode = RenderMode.ScreenSpaceOverlay;
        cv.sortingOrder = 320;
        CanvasScaler cs = cvGo.AddComponent<CanvasScaler>();
        cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(1920, 1080);

        GameObject panel = new GameObject("Bubble");
        panel.transform.SetParent(cvGo.transform, false);
        Image im = panel.AddComponent<Image>();
        im.color = new Color(0f, 0f, 0f, 0.7f);
        im.raycastTarget = false;
        RectTransform prt = im.rectTransform;
        prt.anchorMin = new Vector2(0.5f, 0f);
        prt.anchorMax = new Vector2(0.5f, 0f);
        prt.pivot = new Vector2(0.5f, 0f);
        prt.anchoredPosition = new Vector2(0f, 150f);
        prt.sizeDelta = new Vector2(980f, 72f);

        GameObject tGo = new GameObject("Teks");
        tGo.transform.SetParent(panel.transform, false);
        uiTeks = tGo.AddComponent<Text>();
        uiTeks.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        uiTeks.fontSize = 28;
        uiTeks.fontStyle = FontStyle.Bold;
        uiTeks.alignment = TextAnchor.MiddleCenter;
        uiTeks.color = new Color(1f, 0.95f, 0.7f);
        uiTeks.raycastTarget = false;
        RectTransform trt = uiTeks.rectTransform;
        trt.anchorMin = Vector2.zero;
        trt.anchorMax = Vector2.one;
        trt.offsetMin = new Vector2(14, 0);
        trt.offsetMax = new Vector2(-14, 0);

        uiRoot = cvGo;
        uiRoot.SetActive(false);
    }
}
