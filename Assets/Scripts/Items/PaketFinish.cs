using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

// Paket di POS PAKET: saat diambil muncul BRIEFING misi (paket diantar ke mana),
// game berhenti sejenak, lalu MULAI ANTAR -> Level 1. Kurir mulai membawa tas paket.
public class PaketFinish : MonoBehaviour
{
    public string tujuan = "Level1Scene2";
    GameObject panel;
    Text teksNama, teksIsi, teksTombol;
    string[] nama, isi;
    Color[] warna;
    int idx = -1;
    bool sudah = false;

    void Start()
    {
        BuatUI();
        if (panel != null) panel.SetActive(false);
    }

    void Update()
    {
        if (!Application.isPlaying || idx < 0) return;
        if (Keyboard.current != null &&
            (Keyboard.current.spaceKey.wasPressedThisFrame ||
             Keyboard.current.enterKey.wasPressedThisFrame))
            Lanjut();
    }

    void OnTriggerEnter(Collider other)
    {
        if (sudah || !other.CompareTag("Player")) return;
        sudah = true;
        if (RunGame.instance != null) RunGame.instance.TambahSkor(50);
        RakitKurir rk = other.GetComponent<RakitKurir>();
        if (rk != null) rk.SetBawaTas(true);
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr != null) mr.enabled = false;
        Mulai();
    }

    void Mulai()
    {
        nama = new string[] { "PETUGAS POS", "KURIR" };
        isi = new string[] {
            "Kurir! Ini paket terakhir hari ini. Antar melewati lima kawasan - kota, rel kereta, terowongan, pasar malam, sampai kawasan pabrik - hingga garis finish di tiap level. Jangan sampai terlambat!",
            "Siap! Paket ini pasti kuantar sampai tujuan."
        };
        warna = new Color[] { new Color(1f, 0.85f, 0.4f), new Color(0.5f, 0.9f, 1f) };
        idx = 0;
        if (panel != null) panel.SetActive(true);
        Tampil();
        Time.timeScale = 0f;
    }

    void Tampil()
    {
        if (teksNama != null) { teksNama.text = nama[idx]; teksNama.color = warna[idx]; }
        if (teksIsi != null) teksIsi.text = isi[idx];
        if (teksTombol != null) teksTombol.text = (idx >= isi.Length - 1) ? "MULAI ANTAR >" : "LANJUT >";
    }

    public void Lanjut()
    {
        idx++;
        if (isi == null || idx >= isi.Length)
        {
            idx = 999;
            if (panel != null) panel.SetActive(false);
            Time.timeScale = 1f;
            Transisi.Pindah(tujuan);
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
        GameObject cvGo = new GameObject("BriefingUI");
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
        prt.sizeDelta = new Vector2(1500f, 260f);

        teksNama = BuatTeks(panel.transform, 32, new Vector2(40f, -20f), new Vector2(700f, 46f));
        teksNama.fontStyle = FontStyle.Bold;
        teksIsi = BuatTeks(panel.transform, 30, new Vector2(40f, -78f), new Vector2(1420f, 150f));

        GameObject btnGo = new GameObject("BtnNext");
        btnGo.transform.SetParent(panel.transform, false);
        Image bim = btnGo.AddComponent<Image>();
        bim.color = new Color(0.2f, 0.7f, 0.35f, 1f);
        RectTransform brt = bim.rectTransform;
        brt.anchorMin = new Vector2(1f, 0f);
        brt.anchorMax = new Vector2(1f, 0f);
        brt.pivot = new Vector2(1f, 0f);
        brt.anchoredPosition = new Vector2(-30f, 25f);
        brt.sizeDelta = new Vector2(320f, 66f);
        Button btn = btnGo.AddComponent<Button>();
        btn.targetGraphic = bim;
        btn.onClick.AddListener(Lanjut);

        teksTombol = BuatTeks(btnGo.transform, 26, Vector2.zero, Vector2.zero);
        teksTombol.fontStyle = FontStyle.Bold;
        teksTombol.alignment = TextAnchor.MiddleCenter;
        RectTransform btr = teksTombol.rectTransform;
        btr.anchorMin = Vector2.zero; btr.anchorMax = Vector2.one;
        btr.pivot = new Vector2(0.5f, 0.5f);
        btr.offsetMin = Vector2.zero; btr.offsetMax = Vector2.zero;
        teksTombol.text = "LANJUT >";
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
