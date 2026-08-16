using UnityEngine;
using System.Collections.Generic;

// Menjadikan bangunan POS jadi TOKO 2 LANTAI beratap: fasad kaca lantai-2 ditumpuk di atas dinding pos yang ada,
// + kornis pemisah, kanopi & jendela toko, papan nama, dan ATAP ber-collider (kamera anti-tembus berhenti di bawahnya).
// [ExecuteAlways] -> tampil di editor. Objek DontSave (dibangun ulang tiap load, tidak dobel).
[ExecuteAlways]
public class RenovasiToko : MonoBehaviour
{
    public float tinggiLantai2 = 6f;   // tinggi lantai kedua di atas dinding lama (bangunan lebih tinggi)

    Shader shaderLit;
    Transform root;
    Texture2D texJendela;

    static readonly string[] namaDinding = {
        "PosDindingKanan", "PosDindingBelakang", "PosDindingDepan", "PosDepanBawah", "PosDepanAtas"
    };

    void OnEnable() { Bangun(); }

    void Hancur(Object o)
    {
        if (o == null) return;
        if (Application.isPlaying) Destroy(o); else DestroyImmediate(o);
    }

    void Bangun()
    {
        Transform lama = transform.Find("RenovasiToko");
        if (lama != null) Hancur(lama.gameObject);

        shaderLit = Shader.Find("Universal Render Pipeline/Lit");
        if (shaderLit == null) shaderLit = Shader.Find("Standard");

        // kumpulkan dinding pos yang ada (bawah = lantai 1)
        List<Bounds> dinding = new List<Bounds>();
        foreach (string n in namaDinding)
        {
            GameObject g = GameObject.Find(n);
            if (g == null) continue;
            Renderer rd = g.GetComponent<Renderer>();
            if (rd != null) dinding.Add(rd.bounds);
        }
        if (dinding.Count == 0) return;   // dinding pos belum termuat -> Update coba lagi

        if (texJendela == null) texJendela = GenTekstur(128, JendelaKota);

        GameObject r = new GameObject("RenovasiToko");
        r.hideFlags = HideFlags.DontSave;
        r.transform.SetParent(transform, false);
        root = r.transform;

        Bounds gab = dinding[0];
        float wallTop = 4f;
        foreach (Bounds b in dinding)
        {
            gab.Encapsulate(b);
            wallTop = Mathf.Max(wallTop, b.max.y);
            // dinding lantai-2 (fasad kaca) menumpuk di atas tiap dinding; COLLIDER biar kamera & pemain tak tembus
            FasadKaca("FasadKaca2", new Vector3(b.center.x, b.max.y + tinggiLantai2 * 0.5f, b.center.z),
                      new Vector3(b.size.x + 0.12f, tinggiLantai2, b.size.z + 0.12f), true);
        }

        // isi dinding di atas PINTU (celah depan, sisi x-min, sekitar z 2.5..7)
        FasadKaca("FasadKacaAtasDepan", new Vector3(gab.min.x + 0.25f, wallTop + tinggiLantai2 * 0.5f, 4.75f),
                  new Vector3(0.4f, tinggiLantai2, 4.6f), true);

        float y2 = wallTop + tinggiLantai2;                 // puncak lantai 2 = tinggi atap
        Vector3 c = new Vector3(gab.center.x, 0f, gab.center.z);
        float lx = gab.size.x, lz = gab.size.z, fx = gab.min.x;

        // kornis pemisah antar lantai
        Kotak("Kornis", new Vector3(c.x, wallTop + 0.05f, c.z), new Vector3(lx + 0.5f, 0.3f, lz + 0.5f), new Color(0.86f, 0.84f, 0.76f), false);

        // ATAP datar (collider ON -> kamera berhenti di bawahnya) + parapet tepi
        GameObject atap = Kotak("TudungToko", new Vector3(c.x, y2 + 0.15f, c.z), new Vector3(lx + 0.6f, 0.3f, lz + 0.6f), new Color(0.5f, 0.22f, 0.18f), true);
        Renderer ar = atap.GetComponent<Renderer>();
        if (ar != null) ar.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; // biar dalam toko tetap terang
        float py = y2 + 0.5f;
        Color par = new Color(0.82f, 0.8f, 0.72f);
        Kotak("ParapetD", new Vector3(c.x, py, c.z - lz * 0.5f), new Vector3(lx + 0.6f, 0.5f, 0.3f), par, false);
        Kotak("ParapetB", new Vector3(c.x, py, c.z + lz * 0.5f), new Vector3(lx + 0.6f, 0.5f, 0.3f), par, false);
        Kotak("ParapetKi", new Vector3(c.x - lx * 0.5f, py, c.z), new Vector3(0.3f, 0.5f, lz + 0.6f), par, false);
        Kotak("ParapetKa", new Vector3(c.x + lx * 0.5f, py, c.z), new Vector3(0.3f, 0.5f, lz + 0.6f), par, false);

        // kanopi bergaris di atas pintu (depan, sisi x-min)
        Kotak("Kanopi", new Vector3(fx - 0.65f, wallTop - 0.6f, 4.75f), new Vector3(1.7f, 0.16f, 5.2f), new Color(0.75f, 0.2f, 0.2f), false);
        for (float z = 2.8f; z <= 7.2f; z += 1.1f)
            Kotak("KanopiGaris", new Vector3(fx - 0.65f, wallTop - 0.52f, z), new Vector3(1.72f, 0.1f, 0.4f), new Color(0.96f, 0.96f, 0.92f), false);

        // jendela toko lantai bawah (kaca besar) mengapit pintu
        JendelaBox("KacaTokoA", new Vector3(fx - 0.12f, 1.7f, 0.6f), new Vector3(0.25f, 2.4f, 3f));
        JendelaBox("KacaTokoB", new Vector3(fx - 0.12f, 1.7f, 10.5f), new Vector3(0.25f, 2.4f, 4f));

        // papan nama toko di kornis depan + TEKS "POS PAKET" huruf-balok (kubus solid -> tak tembus/kebalik, nempel di papan)
        Kotak("PapanToko", new Vector3(fx - 0.4f, wallTop + 0.35f, 4.75f), new Vector3(0.25f, 1.1f, 6.4f), new Color(0.14f, 0.32f, 0.58f), false);
        HurufBalok.Tulis(root, "POS PAKET", new Vector3(fx - 0.55f, wallTop + 0.35f, 4.75f), 0.62f, -1, Color.white);

        // ====== LANTAI 2 PENUH + ATRIUM (lubang tangga LEBAR; dari bawah lantai 2 terlihat) ======
        float mezTop = 4.3f;                                       // permukaan pijak lantai 2
        float minX = gab.min.x, maxX = gab.max.x, minZ = gab.min.z, maxZ = gab.max.z;
        float fx0 = minX + 0.55f, fx1 = maxX - 0.55f;             // tepi lantai (di dalam dinding)
        float fz0 = minZ + 0.55f, fz1 = maxZ - 0.55f;
        float mezXc = (fx0 + fx1) * 0.5f;

        // tangga menempel sisi kanan, naik ke +z
        float tangLebar = 2.8f, groundY = 0.25f;
        float tangX = maxX - 3.4f;
        int nAnak = 12;
        float stepH = (mezTop - groundY) / nAnak;
        float stepD = 0.5f;
        float run = nAnak * stepD;
        float naikZ0 = 3.6f, naikZ1 = naikZ0 + run;               // pangkal..puncak tangga
        // ATRIUM: lubang dibuat LEBAR (bukan cuma selebar tangga) -> void jelas di kiri tangga
        float hx0 = 16.8f, hx1 = tangX + tangLebar * 0.5f + 0.35f;
        float hz0 = 3.4f, hz1 = naikZ1;

        // LANTAI 2: 4 bidang mengelilingi lubang -> lantai PENUH (bukan setengah)
        LantaiBidang("Lantai2-Depan", fx0, fx1, fz0, hz0, mezTop);
        LantaiBidang("Lantai2-Belakang", fx0, fx1, hz1, fz1, mezTop);
        LantaiBidang("Lantai2-Kiri", fx0, hx0, hz0, hz1, mezTop);
        LantaiBidang("Lantai2-Kanan", hx1, fx1, hz0, hz1, mezTop);

        // kolom struktur (lantai 1 & 2) dekat dinding -> kesan rangka bangunan (spt referensi)
        float[] kolZ = { fz0 + 1.5f, (fz0 + fz1) * 0.5f, fz1 - 1.5f };
        foreach (float kz in kolZ) { Kolom(fx0 + 0.5f, kz, groundY, mezTop, y2); Kolom(fx1 - 0.5f, kz, groundY, mezTop, y2); }

        // TANGGA realistis: langkah solid (collider) + tread + stringer + pegangan
        Color batu = new Color(0.72f, 0.7f, 0.66f), batu2 = new Color(0.6f, 0.58f, 0.54f);
        for (int i = 0; i < nAnak; i++)
        {
            float topY = groundY + (i + 1) * stepH;
            float fz = naikZ0 + i * stepD;
            Kotak("AnakTangga", new Vector3(tangX, topY * 0.5f, fz + stepD * 0.5f), new Vector3(tangLebar, topY, stepD), batu, true);
            Kotak("Tread", new Vector3(tangX, topY - 0.04f, fz + stepD * 0.5f + 0.05f), new Vector3(tangLebar + 0.14f, 0.1f, stepD + 0.14f), batu2, false);
        }
        float sMidZ = naikZ0 + run * 0.5f, sMidY = (groundY + mezTop) * 0.5f;
        float sPjg = Mathf.Sqrt(run * run + (mezTop - groundY) * (mezTop - groundY)) + 0.4f;
        float sSudut = Mathf.Atan2(mezTop - groundY, run) * Mathf.Rad2Deg;
        Color kayuS = new Color(0.4f, 0.28f, 0.18f);
        GameObject stgKi = Kotak("StringerKi", new Vector3(tangX - tangLebar * 0.5f - 0.09f, sMidY, sMidZ), new Vector3(0.18f, 0.55f, sPjg), kayuS, true);
        stgKi.transform.rotation = Quaternion.Euler(-sSudut, 0f, 0f);
        GameObject stgKa = Kotak("StringerKa", new Vector3(tangX + tangLebar * 0.5f + 0.09f, sMidY, sMidZ), new Vector3(0.18f, 0.55f, sPjg), kayuS, true);
        stgKa.transform.rotation = Quaternion.Euler(-sSudut, 0f, 0f);
        PeganganTangga(tangX - tangLebar * 0.5f - 0.09f, naikZ0, groundY, mezTop, run, sSudut);
        PeganganTangga(tangX + tangLebar * 0.5f + 0.09f, naikZ0, groundY, mezTop, run, sSudut);

        // pagar keliling atrium (buka hanya di mulut tangga)
        PagarLubang(hx0, hx1, hz0, hz1, mezTop, tangX, tangLebar);

        // orang naik-turun tangga
        OrangTangga2(new Vector3(tangX - 0.55f, groundY + 0.15f, naikZ0 + 0.4f), new Vector3(tangX - 0.55f, mezTop + 0.1f, naikZ1 - 0.4f), new Color(0.3f, 0.5f, 0.8f), 1.15f);
        OrangTangga2(new Vector3(tangX + 0.55f, groundY + 0.15f, naikZ0 + 0.4f), new Vector3(tangX + 0.55f, mezTop + 0.1f, naikZ1 - 0.4f), new Color(0.82f, 0.5f, 0.3f), 0.9f);

        // ISI LANTAI 2 (di bidang solid, TIDAK di atas lubang): meja layanan + petugas + pekerja + rak/paket
        MejaLayanan2(new Vector3(mezXc, mezTop, fz1 - 2.3f));
        Orang2(new Vector3(fx0 + 2.5f, mezTop, fz1 - 3.2f), new Color(0.2f, 0.4f, 0.7f), 1, 0f, Vector3.forward, 2.2f);
        Orang2(new Vector3(fx0 + 2.5f, mezTop, (hz0 + hz1) * 0.5f), new Color(0.45f, 0.62f, 0.4f), 0, 90f, Vector3.forward, 0f);
        Rak2(new Vector3(fx0 + 1.8f, mezTop, fz1 - 0.9f));       // rak belakang-kiri
        Rak2(new Vector3(fx1 - 2f, mezTop, fz1 - 0.9f));         // rak belakang-kanan
        Rak2(new Vector3(fx0 + 1.5f, mezTop, fz0 + 1.6f));       // rak depan-kiri
        Rak2(new Vector3(fx0 + 1.5f, mezTop, 6.5f));             // rak strip kiri
        Rak2(new Vector3(fx1 - 2f, mezTop, fz0 + 1.8f));         // rak depan-kanan
        for (int i = 0; i < 3; i++)
            TumpukLantai2(new Vector3(fx0 + 1.4f + i * 0.95f, mezTop, fz1 - 2.6f));
        TumpukLantai2(new Vector3(fx1 - 1.6f, mezTop, fz0 + 1f));
        TumpukLantai2(new Vector3(fx0 + 1.4f, mezTop, fz1 - 4.5f));
    }

    // ---- fasad kaca lantai-2 (jendela emissive dari tekstur) ----
    void FasadKaca(string n, Vector3 pos, Vector3 skala, bool pakaiCollider = false)
    {
        float wPanel = Mathf.Max(skala.x, skala.z);
        Vector2 tiling = new Vector2(Mathf.Max(2f, wPanel / 1.5f), Mathf.Max(2f, skala.y / 1.4f));
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        g.name = n; g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.position = pos; g.transform.localScale = skala;
        g.GetComponent<Renderer>().sharedMaterial = MatFasad(tiling);
        Collider col = g.GetComponent<Collider>();
        if (!pakaiCollider && col != null) Hancur(col);
    }

    // ---- kaca polos emissive (jendela toko) ----
    void JendelaBox(string n, Vector3 pos, Vector3 skala)
    {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        g.name = n; g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.position = pos; g.transform.localScale = skala;
        Material m = Mat(new Color(0.6f, 0.82f, 0.95f));
        m.EnableKeyword("_EMISSION");
        m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        m.SetColor("_EmissionColor", new Color(0.45f, 0.7f, 0.95f) * 0.45f);
        g.GetComponent<Renderer>().sharedMaterial = m;
        Collider col = g.GetComponent<Collider>(); if (col != null) Hancur(col);
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
        m.SetColor("_EmissionColor", Color.white * 0.4f);
        return m;
    }

    Texture2D GenTekstur(int n, System.Func<int, int, Color> pola)
    {
        Texture2D t = new Texture2D(n, n) { wrapMode = TextureWrapMode.Repeat, hideFlags = HideFlags.DontSave };
        Color[] px = new Color[n * n];
        for (int y = 0; y < n; y++)
            for (int x = 0; x < n; x++)
                px[y * n + x] = pola(x, y);
        t.SetPixels(px); t.Apply();
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

    Material Mat(Color col)
    {
        Material m = new Material(shaderLit) { hideFlags = HideFlags.DontSave };
        m.color = col;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", col);
        return m;
    }

    GameObject Kotak(string n, Vector3 pos, Vector3 skala, Color col, bool pakaiCollider)
    {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        g.name = n; g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.position = pos; g.transform.localScale = skala;
        g.GetComponent<Renderer>().sharedMaterial = Mat(col);
        Collider col2 = g.GetComponent<Collider>();
        if (!pakaiCollider && col2 != null) Hancur(col2);
        return g;
    }

    // ---- pegangan tangga (baluster tegak + handrail miring) di sisi x=sx ----
    void PeganganTangga(float sx, float z0, float y0, float y1, float run, float sudut)
    {
        Color kayu = new Color(0.46f, 0.31f, 0.2f);
        int n = 6;
        for (int i = 0; i <= n; i++)
        {
            float t = i / (float)n;
            Kotak("Baluster", new Vector3(sx, y0 + t * (y1 - y0) + 0.5f, z0 + t * run), new Vector3(0.08f, 1f, 0.08f), kayu, false);
        }
        float pjg = Mathf.Sqrt(run * run + (y1 - y0) * (y1 - y0)) + 0.3f;
        GameObject hr = Kotak("Handrail", new Vector3(sx, (y0 + y1) * 0.5f + 1f, z0 + run * 0.5f), new Vector3(0.1f, 0.1f, pjg), kayu, false);
        hr.transform.rotation = Quaternion.Euler(-sudut, 0f, 0f);
    }

    // ---- lantai 2 (satu bidang) dengan collider; shadow off biar lantai 1 tak gelap ----
    void LantaiBidang(string n, float x0, float x1, float z0, float z1, float top)
    {
        if (x1 - x0 < 0.1f || z1 - z0 < 0.1f) return;
        GameObject g = Kotak(n, new Vector3((x0 + x1) * 0.5f, top - 0.15f, (z0 + z1) * 0.5f), new Vector3(x1 - x0, 0.3f, z1 - z0), new Color(0.62f, 0.47f, 0.33f), true);
        Renderer r = g.GetComponent<Renderer>();
        if (r != null) r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    // ---- kolom struktur beton (segmen lantai 1 + lantai 2) ----
    void Kolom(float x, float z, float y0, float y1, float yAtap)
    {
        Color beton = new Color(0.66f, 0.64f, 0.6f);
        Kotak("Kolom1", new Vector3(x, (y0 + y1) * 0.5f, z), new Vector3(0.35f, y1 - y0, 0.35f), beton, false);
        if (yAtap > y1) Kotak("Kolom2", new Vector3(x, (y1 + yAtap) * 0.5f, z), new Vector3(0.35f, yAtap - y1, 0.35f), beton, false);
    }

    // ---- pagar keliling LUBANG tangga; sisi +z (mulut tangga) dibuka ----
    void PagarLubang(float x0, float x1, float z0, float z1, float top, float tangX, float tangLebar)
    {
        Color kayu = new Color(0.46f, 0.31f, 0.2f);
        PagarSegmen(new Vector3((x0 + x1) * 0.5f, top, z0), new Vector3(x1 - x0 + 0.12f, 1f, 0.12f), kayu);   // sisi belakang lubang
        PagarSegmen(new Vector3(x0, top, (z0 + z1) * 0.5f), new Vector3(0.12f, 1f, z1 - z0), kayu);           // sisi kiri
        PagarSegmen(new Vector3(x1, top, (z0 + z1) * 0.5f), new Vector3(0.12f, 1f, z1 - z0), kayu);           // sisi kanan
        float gapKi = tangX - tangLebar * 0.5f - 0.1f, gapKa = tangX + tangLebar * 0.5f + 0.1f;               // mulut tangga (dibuka)
        if (gapKi - x0 > 0.25f) PagarSegmen(new Vector3((x0 + gapKi) * 0.5f, top, z1), new Vector3(gapKi - x0, 1f, 0.12f), kayu);
        if (x1 - gapKa > 0.25f) PagarSegmen(new Vector3((gapKa + x1) * 0.5f, top, z1), new Vector3(x1 - gapKa, 1f, 0.12f), kayu);
    }

    void PagarSegmen(Vector3 c, Vector3 s, Color kayu)
    {
        Kotak("PagarBawah", c + new Vector3(0f, 0.25f, 0f), new Vector3(s.x, 0.5f, s.z), kayu, true);
        Kotak("PagarAtas", c + new Vector3(0f, 0.92f, 0f), new Vector3(s.x + 0.03f, 0.12f, s.z + 0.03f), kayu, true);
    }

    // ---- orang di lantai 2 (RakitOrang visual + OrangPos perilaku) ----
    void Orang2(Vector3 pos, Color warna, int mode, float hadap, Vector3 arah, float jarak)
    {
        GameObject g = new GameObject("OrangLantai2");
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.position = pos;
        g.SetActive(false);                       // warna di-set sebelum RakitOrang.OnEnable membangun
        RakitOrang ro = g.AddComponent<RakitOrang>(); ro.warna = warna;
        OrangPos op = g.AddComponent<OrangPos>(); op.mode = mode; op.hadap = hadap; op.arah = arah; op.jarak = jarak;
        g.SetActive(true);
    }

    // ---- tumpukan paket & rak di lantai 2 ----
    void TumpukLantai2(Vector3 pos)
    {
        Color k = new Color(0.76f, 0.56f, 0.34f), k2 = new Color(0.68f, 0.5f, 0.3f);
        for (int i = 0; i < 3; i++)
            Kotak("PaketAtas", pos + new Vector3(((i % 2) - 0.5f) * 0.12f, 0.3f + i * 0.55f, 0f), new Vector3(0.72f, 0.5f, 0.72f), (i % 2 == 0) ? k : k2, false);
    }

    void Rak2(Vector3 pos)
    {
        Color gelap = new Color(0.35f, 0.28f, 0.22f), kayu = new Color(0.55f, 0.4f, 0.26f);
        float w = 2.6f, h = 2.4f, d = 0.9f;
        Kotak("Rak2Ki", pos + new Vector3(-w * 0.5f, h * 0.5f, 0f), new Vector3(0.12f, h, d), gelap, false);
        Kotak("Rak2Ka", pos + new Vector3(w * 0.5f, h * 0.5f, 0f), new Vector3(0.12f, h, d), gelap, false);
        for (int lv = 0; lv < 3; lv++)
        {
            float sy = 0.5f + lv * 0.85f;
            Kotak("Rak2Papan", pos + new Vector3(0f, sy, 0f), new Vector3(w, 0.08f, d), kayu, false);
            for (int i = -1; i <= 1; i++)
                Kotak("PaketAtas", pos + new Vector3(i * 0.8f, sy + 0.32f, 0f), new Vector3(0.6f, 0.5f, 0.62f), new Color(0.74f, 0.55f, 0.33f), false);
        }
    }

    // ---- meja layanan lantai 2 (konter + 2 petugas + 1 pelanggan) spt lantai bawah ----
    void MejaLayanan2(Vector3 c)
    {
        Color kayu = new Color(0.5f, 0.36f, 0.22f), atas = new Color(0.62f, 0.46f, 0.3f);
        Color seragam = new Color(0.15f, 0.35f, 0.62f);
        Kotak("Meja2", c + new Vector3(0f, 0.55f, 0f), new Vector3(3.2f, 1.1f, 1.1f), kayu, true);
        Kotak("Meja2Atas", c + new Vector3(0f, 1.13f, 0f), new Vector3(3.5f, 0.12f, 1.4f), atas, false);
        // komputer di konter lantai 2
        // komputer konter lantai 2: layar hadap PETUGAS (+z), keyboard di sisi petugas
        Kotak("Monitor2", c + new Vector3(0.9f, 1.55f, 0.08f), new Vector3(0.6f, 0.44f, 0.07f), new Color(0.12f, 0.12f, 0.14f), false);
        Kotak("Layar2", c + new Vector3(0.9f, 1.55f, 0.118f), new Vector3(0.5f, 0.34f, 0.02f), new Color(0.42f, 0.68f, 0.85f), false);
        Kotak("StandLeher2", c + new Vector3(0.9f, 1.3f, 0.08f), new Vector3(0.08f, 0.2f, 0.08f), new Color(0.12f, 0.12f, 0.14f), false);
        Kotak("StandKaki2", c + new Vector3(0.9f, 1.2f, 0.08f), new Vector3(0.28f, 0.04f, 0.32f), new Color(0.12f, 0.12f, 0.14f), false);
        Kotak("Keyboard2", c + new Vector3(0.9f, 1.2f, 0.36f), new Vector3(0.5f, 0.05f, 0.22f), new Color(0.16f, 0.16f, 0.2f), false);
        Orang2(c + new Vector3(-0.8f, 0f, 0.95f), seragam, 0, 180f, Vector3.forward, 0f);   // petugas belakang meja
        Orang2(c + new Vector3(0.8f, 0f, 0.95f), seragam, 0, 180f, Vector3.forward, 0f);
        Orang2(c + new Vector3(0f, 0f, -1.3f), new Color(0.85f, 0.5f, 0.3f), 0, 0f, Vector3.forward, 0f);  // pelanggan depan meja
    }

    // ---- orang naik-turun tangga (RakitOrang visual + OrangTangga gerak diagonal) ----
    void OrangTangga2(Vector3 bawah, Vector3 atas, Color warna, float kec)
    {
        GameObject g = new GameObject("OrangTangga");
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.position = bawah;
        g.SetActive(false);
        RakitOrang ro = g.AddComponent<RakitOrang>(); ro.warna = warna;
        OrangTangga ot = g.AddComponent<OrangTangga>(); ot.bawah = bawah; ot.atas = atas; ot.kecepatan = kec;
        g.SetActive(true);
    }

    void Update()
    {
        if (transform.Find("RenovasiToko") == null) Bangun();   // bangun ulang bila hilang (mis. dinding baru termuat)
    }
}
