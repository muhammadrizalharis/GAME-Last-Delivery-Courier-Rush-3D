using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

// Pengatur Scene 2 (lari di jalan): HP, timer, menang/kalah, HUD
public class RunGame : MonoBehaviour
{
    public int nyawa = 3;              // (lama, tak dipakai lagi)
    public float waktu = 60f;
    public GameObject[] hati;          // (lama) disembunyikan saat Start
    public Text teksWaktu;
    public Text teksPesan;
    public string sceneBerikut = "Menu";
    public int skor = 0;
    public Text teksSkor;

    [Header("HP & Koin (rule final)")]
    public int hpMaks = 100;
    public int koinWajib = 6;
    int hp = 100;
    int koin = 0;
    int jumlahKena = 0;
    int nyawaSisa = 3;
    bool bawaPaket = false;
    PlayerLari pemainLari;

    public static RunGame instance;
    bool selesai = false;
    float sisaKebal = 0f;
    Renderer[] pemainRends;
    Color[] warnaAsli;
    bool kebalTadi = false;
    Image flashMerah;
    CameraFollow kamGetar;
    RectTransform barHPisi;
    Text teksHP;
    Text teksKoin;
    Text teksNyawa;
    Text teksPaket;

    void Awake() { instance = this; }

    void Start()
    {
        Time.timeScale = 1f;
        if (teksPesan) teksPesan.text = "";
        GameObject pl = GameObject.FindGameObjectWithTag("Player");
        if (pl != null)
        {
            pemainRends = pl.GetComponentsInChildren<Renderer>();
            warnaAsli = new Color[pemainRends.Length];
            for (int i = 0; i < pemainRends.Length; i++)
                warnaAsli[i] = pemainRends[i].material.color;
        }
        BuatFlash();
        BuatHUD();
        if (hati != null)
            for (int i = 0; i < hati.Length; i++)
                if (hati[i] != null) hati[i].SetActive(false);
        hp = hpMaks;
        nyawaSisa = Mathf.Max(1, nyawa);
        pemainLari = FindFirstObjectByType<PlayerLari>();
        SpawnKoin();
        SpawnPaket();
        PasangInteraksiNPC();
        kamGetar = FindFirstObjectByType<CameraFollow>();
        UpdateHUD();
    }

    void Update()
    {
        FadeFlash();
        if (selesai)
        {
            if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
                Transisi.Pindah(SceneManager.GetActiveScene().name);
            return;
        }

        waktu -= Time.deltaTime;
        if (waktu <= 0f) { waktu = 0f; Kalah("WAKTU HABIS"); }

        // perisai / kebal sementara
        bool kebal = sisaKebal > 0f;
        if (kebal)
        {
            sisaKebal -= Time.deltaTime;
            if (pemainRends != null)
            {
                float k = 0.5f + 0.5f * Mathf.Sin(Time.time * 12f);
                Color c = Color.Lerp(new Color(0.25f, 0.9f, 1f), Color.white, k);
                for (int i = 0; i < pemainRends.Length; i++)
                {
                    if (pemainRends[i] == null) continue;
                    pemainRends[i].material.color = c;
                    if (pemainRends[i].material.HasProperty("_BaseColor")) pemainRends[i].material.SetColor("_BaseColor", c);
                }
            }
        }
        else if (kebalTadi && pemainRends != null)
        {
            for (int i = 0; i < pemainRends.Length; i++)
            {
                if (pemainRends[i] == null || warnaAsli == null || i >= warnaAsli.Length) continue;
                pemainRends[i].material.color = warnaAsli[i];
                if (pemainRends[i].material.HasProperty("_BaseColor")) pemainRends[i].material.SetColor("_BaseColor", warnaAsli[i]);
            }
        }
        kebalTadi = kebal;

        UpdateHUD();
    }

    void UpdateHUD()
    {
        if (teksWaktu)
        {
            int t = Mathf.CeilToInt(waktu);
            teksWaktu.text = string.Format("{0:00}:{1:00}", t / 60, t % 60);
        }
        if (teksSkor) teksSkor.text = "Skor: " + skor;
        if (barHPisi != null)
        {
            float frac = hpMaks > 0 ? Mathf.Clamp01((float)hp / hpMaks) : 0f;
            barHPisi.localScale = new Vector3(frac, 1f, 1f);
        }
        if (teksHP != null) teksHP.text = "HP " + Mathf.Max(0, hp) + "/" + hpMaks;
        if (teksKoin != null) teksKoin.text = "Koin: " + koin + "/" + koinWajib;
        if (teksNyawa != null) teksNyawa.text = "Nyawa: " + Mathf.Max(0, nyawaSisa);
        if (teksPaket != null)
        {
            teksPaket.text = bawaPaket ? "Paket: DIBAWA" : "Paket: BELUM";
            teksPaket.color = bawaPaket ? new Color(0.4f, 1f, 0.55f) : new Color(1f, 0.6f, 0.4f);
        }
    }

    // dipanggil rintangan/NPC (default 20). Boss pakai Kena(40).
    public void Kena() { Kena(20); }

    public void Kena(int damage)
    {
        if (selesai) return;
        if (sisaKebal > 0f) return; // sedang kebal -> tak kena
        EfekKena();
        jumlahKena++;
        hp -= damage;
        if (hp <= 0)
        {
            nyawaSisa--;
            if (nyawaSisa > 0)
            {
                hp = hpMaks;           // masih ada nyawa -> HP dipulihkan (respawn)
                AktifkanPerisai(1.5f); // kebal sebentar agar tak langsung tumbang lagi
            }
            else { hp = 0; Kalah("GAME OVER"); }
        }
        UpdateHUD();
    }

    // item HP -> pulihkan HP
    public void PulihHP(int n)
    {
        if (selesai) return;
        hp = Mathf.Min(hpMaks, hp + n);
        UpdateHUD();
    }

    // dipanggil koin: hitung koin + tambah skor
    public void AmbilKoin(int nilaiSkor)
    {
        if (selesai) return;
        koin++;
        TambahSkor(nilaiSkor);
    }

    // paket diambil: status bawa paket aktif + skor + kecepatan pemain turun
    public void AmbilPaket()
    {
        if (selesai || bawaPaket) return;
        bawaPaket = true;
        if (pemainLari != null)
        {
            pemainLari.bawaPaket = true;
            RakitKurir rk = pemainLari.GetComponent<RakitKurir>();
            if (rk != null) rk.SetBawaTas(true);
        }
        TambahSkor(50);
        UpdateHUD();
    }

    public bool PunyaPaket() { return bawaPaket; }

    public bool BolehSelesai() { return koin >= koinWajib; }

    public void GagalKoinKurang()
    {
        Kalah("KOIN KURANG " + koin + "/" + koinWajib);
    }

    public void GagalPaket()
    {
        Kalah("PAKET BELUM DIAMBIL!");
    }

    // skor saat mencapai finish: finish + sisa waktu + kondisi paket
    public void SkorFinish()
    {
        int bonusWaktu = Mathf.CeilToInt(waktu) * 2;
        int kondisi = Mathf.Max(0, 100 - jumlahKena * 10);
        TambahSkor(100 + bonusWaktu + kondisi);
    }

    // lengkapi koin di jalur jadi 10 (hitung yang sudah ada, tambah kekurangannya)
    void SpawnKoin()
    {
        if (!Application.isPlaying) return;
        int ada = FindObjectsByType<Koin>(FindObjectsSortMode.None).Length;
        int perlu = 10 - ada;
        float[] lane = { -3f, 0f, 3f };
        for (int i = 0; i < perlu; i++)
        {
            float z = Mathf.Lerp(40f, 300f, (i + 0.5f) / Mathf.Max(1, perlu));
            float x = lane[(i + 1) % 3];
            GameObject g = new GameObject("KoinAuto");
            g.transform.position = new Vector3(x, 1.2f, z);
            BoxCollider bc = g.AddComponent<BoxCollider>();
            bc.isTrigger = true;
            bc.size = Vector3.one * 1.4f;
            g.AddComponent<Koin>();
        }
    }

    // buat 1 paket di dekat titik start kalau scene belum punya (wajib diambil sebelum finish)
    void SpawnPaket()
    {
        if (!Application.isPlaying) return;
        if (FindFirstObjectByType<Paket>() != null) return;
        GameObject g = new GameObject("PaketAuto");
        g.transform.position = new Vector3(0f, 1.2f, 12f);
        BoxCollider bc = g.AddComponent<BoxCollider>();
        bc.isTrigger = true;
        bc.size = Vector3.one * 1.6f;
        g.AddComponent<Paket>();
    }

    // pasang interaksi ke SEMUA NPC kecil (punya MobilJalan/Penyeberang), kecuali boss
    void PasangInteraksiNPC()
    {
        if (!Application.isPlaying) return;
        foreach (var m in FindObjectsByType<MobilJalan>(FindObjectsSortMode.None))
            PasangSatu(m.gameObject);
        foreach (var n in FindObjectsByType<Penyeberang>(FindObjectsSortMode.None))
            PasangSatu(n.gameObject);
    }

    void PasangSatu(GameObject go)
    {
        if (go.GetComponent<BossTruk>() != null || go.GetComponent<DialogBoss>() != null) return;
        if (go.GetComponent<InteraksiNPC>() == null) go.AddComponent<InteraksiNPC>();
    }

    // dipanggil item jam untuk tambah waktu
    public void TambahWaktu(float detik)
    {
        if (selesai) return;
        waktu += detik;
    }

    public void TambahSkor(int n)
    {
        if (selesai) return;
        skor += n;
        SkorGame.Tambah(n);
        UpdateHUD();
    }

    public void AktifkanPerisai(float durasi)
    {
        if (selesai) return;
        if (durasi > sisaKebal) sisaKebal = durasi;
    }

    public void Menang()
    {
        if (selesai) return;
        selesai = true;
        SkorGame.SimpanRekor();
        if (teksPesan) teksPesan.text = "PAKET TERKIRIM!\nTekan R untuk ulang";
        Time.timeScale = 0f;
    }

    void Kalah(string pesan)
    {
        if (selesai) return;
        selesai = true;
        SkorGame.SimpanRekor();
        if (teksPesan) teksPesan.text = pesan + "\nTekan R untuk ulang";
        Time.timeScale = 0f;
    }

    void BuatFlash()
    {
        GameObject cvGo = new GameObject("FlashKena");
        cvGo.transform.SetParent(transform, false);
        Canvas cv = cvGo.AddComponent<Canvas>();
        cv.renderMode = RenderMode.ScreenSpaceOverlay;
        cv.sortingOrder = 400;
        GameObject imgGo = new GameObject("Merah");
        imgGo.transform.SetParent(cvGo.transform, false);
        flashMerah = imgGo.AddComponent<Image>();
        flashMerah.color = new Color(1f, 0f, 0f, 0f);
        flashMerah.raycastTarget = false;
        RectTransform rt = flashMerah.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    void BuatHUD()
    {
        GameObject cvGo = new GameObject("HUD_HP");
        cvGo.transform.SetParent(transform, false);
        Canvas cv = cvGo.AddComponent<Canvas>();
        cv.renderMode = RenderMode.ScreenSpaceOverlay;
        cv.sortingOrder = 300;
        CanvasScaler cs = cvGo.AddComponent<CanvasScaler>();
        cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(1920, 1080);

        RectTransform bg = BuatKotak(cv.transform, "HPbg", new Color(0f, 0f, 0f, 0.55f),
            new Vector2(0, 1), new Vector2(30, -30), new Vector2(340, 34));
        barHPisi = BuatKotak(bg, "HPisi", new Color(0.3f, 0.85f, 0.35f, 1f),
            new Vector2(0, 0.5f), new Vector2(4, 0), new Vector2(332, 26));
        teksHP = BuatLabel(bg, "HP 100/100", 20, TextAnchor.MiddleCenter);
        RectTransform kr = BuatKotak(cv.transform, "KoinBg", new Color(0f, 0f, 0f, 0f),
            new Vector2(0, 1), new Vector2(30, -72), new Vector2(340, 34));
        teksKoin = BuatLabel(kr, "Koin: 0/6", 24, TextAnchor.MiddleLeft);
        RectTransform nr = BuatKotak(cv.transform, "NyawaBg", new Color(0f, 0f, 0f, 0f),
            new Vector2(0, 1), new Vector2(30, -108), new Vector2(340, 32));
        teksNyawa = BuatLabel(nr, "Nyawa: 3", 22, TextAnchor.MiddleLeft);
        RectTransform pr = BuatKotak(cv.transform, "PaketBg", new Color(0f, 0f, 0f, 0f),
            new Vector2(0, 1), new Vector2(30, -142), new Vector2(340, 32));
        teksPaket = BuatLabel(pr, "Paket: BELUM", 22, TextAnchor.MiddleLeft);
    }

    RectTransform BuatKotak(Transform parent, string nama, Color warna, Vector2 anchor, Vector2 pos, Vector2 size)
    {
        GameObject go = new GameObject(nama);
        go.transform.SetParent(parent, false);
        Image im = go.AddComponent<Image>();
        im.color = warna;
        im.raycastTarget = false;
        RectTransform rt = im.rectTransform;
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.pivot = anchor;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return rt;
    }

    Text BuatLabel(Transform parent, string isi, int ukuran, TextAnchor align)
    {
        GameObject go = new GameObject("Teks");
        go.transform.SetParent(parent, false);
        Text t = go.AddComponent<Text>();
        t.text = isi;
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = ukuran;
        t.fontStyle = FontStyle.Bold;
        t.alignment = align;
        t.color = Color.white;
        t.raycastTarget = false;
        RectTransform rt = t.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(8, 0);
        rt.offsetMax = new Vector2(-8, 0);
        return t;
    }

    void FadeFlash()
    {
        if (flashMerah != null && flashMerah.color.a > 0f)
        {
            float a = flashMerah.color.a - Time.unscaledDeltaTime * 1.8f;
            flashMerah.color = new Color(1f, 0f, 0f, Mathf.Max(0f, a));
        }
    }

    void EfekKena()
    {
        if (flashMerah != null) flashMerah.color = new Color(1f, 0f, 0f, 0.5f);
        if (kamGetar == null) kamGetar = FindFirstObjectByType<CameraFollow>();
        if (kamGetar != null) kamGetar.Getar(0.35f);
    }
}
