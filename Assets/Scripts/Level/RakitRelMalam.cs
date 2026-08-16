using UnityEngine;
using System.Collections.Generic;

// Lingkungan KERETA MALAM (Level 2): mengubah "ruang hitam kosong" jadi jalur kereta perkotaan malam yang hidup.
// Skyline gedung berjendela menyala di kejauhan + jalan layang/embankment di atas dinding (pagar, lampu, ORANG menonton)
// + tiang listrik (catenary) + kabel + sinyal merah/hijau + bulan & bintang. Semua kode + primitif + tekstur prosedural.
// [ExecuteAlways] -> tampil di editor. Objek DontSave (dibangun ulang tiap load, tak dobel).
[ExecuteAlways]
public class RakitRelMalam : MonoBehaviour
{
    public float mulaiZ = -20f;
    public float panjang = 380f;
    public float tepiX = 6.3f;        // posisi dinding pembatas (lingkungan dibangun di luar ini)
    public float atasDinding = 2.9f;  // tinggi puncak dinding = permukaan jalan atas

    Shader shaderLit;
    Transform root;
    Texture2D texJendela;
    readonly List<Material> lampuMat = new List<Material>();

    void OnEnable() { Bangun(); }

    void Hancur(Object o)
    {
        if (o == null) return;
        if (Application.isPlaying) Destroy(o); else DestroyImmediate(o);
    }

    void Bangun()
    {
        Transform lama = transform.Find("RelMalam");
        if (lama != null) Hancur(lama.gameObject);

        shaderLit = Shader.Find("Universal Render Pipeline/Lit");
        if (shaderLit == null) shaderLit = Shader.Find("Standard");
        if (texJendela == null) texJendela = GenTekstur(128, JendelaMalam);
        lampuMat.Clear();

        GameObject r = new GameObject("RelMalam");
        r.hideFlags = HideFlags.DontSave;
        r.transform.SetParent(transform, false);
        root = r.transform;

        System.Random rnd = new System.Random(202);
        // skyline gedung (2 baris tiap sisi)
        for (float z = mulaiZ; z <= mulaiZ + panjang; z += 13f)
        {
            Gedung(rnd, -1, z, tepiX + 5.5f);
            Gedung(rnd, 1, z, tepiX + 5.5f);
            if (rnd.Next(2) == 0) Gedung(rnd, -1, z + 6.5f, tepiX + 13f);
            if (rnd.Next(2) == 0) Gedung(rnd, 1, z + 6.5f, tepiX + 13f);
        }

        JalanAtas(-1);
        JalanAtas(1);

        for (float z = mulaiZ + 12f; z <= mulaiZ + panjang; z += 24f)
            Catenary(z);

        Sinyal(new Vector3(-tepiX - 0.7f, atasDinding, mulaiZ + 55f), true);
        Sinyal(new Vector3(tepiX + 0.7f, atasDinding, mulaiZ + 150f), false);
        Sinyal(new Vector3(-tepiX - 0.7f, atasDinding, mulaiZ + 245f), true);

        Langit();
    }

    // ============ SKYLINE GEDUNG ============
    void Gedung(System.Random rnd, int sisi, float z, float xDasar)
    {
        float tinggi = 8f + (float)rnd.NextDouble() * 16f;
        float lebar = 4f + (float)rnd.NextDouble() * 4f;
        float dalam = 4f + (float)rnd.NextDouble() * 3f;
        float x = sisi * (xDasar + (float)rnd.NextDouble() * 3f);
        Vector2 tiling = new Vector2(Mathf.Max(2f, lebar / 1.8f), Mathf.Max(3f, tinggi / 1.8f));
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        g.name = "GedungMalam"; g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.localPosition = new Vector3(x, tinggi * 0.5f, z);
        g.transform.localScale = new Vector3(lebar, tinggi, dalam);
        g.GetComponent<Renderer>().sharedMaterial = MatGedung(tiling);
        Collider c = g.GetComponent<Collider>(); if (c != null) Hancur(c);
        // atap gelap
        Kotak("AtapMalam", new Vector3(x, tinggi + 0.15f, z), new Vector3(lebar + 0.3f, 0.3f, dalam + 0.3f), new Color(0.05f, 0.06f, 0.09f), 0f);
        if (rnd.Next(3) == 0)
        {
            GameObject l = Kotak("LampuAtap", new Vector3(x, tinggi + 0.9f, z), new Vector3(0.2f, 0.2f, 0.2f), new Color(1f, 0.2f, 0.2f), 1.8f);
            lampuMat.Add(l.GetComponent<Renderer>().sharedMaterial);
        }
    }

    // ============ JALAN LAYANG / EMBANKMENT + PAGAR + LAMPU + ORANG ============
    void JalanAtas(int sisi)
    {
        float x0 = sisi * (tepiX + 0.05f);        // tepi dalam (di atas dinding)
        float lebarJalan = 5.5f;
        float xTengah = sisi * (tepiX + lebarJalan * 0.5f);
        float y = atasDinding;

        // permukaan jalan
        Kotak("JalanAtas", new Vector3(xTengah, y - 0.15f, mulaiZ + panjang * 0.5f), new Vector3(lebarJalan, 0.3f, panjang), new Color(0.14f, 0.14f, 0.17f), 0f);
        // pagar pembatas di tepi jurang (menghadap rel)
        Kotak("PagarBawah", new Vector3(x0, y + 0.45f, mulaiZ + panjang * 0.5f), new Vector3(0.12f, 0.5f, panjang), new Color(0.5f, 0.5f, 0.55f), 0f);
        Kotak("PagarAtas", new Vector3(x0, y + 0.95f, mulaiZ + panjang * 0.5f), new Vector3(0.16f, 0.12f, panjang), new Color(0.6f, 0.6f, 0.66f), 0f);
        for (float z = mulaiZ; z <= mulaiZ + panjang; z += 9f)
            Kotak("Tiang", new Vector3(x0, y + 0.5f, z), new Vector3(0.1f, 0.9f, 0.1f), new Color(0.45f, 0.45f, 0.5f), 0f);

        // lampu jalan tiap 18 m
        for (float z = mulaiZ + 8f; z <= mulaiZ + panjang - 8f; z += 18f)
            LampuJalan(new Vector3(sisi * (tepiX + 1.4f), y, z));

        // ORANG menonton / lalu-lalang di jalan atas (di luar jalur main -> tak menabrak)
        System.Random rnd = new System.Random(sisi > 0 ? 311 : 733);
        for (float z = mulaiZ + 16f; z <= mulaiZ + panjang - 16f; z += 26f)
        {
            float ox = sisi * (tepiX + 1.1f + (float)rnd.NextDouble() * 2.5f);
            int mode = rnd.Next(3) == 0 ? 1 : 0;                     // sebagian jalan, sebagian berdiri menonton
            float hadap = mode == 0 ? (sisi > 0 ? -90f : 90f) : 0f;  // yang berdiri menghadap rel
            Orang(new Vector3(ox, y, z), WarnaBaju(rnd), mode, hadap, Vector3.forward, 2.5f + (float)rnd.NextDouble() * 2f);
        }
    }

    void LampuJalan(Vector3 dasar)
    {
        Color gelap = new Color(0.14f, 0.14f, 0.17f);
        Kotak("TiangLampu", dasar + Vector3.up * 2.4f, new Vector3(0.18f, 4.8f, 0.18f), gelap, 0f);
        float arah = dasar.x > 0 ? -1f : 1f;
        Kotak("LenganLampu", dasar + new Vector3(arah * 0.6f, 4.7f, 0f), new Vector3(1.3f, 0.14f, 0.14f), gelap, 0f);
        GameObject k = Bola("KepalaLampu", dasar + new Vector3(arah * 1.15f, 4.55f, 0f), new Vector3(0.45f, 0.32f, 0.45f), new Color(1f, 0.86f, 0.55f), 1.7f);
        lampuMat.Add(k.GetComponent<Renderer>().sharedMaterial);
    }

    // ============ CATENARY (tiang listrik + palang + kabel) ============
    void Catenary(float z)
    {
        Color besi = new Color(0.22f, 0.23f, 0.26f);
        float h = 5.6f;
        Kotak("TiangKiri", new Vector3(-tepiX + 0.5f, h * 0.5f, z), new Vector3(0.22f, h, 0.22f), besi, 0f);
        Kotak("TiangKanan", new Vector3(tepiX - 0.5f, h * 0.5f, z), new Vector3(0.22f, h, 0.22f), besi, 0f);
        Kotak("PalangAtas", new Vector3(0f, h, z), new Vector3((tepiX - 0.5f) * 2f, 0.16f, 0.16f), besi, 0f);
        // kabel kontak membujur di atas tiap jalur
        foreach (float lane in new float[] { -3f, 0f, 3f })
            Kotak("Kabel", new Vector3(lane, h - 0.5f, z), new Vector3(0.04f, 0.04f, 24f), new Color(0.1f, 0.1f, 0.12f), 0f);
    }

    // ============ SINYAL KERETA (mast + lampu merah/hijau) ============
    void Sinyal(Vector3 dasar, bool hijau)
    {
        Color besi = new Color(0.18f, 0.19f, 0.22f);
        Kotak("MastSinyal", dasar + Vector3.up * 2.2f, new Vector3(0.2f, 4.4f, 0.2f), besi, 0f);
        Kotak("KotakSinyal", dasar + new Vector3(0f, 4.2f, 0f), new Vector3(0.5f, 1.3f, 0.35f), new Color(0.1f, 0.1f, 0.12f), 0f);
        GameObject merah = Bola("LampuMerah", dasar + new Vector3(0f, 4.6f, 0.2f), new Vector3(0.26f, 0.26f, 0.14f), new Color(1f, 0.15f, 0.12f), hijau ? 0.2f : 2.2f);
        GameObject hij = Bola("LampuHijau", dasar + new Vector3(0f, 3.95f, 0.2f), new Vector3(0.26f, 0.26f, 0.14f), new Color(0.2f, 1f, 0.3f), hijau ? 2.2f : 0.2f);
    }

    // ============ LANGIT (bulan + bintang) ============
    void Langit()
    {
        float zc = mulaiZ + panjang * 0.6f;
        Bola("Bulan", new Vector3(-40f, 34f, zc + 60f), new Vector3(9f, 9f, 9f), new Color(0.95f, 0.95f, 0.85f), 1.3f);
        System.Random rnd = new System.Random(9);
        for (int i = 0; i < 60; i++)
        {
            float x = (float)(rnd.NextDouble() * 120 - 60);
            float yy = 22f + (float)rnd.NextDouble() * 26f;
            float z = mulaiZ + (float)rnd.NextDouble() * panjang;
            Bola("Bintang", new Vector3(x, yy, z + 80f), Vector3.one * (0.25f + (float)rnd.NextDouble() * 0.3f), new Color(0.9f, 0.95f, 1f), 1.4f);
        }
    }

    // ============ ORANG (RakitOrang + OrangPos) ============
    void Orang(Vector3 pos, Color warna, int mode, float hadap, Vector3 arah, float jarak)
    {
        GameObject g = new GameObject("OrangRel");
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.position = pos;
        g.SetActive(false);
        RakitOrang ro = g.AddComponent<RakitOrang>(); ro.warna = warna;
        OrangPos op = g.AddComponent<OrangPos>(); op.mode = mode; op.hadap = hadap; op.arah = arah; op.jarak = jarak;
        g.SetActive(true);
    }

    Color WarnaBaju(System.Random rnd)
    {
        Color[] baju = {
            new Color(0.8f,0.35f,0.32f), new Color(0.3f,0.5f,0.8f), new Color(0.35f,0.6f,0.42f),
            new Color(0.85f,0.75f,0.4f), new Color(0.6f,0.45f,0.78f), new Color(0.55f,0.68f,0.75f)
        };
        return baju[rnd.Next(baju.Length)];
    }

    // ============ TEKSTUR & HELPER ============
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

    Color JendelaMalam(int x, int y)
    {
        const int cell = 16, bingkai = 4;
        int cx = x / cell, cy = y / cell;
        int lx = x % cell, ly = y % cell;
        if (lx < bingkai || ly < bingkai) return new Color(0.06f, 0.07f, 0.1f);   // bingkai gelap
        int hsh = (cx * 73856093) ^ (cy * 19349663) ^ (cx * cy * 83492791);
        if ((hsh & 7) > 3)                                                        // ~separuh jendela menyala
            return ((hsh >> 3) & 1) == 0 ? new Color(1f, 0.85f, 0.5f) : new Color(0.6f, 0.8f, 1f);
        return new Color(0.04f, 0.05f, 0.08f);
    }

    Material MatGedung(Vector2 tiling)
    {
        Material m = new Material(shaderLit) { hideFlags = HideFlags.DontSave };
        Color dasar = new Color(0.1f, 0.11f, 0.14f);
        m.color = dasar;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", dasar);
        m.mainTexture = texJendela;
        m.mainTextureScale = tiling;
        m.EnableKeyword("_EMISSION");
        m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        if (m.HasProperty("_EmissionMap")) { m.SetTexture("_EmissionMap", texJendela); m.SetTextureScale("_EmissionMap", tiling); }
        m.SetColor("_EmissionColor", Color.white * 0.6f);
        return m;
    }

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

    GameObject Buat(PrimitiveType t, string nama, Vector3 pos, Vector3 skala, Color c, float emisi)
    {
        GameObject g = GameObject.CreatePrimitive(t);
        g.name = nama; g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.localPosition = pos; g.transform.localScale = skala;
        g.GetComponent<Renderer>().sharedMaterial = Mat(c, emisi);
        Collider col = g.GetComponent<Collider>(); if (col != null) Hancur(col);
        return g;
    }

    GameObject Kotak(string n, Vector3 p, Vector3 s, Color c, float e) { return Buat(PrimitiveType.Cube, n, p, s, c, e); }
    GameObject Bola(string n, Vector3 p, Vector3 s, Color c, float e) { return Buat(PrimitiveType.Sphere, n, p, s, c, e); }

    void Update()
    {
        if (transform.Find("RelMalam") == null) { Bangun(); return; }
        if (!Application.isPlaying) return;
        float k = 1.4f + 0.4f * Mathf.Sin(Time.time * 2f);
        Color warm = new Color(1f, 0.85f, 0.55f);
        for (int i = 0; i < lampuMat.Count; i++)
            if (lampuMat[i] != null) lampuMat[i].SetColor("_EmissionColor", warm * k);
    }
}
