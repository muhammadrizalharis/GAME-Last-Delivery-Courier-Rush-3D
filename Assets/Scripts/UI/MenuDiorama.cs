using UnityEngine;
using System.Collections.Generic;

// Membangun diorama menu (5 kawasan) dari primitif + TEKSTUR PROSEDURAL (digambar via kode, bukan aset jadi:
// bata/dinding, genteng, aspal/jalan, rumput) + ANIMASI ringan (mobil & kereta jalan, matahari & orang bergerak,
// seluruh diorama bergoyang halus). [ExecuteAlways] -> tampil di editor juga. Objek DontSave (dibangun ulang tiap load).
[ExecuteAlways]
public class MenuDiorama : MonoBehaviour
{
    Shader shaderLit;
    Transform root;
    Texture2D texBata, texGenteng, texAspal, texRumput;

    // referensi untuk animasi (diisi saat Bangun)
    readonly List<Transform> mobil = new List<Transform>();
    readonly List<Transform> kereta = new List<Transform>();
    readonly List<float> keretaBaseX = new List<float>();
    readonly List<Transform> orang = new List<Transform>();
    readonly List<float> orangBaseY = new List<float>();
    Transform matahari;
    Material matMatahari;
    float matahariBaseY;

    void OnEnable() { Bangun(); }

    void Hancur(Object o)
    {
        if (o == null) return;
        if (Application.isPlaying) Destroy(o); else DestroyImmediate(o);
    }

    void Bangun()
    {
        Transform lama = transform.Find("Diorama3D");
        if (lama != null) Hancur(lama.gameObject);

        shaderLit = Shader.Find("Universal Render Pipeline/Lit");
        if (shaderLit == null) shaderLit = Shader.Find("Standard");

        BuatTekstur();

        mobil.Clear(); kereta.Clear(); keretaBaseX.Clear(); orang.Clear(); orangBaseY.Clear();
        matahari = null; matMatahari = null;

        GameObject r = new GameObject("Diorama3D");
        r.hideFlags = HideFlags.DontSave;
        r.transform.SetParent(transform, false);
        root = r.transform;

        BuatTanah();
        BuatKota();
        BuatRelKereta();
        BuatPasar();
        BuatTerowongan();
        BuatIndustri();
        BuatMatahari();
    }

    // ================= TEKSTUR PROSEDURAL (dibuat dari kode) =================
    void BuatTekstur()
    {
        texBata = GenTekstur(64, Bata);
        texGenteng = GenTekstur(64, Genteng);
        texAspal = GenTekstur(64, Aspal);
        texRumput = GenTekstur(64, Rumput);
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

    Color Bata(int x, int y)               // dinding: bata merah + garis semen
    {
        const int bw = 32, bh = 16, semen = 3;
        int row = y / bh;
        int lx = (x + (row % 2) * (bw / 2)) % bw;
        int ly = y % bh;
        if (ly < semen || lx < semen) return new Color(0.82f, 0.8f, 0.74f);
        float n = Mathf.PerlinNoise(x * 0.35f, y * 0.35f) * 0.12f;
        return new Color(0.62f + n, 0.30f + n * 0.6f, 0.24f + n * 0.4f);
    }

    Color Genteng(int x, int y)            // atap: genteng terakota bertumpuk
    {
        const int th = 14;
        int row = y / th;
        int ly = y % th;
        int lx = (x + (row % 2) * 8) % 16;
        float shade = 0.7f + 0.3f * ((float)ly / th);
        Color c = new Color(0.80f, 0.36f, 0.20f) * shade;
        if (ly < 2 || lx < 1) c *= 0.6f;
        c.a = 1f;
        return c;
    }

    Color Aspal(int x, int y)              // jalan: aspal abu berbintik
    {
        float n = Mathf.PerlinNoise(x * 0.6f, y * 0.6f) * 0.14f;
        float sp = (((x * 17 + y * 29) % 23) / 23f) * 0.08f;
        float g = 0.18f + n + sp;
        return new Color(g, g, g + 0.015f);
    }

    Color Rumput(int x, int y)             // tanah: rumput hijau berbintik
    {
        float n = Mathf.PerlinNoise(x * 0.5f, y * 0.5f);
        float sp = (((x * 7 + y * 13) % 11) / 11f) * 0.08f;
        return Color.Lerp(new Color(0.28f, 0.5f, 0.22f), new Color(0.46f, 0.72f, 0.34f), n) + new Color(0f, sp, 0f);
    }

    // ================= MATERIAL & BENTUK =================
    Material Mat(Color c, float emisi = 0f, Texture2D tex = null, Vector2 tiling = default)
    {
        Material m = new Material(shaderLit) { hideFlags = HideFlags.DontSave };
        m.color = c;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
        if (tex != null)
        {
            m.mainTexture = tex;
            m.mainTextureScale = (tiling == default(Vector2)) ? Vector2.one : tiling;
        }
        if (emisi > 0f)
        {
            m.EnableKeyword("_EMISSION");
            m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            m.SetColor("_EmissionColor", c * emisi);
        }
        return m;
    }

    GameObject Kotak(string nama, Vector3 pos, Vector3 skala, Color warna, float emisi = 0f, Texture2D tex = null, Vector2 tiling = default)
    {
        return Bentuk(PrimitiveType.Cube, nama, pos, skala, warna, emisi, tex, tiling);
    }

    GameObject Bentuk(PrimitiveType t, string nama, Vector3 pos, Vector3 skala, Color warna, float emisi = 0f, Texture2D tex = null, Vector2 tiling = default)
    {
        GameObject g = GameObject.CreatePrimitive(t);
        g.name = nama;
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.localPosition = pos;
        g.transform.localScale = skala;
        g.GetComponent<Renderer>().sharedMaterial = Mat(warna, emisi, tex, tiling);
        Collider c = g.GetComponent<Collider>();
        if (c != null) Hancur(c);
        return g;
    }

    // ================= ISI DIORAMA =================
    void BuatTanah()
    {
        Kotak("Tanah", new Vector3(0, -0.5f, 4), new Vector3(48, 1, 22), new Color(0.5f, 0.72f, 0.4f), 0f, texRumput, new Vector2(16, 8));
        Kotak("Dasar", new Vector3(0, 0.02f, 4), new Vector3(46, 0.1f, 20), new Color(0.6f, 0.6f, 0.62f), 0f, texAspal, new Vector2(14, 7));
    }

    Color BiruGedung(int i)
    {
        Color[] c = {
            new Color(0.62f,0.5f,0.44f), new Color(0.7f,0.55f,0.48f),
            new Color(0.58f,0.46f,0.42f), new Color(0.66f,0.52f,0.46f)
        };
        return c[i % c.Length];
    }

    void BuatKota()
    {
        float[] tinggi = { 5, 7, 4, 6, 8 };
        for (int i = 0; i < tinggi.Length; i++)
        {
            float x = -20 + i * 2.4f;
            float h = tinggi[i];
            Kotak("Gedung" + i, new Vector3(x, h / 2f, 9), new Vector3(2.2f, h, 2.2f), BiruGedung(i), 0f, texBata, new Vector2(1.6f, h * 0.6f));
            Kotak("Jdl" + i, new Vector3(x, h / 2f, 7.88f), new Vector3(1.6f, h * 0.8f, 0.05f), new Color(0.5f, 0.72f, 0.95f), 0.4f);
        }
        Kotak("JalanKota", new Vector3(-16, 0.12f, 4.5f), new Vector3(12, 0.12f, 3.2f), new Color(0.55f, 0.55f, 0.55f), 0f, texAspal, new Vector2(8, 2));
        mobil.Add(Kotak("Mobil1", new Vector3(-18, 0.6f, 4.5f), new Vector3(1.6f, 0.9f, 0.9f), new Color(0.85f, 0.3f, 0.25f)).transform);
        mobil.Add(Kotak("Mobil2", new Vector3(-14, 0.6f, 4.2f), new Vector3(1.6f, 0.9f, 0.9f), new Color(0.3f, 0.55f, 0.85f)).transform);
        Kotak("Krd1", new Vector3(-21, 0.5f, 4), new Vector3(1, 1, 1), new Color(0.78f, 0.6f, 0.36f));
        Kotak("Krd2", new Vector3(-21, 1.4f, 4), new Vector3(0.9f, 0.9f, 0.9f), new Color(0.82f, 0.64f, 0.4f));
    }

    void BuatRelKereta()
    {
        Kotak("RelDasar", new Vector3(-6, 0.14f, 12), new Vector3(14, 0.15f, 3), new Color(0.5f, 0.45f, 0.42f), 0f, texAspal, new Vector2(10, 2));
        Kotak("Rel1", new Vector3(-6, 0.25f, 11.2f), new Vector3(14, 0.12f, 0.2f), new Color(0.3f, 0.3f, 0.33f));
        Kotak("Rel2", new Vector3(-6, 0.25f, 12.8f), new Vector3(14, 0.12f, 0.2f), new Color(0.3f, 0.3f, 0.33f));
        kereta.Add(Kotak("Loko", new Vector3(-3, 0.9f, 12), new Vector3(3, 1.3f, 1.6f), new Color(0.35f, 0.42f, 0.55f)).transform); keretaBaseX.Add(-3f);
        kereta.Add(Kotak("Gerbong1", new Vector3(-6.5f, 0.9f, 12), new Vector3(3, 1.3f, 1.6f), new Color(0.5f, 0.3f, 0.3f)).transform); keretaBaseX.Add(-6.5f);
        kereta.Add(Kotak("Gerbong2", new Vector3(-10, 0.9f, 12), new Vector3(3, 1.3f, 1.6f), new Color(0.3f, 0.5f, 0.4f)).transform); keretaBaseX.Add(-10f);
        Kotak("TiangSinyal", new Vector3(-1, 1, 10.5f), new Vector3(0.2f, 2, 0.2f), new Color(0.9f, 0.9f, 0.9f));
        Kotak("LampuMerah", new Vector3(-1, 2, 10.5f), new Vector3(0.4f, 0.4f, 0.4f), new Color(0.9f, 0.2f, 0.2f), 0.6f);
    }

    void BuatPasar()
    {
        Kotak("AlasPasar", new Vector3(0, 0.13f, 3), new Vector3(9, 0.12f, 5), new Color(0.5f, 0.48f, 0.5f), 0f, texAspal, new Vector2(7, 4));
        Color[] kanopi = { new Color(0.85f,0.3f,0.3f), new Color(0.9f,0.8f,0.4f), new Color(0.3f,0.6f,0.8f) };
        for (int i = 0; i < 3; i++)
        {
            float x = -3 + i * 3f;
            Kotak("Meja" + i, new Vector3(x, 0.6f, 3), new Vector3(1.6f, 0.9f, 1.2f), new Color(0.6f, 0.42f, 0.28f));
            GameObject kan = Kotak("Kanopi" + i, new Vector3(x, 1.6f, 3), new Vector3(2f, 0.1f, 1.6f), kanopi[i]);
            kan.transform.localRotation = Quaternion.Euler(12, 0, 0);
            Kotak("Lampu" + i, new Vector3(x, 1.35f, 3), new Vector3(0.25f, 0.25f, 0.25f), new Color(1f, 0.8f, 0.4f), 1.2f);
        }
        Color[] baju = { new Color(0.9f,0.5f,0.4f), new Color(0.4f,0.6f,0.9f), new Color(0.5f,0.8f,0.5f), new Color(0.9f,0.85f,0.5f), new Color(0.7f,0.5f,0.8f) };
        System.Random rnd = new System.Random(7);
        for (int i = 0; i < 8; i++)
        {
            float x = -3.5f + (float)rnd.NextDouble() * 7f;
            float z = 1.2f + (float)rnd.NextDouble() * 3f;
            orang.Add(Bentuk(PrimitiveType.Capsule, "Orang" + i, new Vector3(x, 0.6f, z), new Vector3(0.32f, 0.5f, 0.32f), baju[i % baju.Length]).transform);
            orangBaseY.Add(0.6f);
        }
    }

    void BuatTerowongan()
    {
        Bentuk(PrimitiveType.Sphere, "Batu", new Vector3(9, 1.5f, 12), new Vector3(7, 5, 6), new Color(0.28f, 0.28f, 0.32f));
        Kotak("LubangTerowongan", new Vector3(9, 1.1f, 9.2f), new Vector3(2.4f, 2.2f, 1.2f), new Color(0.03f, 0.03f, 0.05f));
        Kotak("Truk", new Vector3(6.5f, 0.7f, 8), new Vector3(2.2f, 1.1f, 1.2f), new Color(0.85f, 0.75f, 0.4f));
    }

    void BuatIndustri()
    {
        Kotak("Pabrik", new Vector3(16, 2, 9), new Vector3(4.5f, 4, 4), new Color(0.62f, 0.5f, 0.44f), 0f, texBata, new Vector2(3, 2.4f));
        Kotak("AtapPabrik", new Vector3(16, 4.1f, 9), new Vector3(4.6f, 0.4f, 4.1f), new Color(0.8f, 0.4f, 0.24f), 0f, texGenteng, new Vector2(4, 3));
        Kotak("CraneTiang", new Vector3(13, 3, 12), new Vector3(0.3f, 6, 0.3f), new Color(0.9f, 0.75f, 0.2f));
        Kotak("CraneLengan", new Vector3(14.5f, 5.8f, 12), new Vector3(4, 0.3f, 0.3f), new Color(0.9f, 0.75f, 0.2f));
        Kotak("CraneKabel", new Vector3(16.3f, 5f, 12), new Vector3(0.05f, 1.6f, 0.05f), new Color(0.1f, 0.1f, 0.1f));
        Kotak("CraneKait", new Vector3(16.3f, 4.1f, 12), new Vector3(0.6f, 0.6f, 0.6f), new Color(0.78f, 0.6f, 0.36f));
        Color[] kont = { new Color(0.8f,0.3f,0.25f), new Color(0.3f,0.5f,0.8f), new Color(0.4f,0.7f,0.45f), new Color(0.85f,0.7f,0.3f) };
        for (int i = 0; i < 4; i++)
        {
            int baris = i % 2, kolom = i / 2;
            Kotak("Kontainer" + i, new Vector3(19.5f, 0.7f + baris * 1.4f, 6 + kolom * 2.6f), new Vector3(2.6f, 1.3f, 2.2f), kont[i]);
        }
    }

    void BuatMatahari()
    {
        GameObject s = Bentuk(PrimitiveType.Sphere, "Matahari", new Vector3(15, 11, 14), new Vector3(3, 3, 3), new Color(1f, 0.85f, 0.25f), 1.4f);
        matahari = s.transform;
        matahariBaseY = 11f;
        matMatahari = s.GetComponent<Renderer>().sharedMaterial;
    }

    // ================= ANIMASI (hanya saat Play) =================
    void Update()
    {
        if (transform.Find("Diorama3D") == null) { Bangun(); return; } // bangun ulang bila hilang
        if (!Application.isPlaying || root == null) return;

        float t = Time.unscaledTime;

        // seluruh diorama bergoyang halus (efek turntable bernafas)
        root.localRotation = Quaternion.Euler(0f, Mathf.Sin(t * 0.25f) * 6f, 0f);

        // mobil kota jalan bolak-balik di jalannya
        for (int i = 0; i < mobil.Count; i++)
        {
            if (mobil[i] == null) continue;
            Vector3 p = mobil[i].localPosition;
            p.x = -21f + Mathf.PingPong(t * 2.2f + i * 6f, 10f);
            mobil[i].localPosition = p;
        }

        // kereta meluncur pelan di rel
        float geser = Mathf.Sin(t * 0.5f) * 3.5f;
        for (int i = 0; i < kereta.Count; i++)
        {
            if (kereta[i] == null) continue;
            Vector3 p = kereta[i].localPosition;
            p.x = keretaBaseX[i] + geser;
            kereta[i].localPosition = p;
        }

        // matahari naik-turun + cahaya berdenyut
        if (matahari != null)
        {
            Vector3 p = matahari.localPosition;
            p.y = matahariBaseY + Mathf.Sin(t * 0.6f) * 0.5f;
            matahari.localPosition = p;
        }
        if (matMatahari != null)
        {
            float k = 1.1f + 0.5f * (Mathf.Sin(t * 2f) * 0.5f + 0.5f);
            matMatahari.SetColor("_EmissionColor", new Color(1f, 0.85f, 0.25f) * k);
        }

        // orang pasar bergoyang naik-turun
        for (int i = 0; i < orang.Count; i++)
        {
            if (orang[i] == null) continue;
            Vector3 p = orang[i].localPosition;
            p.y = orangBaseY[i] + Mathf.Abs(Mathf.Sin(t * 3f + i)) * 0.12f;
            orang[i].localPosition = p;
        }
    }
}
