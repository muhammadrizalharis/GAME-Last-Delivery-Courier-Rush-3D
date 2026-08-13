using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Mengatur tombol di layar judul (Scene Menu): Play, About, Quit
public class MenuUtama : MonoBehaviour
{
    public string sceneLevel1 = "Level1Scene1";
    GameObject panelAbout;

    public void Mulai()
    {
        SkorGame.Reset();
        Transisi.Pindah(sceneLevel1);
    }

    public void Keluar()
    {
        Debug.Log("Keluar game");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // tombol About: bangun panel info (sekali) lalu tampil/sembunyikan
    public void Tentang()
    {
        if (panelAbout == null) { BuatAbout(); return; }
        panelAbout.SetActive(!panelAbout.activeSelf);
    }

    void TutupAbout()
    {
        if (panelAbout != null) panelAbout.SetActive(false);
    }

    void BuatAbout()
    {
        GameObject cvGo = new GameObject("AboutUI");
        Canvas cv = cvGo.AddComponent<Canvas>();
        cv.renderMode = RenderMode.ScreenSpaceOverlay;
        cv.sortingOrder = 500;
        CanvasScaler cs = cvGo.AddComponent<CanvasScaler>();
        cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(1920, 1080);
        cvGo.AddComponent<GraphicRaycaster>();

        panelAbout = new GameObject("PanelAbout");
        panelAbout.transform.SetParent(cvGo.transform, false);
        Image dim = panelAbout.AddComponent<Image>();
        dim.color = new Color(0f, 0f, 0f, 0.78f);
        RectTransform drt = dim.rectTransform;
        drt.anchorMin = Vector2.zero; drt.anchorMax = Vector2.one;
        drt.offsetMin = Vector2.zero; drt.offsetMax = Vector2.zero;

        GameObject box = new GameObject("Kotak");
        box.transform.SetParent(panelAbout.transform, false);
        Image bim = box.AddComponent<Image>();
        bim.color = new Color(0.1f, 0.12f, 0.18f, 0.98f);
        RectTransform brt = bim.rectTransform;
        brt.anchorMin = new Vector2(0.5f, 0.5f);
        brt.anchorMax = new Vector2(0.5f, 0.5f);
        brt.pivot = new Vector2(0.5f, 0.5f);
        brt.anchoredPosition = Vector2.zero;
        brt.sizeDelta = new Vector2(1160f, 760f);

        Teks(box.transform, "TENTANG GAME", 54, new Vector2(0, -46), new Vector2(1080, 70),
            FontStyle.Bold, new Color(1f, 0.85f, 0.3f), TextAnchor.MiddleCenter);

        string isi =
            "Last Delivery: Courier Rush 3D\n\n" +
            "Game 3D action-runner. Kamu seorang KURIR yang harus mengambil\n" +
            "PAKET lalu mengantarnya ke garis FINISH di setiap level.\n\n" +
            "- Ambil paket dulu & kumpulkan minimal 6 koin untuk bisa finish.\n" +
            "- Membawa paket membuat kurir lebih lambat -- atur jalurmu!\n" +
            "- Hindari rintangan, NPC, dan boss di akhir tiap level.\n" +
            "- 100 HP, 3 nyawa, timer, item HP / waktu / perisai.\n\n" +
            "Kontrol:\n" +
            "A/D atau panah kiri-kanan = geser lajur\n" +
            "W / Spasi = lompat,   S / bawah / Ctrl = menunduk\n" +
            "E = mengobrol dengan NPC,   R = ulang saat kalah\n\n" +
            "5 Level: City Street, Railway Track, Dark Tunnel,\n" +
            "Night Market, dan Industrial Zone.";
        Teks(box.transform, isi, 26, new Vector2(0, -140), new Vector2(1060, 540),
            FontStyle.Normal, Color.white, TextAnchor.UpperCenter);

        GameObject btn = new GameObject("BtnTutup");
        btn.transform.SetParent(box.transform, false);
        Image tim = btn.AddComponent<Image>();
        tim.color = new Color(0.85f, 0.25f, 0.25f, 1f);
        RectTransform trt = tim.rectTransform;
        trt.anchorMin = new Vector2(0.5f, 0f);
        trt.anchorMax = new Vector2(0.5f, 0f);
        trt.pivot = new Vector2(0.5f, 0f);
        trt.anchoredPosition = new Vector2(0f, 30f);
        trt.sizeDelta = new Vector2(300f, 72f);
        Button b = btn.AddComponent<Button>();
        b.targetGraphic = tim;
        b.onClick.AddListener(TutupAbout);

        Text bt = Teks(btn.transform, "TUTUP", 32, Vector2.zero, Vector2.zero,
            FontStyle.Bold, Color.white, TextAnchor.MiddleCenter);
        RectTransform btr = bt.rectTransform;
        btr.anchorMin = Vector2.zero; btr.anchorMax = Vector2.one;
        btr.offsetMin = Vector2.zero; btr.offsetMax = Vector2.zero;
    }

    Text Teks(Transform parent, string teks, int size, Vector2 pos, Vector2 sz,
              FontStyle style, Color col, TextAnchor align)
    {
        GameObject g = new GameObject("Teks");
        g.transform.SetParent(parent, false);
        Text t = g.AddComponent<Text>();
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.text = teks;
        t.fontSize = size;
        t.fontStyle = style;
        t.color = col;
        t.alignment = align;
        t.supportRichText = false;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        t.raycastTarget = false;
        RectTransform rt = t.rectTransform;
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = sz;
        return t;
    }
}
