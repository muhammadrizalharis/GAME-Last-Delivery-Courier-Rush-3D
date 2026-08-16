using UnityEngine;
using System.Collections.Generic;

// Menghias kawasan kota dengan PROPERTI dari primitif: pohon, lampu jalan, properti atap gedung, & bangku.
// Penempatan otomatis pakai bounds objek scene (nama "gedung" utk atap, "jalan" utk lampu, "tanah" utk pohon).
// [ExecuteAlways] -> tampil di editor juga. Objek DontSave (dibangun ulang tiap load, tidak dobel).
[ExecuteAlways]
public class HiasKota : MonoBehaviour
{
    public int jumlahPohon = 26;
    public int jumlahOrang = 12;

    Shader shaderLit;
    Transform root;
    Texture2D texJendela;
    readonly List<Bounds> gedung = new List<Bounds>();
    readonly List<Bounds> hindari = new List<Bounds>();
    readonly List<Material> lampuMat = new List<Material>();
    Bounds jalanB, tanahB;
    bool adaJalan, adaTanah;

    void OnEnable() { Bangun(); }

    void Hancur(Object o)
    {
        if (o == null) return;
        if (Application.isPlaying) Destroy(o); else DestroyImmediate(o);
    }

    void Bangun()
    {
        Transform lama = transform.Find("HiasKota");
        if (lama != null) Hancur(lama.gameObject);

        shaderLit = Shader.Find("Universal Render Pipeline/Lit");
        if (shaderLit == null) shaderLit = Shader.Find("Standard");
        if (texJendela == null) texJendela = GenTekstur(128, JendelaKota);

        GameObject r = new GameObject("HiasKota");
        r.hideFlags = HideFlags.DontSave;
        r.transform.SetParent(transform, false);
        root = r.transform;

        gedung.Clear(); hindari.Clear(); lampuMat.Clear();
        adaJalan = adaTanah = false;

        Pindai();
        Gedungkan();
        LampuJalan();
        Pohon();
        Bangku();
        OrangKota();
    }

    void Pindai()
    {
        foreach (MeshRenderer mr in FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None))
        {
            if (mr == null || mr.transform.IsChildOf(transform)) continue; // lewati properti sendiri
            string n = mr.gameObject.name.ToLower();
            if (n.Contains("gedung") || n.Contains("menara")) { gedung.Add(mr.bounds); hindari.Add(mr.bounds); }
            else if (n.Contains("pos") || n.Contains("dinding") || n.Contains("lantai") || n.Contains("meja") || n.Contains("atap")) hindari.Add(mr.bounds);
            else if (n == "jalan") { jalanB = mr.bounds; adaJalan = true; }
            else if (n == "tanah") { tanahB = mr.bounds; adaTanah = true; }
        }
    }

    // ============ PROPERTI ============
    // Bungkus tiap kotak gedung jadi bangunan detail (fasad kaca + lobby + ledge + pintu/kanopi + atap + properti).
    void Gedungkan()
    {
        System.Random rnd = new System.Random(31);
        float roadX = adaJalan ? jalanB.center.x : 0f;
        float gy = adaTanah ? tanahB.max.y : 0f;
        string[] jenis = { "KANTOR", "BANK", "TOKO", "KAFE", "HOTEL", "APOTEK" };
        Color[] warnaPapan = {
            new Color(0.16f,0.34f,0.62f), new Color(0.15f,0.45f,0.3f), new Color(0.75f,0.4f,0.12f),
            new Color(0.6f,0.18f,0.2f), new Color(0.35f,0.24f,0.5f), new Color(0.15f,0.5f,0.5f)
        };
        int idx = 0;
        foreach (Bounds b in gedung)
        {
            Vector3 c = b.center;
            float w = b.size.x, h = b.size.y, d = b.size.z;
            int kolom = Mathf.Max(2, Mathf.RoundToInt(Mathf.Max(w, d) / 1.6f));
            int lantai = Mathf.Max(3, Mathf.RoundToInt(h / 1.6f));

            KotakFasad("Fasad", c, new Vector3(w + 0.08f, h + 0.02f, d + 0.08f), new Vector2(kolom, lantai));
            Kotak("Lobby", new Vector3(c.x, b.min.y + 1f, c.z), new Vector3(w + 0.25f, 2f, d + 0.25f), new Color(0.32f, 0.33f, 0.4f));

            int nL = Mathf.Clamp(Mathf.FloorToInt(h / 3.2f), 0, 6);
            for (int L = 1; L <= nL; L++)
                Kotak("Ledge", new Vector3(c.x, b.min.y + L * 3.2f, c.z), new Vector3(w + 0.22f, 0.2f, d + 0.22f), new Color(0.4f, 0.38f, 0.36f));

            Kotak("AtapDatar", new Vector3(c.x, b.max.y + 0.12f, c.z), new Vector3(w + 0.3f, 0.25f, d + 0.3f), new Color(0.28f, 0.28f, 0.32f));

            float dir = Mathf.Sign(roadX - c.x); if (dir == 0f) dir = -1f;
            float faceX = c.x + dir * (w * 0.5f);
            float pintuLebar = Mathf.Min(1.6f, d * 0.4f);
            Kotak("KusenPintu", new Vector3(faceX + dir * 0.04f, b.min.y + 1.0f, c.z), new Vector3(0.16f, 2.15f, pintuLebar + 0.4f), new Color(0.86f, 0.86f, 0.9f));   // kusen terang -> pintu jelas
            Kotak("Pintu", new Vector3(faceX + dir * 0.1f, b.min.y + 0.95f, c.z), new Vector3(0.12f, 1.95f, pintuLebar), new Color(0.22f, 0.3f, 0.42f));                // daun pintu kaca
            Kotak("GarisPintu", new Vector3(faceX + dir * 0.13f, b.min.y + 0.95f, c.z), new Vector3(0.06f, 1.95f, 0.05f), new Color(0.7f, 0.7f, 0.74f));               // celah 2 daun
            Kotak("Gagang1", new Vector3(faceX + dir * 0.16f, b.min.y + 1.0f, c.z - 0.18f), new Vector3(0.06f, 0.35f, 0.05f), new Color(0.85f, 0.72f, 0.3f));
            Kotak("Gagang2", new Vector3(faceX + dir * 0.16f, b.min.y + 1.0f, c.z + 0.18f), new Vector3(0.06f, 0.35f, 0.05f), new Color(0.85f, 0.72f, 0.3f));
            Kotak("Kanopi", new Vector3(faceX + dir * 0.55f, b.min.y + 2.1f, c.z), new Vector3(1.1f, 0.12f, Mathf.Min(2f, d * 0.55f)), new Color(0.55f, 0.22f, 0.22f));

            PropAtap(rnd, b);
            EntranceReal(b, dir, faceX, gy, jenis[idx % jenis.Length], warnaPapan[idx % warnaPapan.Length]);  // #9 bangunan sungguhan
            OrangMasukKeluar(rnd, b, dir, faceX, gy);                                                          // #8 orang keluar-masuk
            idx++;
        }
    }

    void PropAtap(System.Random rnd, Bounds b)
    {
        Vector3 c = new Vector3(b.center.x, b.max.y + 0.25f, b.center.z);
        int t = rnd.Next(4);
        if (t == 0)
            Silinder("TangkiAir", c + Vector3.up * 0.6f, new Vector3(1.1f, 0.55f, 1.1f), new Color(0.55f, 0.4f, 0.3f));
        else if (t == 1)
        {
            Kotak("Antena", c + Vector3.up * 1.3f, new Vector3(0.1f, 2.4f, 0.1f), new Color(0.2f, 0.2f, 0.22f));
            GameObject l = Kotak("LampuAntena", c + Vector3.up * 2.5f, new Vector3(0.24f, 0.24f, 0.24f), new Color(1f, 0.2f, 0.2f), 1.7f);
            lampuMat.Add(l.GetComponent<Renderer>().sharedMaterial);
        }
        else if (t == 2)
        {
            Kotak("AC1", c + new Vector3(b.extents.x * 0.3f, 0.4f, 0f), new Vector3(1f, 0.7f, 1f), new Color(0.5f, 0.5f, 0.54f));
            Kotak("AC2", c + new Vector3(-b.extents.x * 0.3f, 0.35f, b.extents.z * 0.2f), new Vector3(0.8f, 0.5f, 0.8f), new Color(0.48f, 0.48f, 0.52f));
        }
    }

    // ---- kesan bangunan sungguhan: tangga masuk + etalase kaca + papan nama berteks (#9) ----
    void EntranceReal(Bounds b, float dir, float faceX, float gy, string teks, Color papan)
    {
        Vector3 c = b.center;
        for (int i = 0; i < 2; i++)   // tangga masuk 2 undak
            Kotak("UndakMasuk", new Vector3(faceX + dir * (0.45f + i * 0.4f), gy + 0.12f + i * 0.16f, c.z),
                  new Vector3(0.8f, 0.24f + i * 0.32f, Mathf.Min(3.2f, b.size.z * 0.6f)), new Color(0.55f, 0.54f, 0.5f));
        float zoff = Mathf.Min(2.2f, b.size.z * 0.34f);   // etalase kaca mengapit pintu (lantai dasar)
        Kotak("Etalase", new Vector3(faceX + dir * 0.05f, b.min.y + 1.3f, c.z + zoff), new Vector3(0.1f, 1.7f, 1.5f), new Color(0.6f, 0.82f, 0.95f), 0.5f);
        Kotak("Etalase", new Vector3(faceX + dir * 0.05f, b.min.y + 1.3f, c.z - zoff), new Vector3(0.1f, 1.7f, 1.5f), new Color(0.6f, 0.82f, 0.95f), 0.5f);
        float signY = Mathf.Min(b.min.y + 2.75f, b.max.y - 0.6f);   // jangan melewati atap gedung pendek
        LabelGedung(new Vector3(faceX + dir * 0.16f, signY, c.z), dir, teks, papan);   // papan nama + jenis
    }

    // ---- papan nama gedung: papan + TEKS HURUF-BALOK (kubus solid -> tak tembus/kebalik, tetap nempel di papan) ----
    void LabelGedung(Vector3 pos, float dir, string teks, Color papan)
    {
        float panjang = Mathf.Max(2f, teks.Length * 0.62f);
        Kotak("PapanNama", pos, new Vector3(0.14f, 0.85f, panjang), papan, 0.55f);
        HurufBalok.Tulis(root, teks, pos + new Vector3(dir * 0.11f, 0.02f, 0f), 0.5f, (int)dir, Color.white);
    }

    // ---- orang benar-benar MASUK-KELUAR pintu gedung (dari jalan menembus fasad = masuk) (#8) ----
    void OrangMasukKeluar(System.Random rnd, Bounds b, float dir, float faceX, float gy)
    {
        if (!adaTanah) return;
        Vector3 c = b.center;
        // ayun sepanjang normal pintu: dari jalan (faceX+jarak) MASUK ke dalam gedung (faceX-jarak, di balik fasad = tersembunyi)
        OrangJalan(new Vector3(faceX, gy, c.z + 0.5f), WarnaBaju(rnd), new Vector3(dir, 0f, 0f), 2.7f, 1);
        OrangJalan(new Vector3(faceX, gy, c.z - 0.9f), WarnaBaju(rnd), new Vector3(dir, 0f, 0f), 3.1f, 1);
        if (rnd.Next(2) == 0)
            OrangJalan(new Vector3(faceX + dir * 2.8f, gy, c.z - 1.7f), WarnaBaju(rnd), Vector3.forward, 2.2f, 1);      // menyusuri trotoar
    }

    Color WarnaBaju(System.Random rnd)
    {
        Color[] baju = {
            new Color(0.85f,0.35f,0.3f), new Color(0.3f,0.55f,0.85f), new Color(0.35f,0.65f,0.4f),
            new Color(0.9f,0.8f,0.4f), new Color(0.6f,0.45f,0.8f), new Color(0.9f,0.55f,0.3f),
            new Color(0.5f,0.7f,0.78f), new Color(0.82f,0.5f,0.6f)
        };
        return baju[rnd.Next(baju.Length)];
    }

    GameObject KotakFasad(string nama, Vector3 pos, Vector3 skala, Vector2 tiling)
    {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        g.name = nama;
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.localPosition = pos;
        g.transform.localScale = skala;
        g.GetComponent<Renderer>().sharedMaterial = MatFasad(tiling);
        Collider col = g.GetComponent<Collider>();
        if (col != null) Hancur(col);
        return g;
    }

    Material MatFasad(Vector2 tiling)
    {
        Material m = new Material(shaderLit) { hideFlags = HideFlags.DontSave };
        m.color = Color.white;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", Color.white);
        m.mainTexture = texJendela;
        m.mainTextureScale = tiling;
        m.EnableKeyword("_EMISSION");
        m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        if (m.HasProperty("_EmissionMap")) { m.SetTexture("_EmissionMap", texJendela); m.SetTextureScale("_EmissionMap", tiling); }
        m.SetColor("_EmissionColor", Color.white * 0.45f);
        return m;
    }

    Texture2D GenTekstur(int n, System.Func<int, int, Color> pola)
    {
        Texture2D t = new Texture2D(n, n) { wrapMode = TextureWrapMode.Repeat, hideFlags = HideFlags.DontSave };
        Color[] px = new Color[n * n];
        for (int y = 0; y < n; y++)
            for (int x = 0; x < n; x++)
                px[y * n + x] = pola(x, y);
        t.SetPixels(px);
        t.Apply();
        return t;
    }

    Color JendelaKota(int x, int y)
    {
        const int cell = 16, bingkai = 4;
        int cx = x / cell, cy = y / cell;
        int lx = x % cell, ly = y % cell;
        if (lx < bingkai || ly < bingkai) return new Color(0.30f, 0.31f, 0.36f);
        int hsh = (cx * 73856093) ^ (cy * 19349663) ^ (cx * cy * 83492791);
        if ((hsh & 7) > 2)
            return ((hsh >> 3) & 1) == 0 ? new Color(1f, 0.86f, 0.55f) : new Color(0.6f, 0.82f, 1f);
        return new Color(0.10f, 0.12f, 0.18f);
    }

    void LampuJalan()
    {
        if (!adaJalan) return;
        float tepi = jalanB.extents.x + 0.9f;
        float z0 = jalanB.center.z - jalanB.extents.z + 3f;
        float z1 = jalanB.center.z + jalanB.extents.z - 3f;
        if (z1 - z0 > 70f) { z0 = jalanB.center.z - 33f; z1 = jalanB.center.z + 20f; } // batasi ke area terlihat
        for (float z = z0; z <= z1; z += 8f)
        {
            SatuLampu(new Vector3(jalanB.center.x - tepi, 0f, z), -1);
            SatuLampu(new Vector3(jalanB.center.x + tepi, 0f, z), 1);
        }
    }

    void SatuLampu(Vector3 pos, int sisi)
    {
        Color gelap = new Color(0.16f, 0.16f, 0.18f);
        Kotak("Tiang", pos + Vector3.up * 2f, new Vector3(0.18f, 4f, 0.18f), gelap);
        Kotak("Lengan", pos + new Vector3(-sisi * 0.4f, 3.9f, 0f), new Vector3(0.9f, 0.14f, 0.14f), gelap);
        GameObject k = Bola("Kepala", pos + new Vector3(-sisi * 0.8f, 3.75f, 0f), new Vector3(0.4f, 0.35f, 0.4f), new Color(1f, 0.85f, 0.5f), 1.6f);
        lampuMat.Add(k.GetComponent<Renderer>().sharedMaterial);
    }

    void Pohon()
    {
        if (!adaTanah) return;
        System.Random rnd = new System.Random(53);
        float halfRoad = adaJalan ? jalanB.extents.x + 1.8f : 6f;
        float cx = adaJalan ? jalanB.center.x : 0f;
        int dibuat = 0, coba = 0;
        while (dibuat < jumlahPohon && coba < jumlahPohon * 12)
        {
            coba++;
            float x = cx + (float)(rnd.NextDouble() * 44 - 22);
            float z = jalanB.center.z + (float)(rnd.NextDouble() * 52 - 34);
            if (Mathf.Abs(x - cx) < halfRoad) continue;           // jangan di jalan
            Vector3 p = new Vector3(x, 0f, z);
            if (KenaHindari(p)) continue;                          // jangan di dalam bangunan
            SatuPohon(rnd, p);
            dibuat++;
        }
    }

    bool KenaHindari(Vector3 p)
    {
        foreach (Bounds b in hindari)
        {
            if (p.x > b.min.x - 1.2f && p.x < b.max.x + 1.2f && p.z > b.min.z - 1.2f && p.z < b.max.z + 1.2f)
                return true;
        }
        return false;
    }

    void SatuPohon(System.Random rnd, Vector3 pos)
    {
        float h = 1.7f + (float)rnd.NextDouble() * 1.5f;
        Silinder("Batang", pos + Vector3.up * (h * 0.5f), new Vector3(0.28f, h * 0.5f, 0.28f), new Color(0.45f, 0.32f, 0.2f));
        Color hijau = Color.Lerp(new Color(0.18f, 0.42f, 0.18f), new Color(0.36f, 0.62f, 0.3f), (float)rnd.NextDouble());
        Bola("Daun1", pos + Vector3.up * (h + 0.35f), new Vector3(1.7f, 1.6f, 1.7f), hijau);
        Bola("Daun2", pos + new Vector3(0.45f, h - 0.05f, 0.2f), new Vector3(1.2f, 1.1f, 1.2f), hijau * 0.9f);
        Bola("Daun3", pos + new Vector3(-0.4f, h + 0.15f, -0.25f), new Vector3(1.2f, 1.1f, 1.2f), hijau);
    }

    void Bangku()
    {
        if (!adaJalan) return;
        float tepi = jalanB.extents.x + 2.3f;
        Color kayu = new Color(0.5f, 0.36f, 0.22f);
        float[] zs = { jalanB.center.z - 18f, jalanB.center.z - 4f, jalanB.center.z + 8f };
        foreach (float z in zs)
        {
            SatuBangku(new Vector3(jalanB.center.x - tepi, 0f, z), kayu);
            SatuBangku(new Vector3(jalanB.center.x + tepi, 0f, z), kayu);
        }
    }

    void SatuBangku(Vector3 pos, Color kayu)
    {
        if (KenaHindari(pos)) return;
        Kotak("Dudukan", pos + Vector3.up * 0.5f, new Vector3(1.6f, 0.1f, 0.5f), kayu);
        Kotak("Sandaran", pos + new Vector3(0f, 0.85f, -0.22f), new Vector3(1.6f, 0.5f, 0.08f), kayu);
        Kotak("KakiKi", pos + new Vector3(-0.6f, 0.25f, 0f), new Vector3(0.1f, 0.5f, 0.45f), kayu * 0.8f);
        Kotak("KakiKa", pos + new Vector3(0.6f, 0.25f, 0f), new Vector3(0.1f, 0.5f, 0.45f), kayu * 0.8f);
    }

    // ============ ORANG KOTA (pejalan kaki di luar; berjalan, sebagian melompat; kaki napak di tanah) ============
    void OrangKota()
    {
        if (!adaTanah) return;
        float gy = tanahB.max.y;                                  // permukaan tanah -> kaki napak (tidak melayang)
        float roadX = adaJalan ? jalanB.center.x : 0f;
        float halfRoad = adaJalan ? jalanB.extents.x : 5f;
        System.Random rnd = new System.Random(71);
        Color[] baju = {
            new Color(0.85f,0.35f,0.3f), new Color(0.3f,0.55f,0.85f), new Color(0.35f,0.65f,0.4f),
            new Color(0.9f,0.8f,0.4f), new Color(0.6f,0.45f,0.8f), new Color(0.9f,0.55f,0.3f),
            new Color(0.5f,0.7f,0.78f), new Color(0.82f,0.5f,0.6f)
        };

        // (A) Pejalan kaki di trotoar kiri & kanan jalan (berjalan searah jalan / sumbu z)
        if (adaJalan)
        {
            float trotoarX = halfRoad + 1.7f;
            float z0 = jalanB.center.z - jalanB.extents.z + 5f;
            float z1 = jalanB.center.z + jalanB.extents.z - 5f;
            if (z1 - z0 > 60f) { z0 = jalanB.center.z - 28f; z1 = jalanB.center.z + 32f; }
            for (float z = z0; z <= z1; z += 10f)
            {
                Vector3 ki = new Vector3(roadX - trotoarX, gy, z + (float)rnd.NextDouble() * 2f);
                Vector3 ka = new Vector3(roadX + trotoarX, gy, z + (float)rnd.NextDouble() * 2f);
                if (!KenaHindari(ki)) OrangJalan(ki, baju[rnd.Next(baju.Length)], Vector3.forward, 2.5f + (float)rnd.NextDouble() * 2f, 1);
                if (!KenaHindari(ka)) OrangJalan(ka, baju[rnd.Next(baju.Length)], Vector3.forward, 2.5f + (float)rnd.NextDouble() * 2f, 1);
            }
        }

        // (B) Orang berkeliaran di taman/rumput (hindari jalan & bangunan), sebagian MELOMPAT
        int buat = 0, coba = 0;
        while (buat < jumlahOrang && coba < jumlahOrang * 14)
        {
            coba++;
            float x = roadX + (float)(rnd.NextDouble() * 40 - 20);
            float z = jalanB.center.z + (float)(rnd.NextDouble() * 56 - 32);
            if (Mathf.Abs(x - roadX) < halfRoad + 2f) continue;   // jangan di tengah jalan
            Vector3 p = new Vector3(x, gy, z);
            if (KenaHindari(p)) continue;                         // jangan menembus bangunan/pos
            int m = (rnd.Next(4) == 0) ? 2 : 1;                   // 1 dari 4 melompat-lompat
            Vector3 arah = rnd.Next(2) == 0 ? Vector3.forward : Vector3.right;
            OrangJalan(p, baju[rnd.Next(baju.Length)], arah, 2f + (float)rnd.NextDouble() * 3f, m);
            buat++;
        }
    }

    void OrangJalan(Vector3 pos, Color warna, Vector3 arah, float jarak, int mode)
    {
        GameObject g = new GameObject("OrangKota");
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.position = pos;
        g.SetActive(false);                       // warna di-set sebelum RakitOrang.OnEnable membangun visual
        RakitOrang ro = g.AddComponent<RakitOrang>();
        ro.warna = warna;
        OrangPos op = g.AddComponent<OrangPos>();
        op.mode = mode; op.arah = arah; op.jarak = jarak; op.hadap = 0f;
        g.SetActive(true);
    }

    // ============ HELPER PRIMITIF ============
    Material Mat(Color c, float emisi)
    {
        Material m = new Material(shaderLit) { hideFlags = HideFlags.DontSave };
        m.color = c;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
        if (emisi > 0f)
        {
            m.EnableKeyword("_EMISSION");
            m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            m.SetColor("_EmissionColor", c * emisi);
        }
        return m;
    }

    GameObject Buat(PrimitiveType t, string nama, Vector3 pos, Vector3 skala, Color warna, float emisi)
    {
        GameObject g = GameObject.CreatePrimitive(t);
        g.name = nama;
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.localPosition = pos;
        g.transform.localScale = skala;
        g.GetComponent<Renderer>().sharedMaterial = Mat(warna, emisi);
        Collider c = g.GetComponent<Collider>();
        if (c != null) Hancur(c);
        return g;
    }

    GameObject Kotak(string n, Vector3 p, Vector3 s, Color c, float e = 0f) { return Buat(PrimitiveType.Cube, n, p, s, c, e); }
    GameObject Silinder(string n, Vector3 p, Vector3 s, Color c, float e = 0f) { return Buat(PrimitiveType.Cylinder, n, p, s, c, e); }
    GameObject Bola(string n, Vector3 p, Vector3 s, Color c, float e = 0f) { return Buat(PrimitiveType.Sphere, n, p, s, c, e); }

    void Update()
    {
        if (transform.Find("HiasKota") == null) { Bangun(); return; }
        if (!Application.isPlaying) return;
        float k = 1.3f + 0.35f * (Mathf.Sin(Time.time * 2.5f) * 0.5f + 0.5f);
        Color warm = new Color(1f, 0.85f, 0.5f);
        for (int i = 0; i < lampuMat.Count; i++)
            if (lampuMat[i] != null) lampuMat[i].SetColor("_EmissionColor", warm * k);
    }
}
