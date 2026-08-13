using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

// Dialog boss: saat pemain mendekat, game BERHENTI (pause) dan muncul panel dialog
// (boss lalu kurir) dengan tombol LANJUT. Setelah selesai, game lanjut lagi. Play-only.
public class DialogBoss : MonoBehaviour
{
    public string namaBoss = "BOSS";
    public string dialogBoss = "Kau tak akan lewat!";
    public string dialogKurir = "Paket ini harus sampai!";
    public float jarakPicu = 30f;

    Transform player;
    GameObject panel;
    Text teksNama, teksIsi;
    string[] nama;
    string[] isi;
    Color[] warna;
    int idx = -1;
    bool selesai = false;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
        BuatUI();
        if (panel != null) panel.SetActive(false);
    }

    void Update()
    {
        if (!Application.isPlaying || selesai || player == null) return;

        if (idx < 0)
        {
            float dz = transform.position.z - player.position.z;
            if (dz <= jarakPicu && dz > -5f) Mulai();
            return;
        }

        if (Keyboard.current != null &&
            (Keyboard.current.spaceKey.wasPressedThisFrame ||
             Keyboard.current.enterKey.wasPressedThisFrame))
            Lanjut();
    }

    void Mulai()
    {
        nama = new string[] { namaBoss, "KURIR" };
        isi = new string[] { dialogBoss, dialogKurir };
        warna = new Color[] { new Color(1f, 0.5f, 0.4f), new Color(0.5f, 0.9f, 1f) };
        idx = 0;
        if (panel != null) panel.SetActive(true);
        Tampil();
        Time.timeScale = 0f;
    }

    void Tampil()
    {
        if (teksNama != null) { teksNama.text = nama[idx]; teksNama.color = warna[idx]; }
        if (teksIsi != null) teksIsi.text = isi[idx];
    }

    public void Lanjut()
    {
        idx++;
        if (isi == null || idx >= isi.Length)
        {
            selesai = true;
            if (panel != null) panel.SetActive(false);
            Time.timeScale = 1f;
            return;
        }
        Tampil();
    }

    void PastikanEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }
    }

    void BuatUI()
    {
        PastikanEventSystem();

        GameObject cvGo = new GameObject("DialogBossUI");
        Canvas cv = cvGo.AddComponent<Canvas>();
        cv.renderMode = RenderMode.ScreenSpaceOverlay;
        cv.sortingOrder = 360;
        CanvasScaler cs = cvGo.AddComponent<CanvasScaler>();
        cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(1920, 1080);
        cvGo.AddComponent<GraphicRaycaster>();

        panel = new GameObject("Panel");
        panel.transform.SetParent(cvGo.transform, false);
        Image pim = panel.AddComponent<Image>();
        pim.color = new Color(0f, 0f, 0f, 0.85f);
        RectTransform prt = pim.rectTransform;
        prt.anchorMin = new Vector2(0.5f, 0f);
        prt.anchorMax = new Vector2(0.5f, 0f);
        prt.pivot = new Vector2(0.5f, 0f);
        prt.anchoredPosition = new Vector2(0f, 40f);
        prt.sizeDelta = new Vector2(1500f, 250f);

        teksNama = BuatTeks(panel.transform, 32, new Vector2(40f, -20f), new Vector2(700f, 46f));
        teksNama.fontStyle = FontStyle.Bold;
        teksIsi = BuatTeks(panel.transform, 34, new Vector2(40f, -78f), new Vector2(1420f, 120f));

        GameObject btnGo = new GameObject("BtnNext");
        btnGo.transform.SetParent(panel.transform, false);
        Image bim = btnGo.AddComponent<Image>();
        bim.color = new Color(0.2f, 0.6f, 0.9f, 1f);
        RectTransform brt = bim.rectTransform;
        brt.anchorMin = new Vector2(1f, 0f);
        brt.anchorMax = new Vector2(1f, 0f);
        brt.pivot = new Vector2(1f, 0f);
        brt.anchoredPosition = new Vector2(-30f, 25f);
        brt.sizeDelta = new Vector2(240f, 66f);
        Button btn = btnGo.AddComponent<Button>();
        btn.targetGraphic = bim;
        btn.onClick.AddListener(Lanjut);

        Text bt = BuatTeks(btnGo.transform, 28, Vector2.zero, Vector2.zero);
        bt.fontStyle = FontStyle.Bold;
        bt.alignment = TextAnchor.MiddleCenter;
        RectTransform btr = bt.rectTransform;
        btr.anchorMin = Vector2.zero;
        btr.anchorMax = Vector2.one;
        btr.pivot = new Vector2(0.5f, 0.5f);
        btr.offsetMin = Vector2.zero;
        btr.offsetMax = Vector2.zero;
        bt.text = "LANJUT >";
    }

    Text BuatTeks(Transform parent, int ukuran, Vector2 pos, Vector2 size)
    {
        GameObject t = new GameObject("Teks");
        t.transform.SetParent(parent, false);
        Text tx = t.AddComponent<Text>();
        tx.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        tx.fontSize = ukuran;
        tx.alignment = TextAnchor.UpperLeft;
        tx.color = Color.white;
        tx.supportRichText = false;
        tx.horizontalOverflow = HorizontalWrapMode.Wrap;
        tx.verticalOverflow = VerticalWrapMode.Overflow;
        tx.raycastTarget = false;
        RectTransform rt = tx.rectTransform;
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 1f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return tx;
    }
}
