using UnityEngine;
using System.Collections.Generic;

// Membangun suasana KOTA di sisi lintasan Level 1 dari primitif + TEKSTUR PROSEDURAL (digambar via kode):
// deretan gedung bata beratap genteng + jendela menyala + lampu jalan; jalan & rumput diberi tekstur aspal/rumput.
// [ExecuteAlways] -> gedung tampil di editor; tekstur jalan/rumput & animasi lampu berjalan saat Play.
[ExecuteAlways]
public class RakitKota : MonoBehaviour
{
    public float mulaiZ = -12f;
    public float panjang = 280f;
    public float jarakGedung = 13f;

    Shader shaderLit;
    Transform root;
    Texture2D texBata, texGenteng, texAspal, texRumput, texJendela;
    readonly List<Material> lampuMat = new List<Material>();
    bool sudahTekstur = false;

    void OnEnable() { Bangun(); }

    void Hancur(Object o)
    {
        if (o == null) return;
        if (Application.isPlaying) Destroy(o); else DestroyImmediate(o);
    }

    void Bangun()
    {
        Transform lama = transform.Find("Kota");
        if (lama != null) Hancur(lama.gameObject);

        shaderLit = Shader.Find("Universal Render Pipeline/Lit");
        if (shaderLit == null) shaderLit = Shader.Find("Standard");

        texBata = GenTekstur(64, Bata);
        texGenteng = GenTekstur(64, Genteng);
        texAspal = GenTekstur(64, Aspal);
        texRumput = GenTekstur(64, Rumput);
        texJendela = GenTekstur(128, JendelaKota);
        lampuMat.Clear();
        sudahTekstur = false;

        GameObject r = new GameObject("Kota");
        r.hideFlags = HideFlags.DontSave;
        r.transform.SetParent(transform, false);
        root = r.transform;

        System.Random rnd = new System.Random(21);
        for (float z = mulaiZ; z <= mulaiZ + panjang; z += jarakGedung)
        {
            Gedung(rnd, -1, z, 13f);
            Gedung(rnd, 1, z, 13f);
            if (rnd.Next(2) == 0) Gedung(rnd, -1, z + jarakGedung * 0.5f, 21f);
            if (rnd.Next(2) == 0) Gedung(rnd, 1, z + jarakGedung * 0.5f, 21f);
        }
        for (float z = mulaiZ; z <= mulaiZ + panjang; z += 22f)
        {
            LampuJalan(-1, z);
            LampuJalan(1, z);
        }
    }

    // ============ TEKSTUR PROSEDURAL (via kode) ============
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

    Color Bata(int x, int y)
    {
        const int bw = 32, bh = 16, semen = 3;
        int row = y / bh;
        int lx = (x + (row % 2) * (bw / 2)) % bw;
        int ly = y % bh;
        if (ly < semen || lx < semen) return new Color(0.82f, 0.8f, 0.74f);
        float n = Mathf.PerlinNoise(x * 0.35f, y * 0.35f) * 0.12f;
        return new Color(0.62f + n, 0.30f + n * 0.6f, 0.24f + n * 0.4f);
    }

    Color Genteng(int x, int y)
    {
        const int th = 14;
        int ly = y % th;
        int lx = (x + ((y / th) % 2) * 8) % 16;
        float shade = 0.7f + 0.3f * ((float)ly / th);
        Color c = new Color(0.80f, 0.36f, 0.20f) * shade;
        if (ly < 2 || lx < 1) c *= 0.6f;
        c.a = 1f;
        return c;
    }

    Color Aspal(int x, int y)
    {
        float n = Mathf.PerlinNoise(x * 0.6f, y * 0.6f) * 0.14f;
        float sp = (((x * 17 + y * 29) % 23) / 23f) * 0.08f;
        float g = 0.18f + n + sp;
        return new Color(g, g, g + 0.015f);
    }

    Color Rumput(int x, int y)
    {
        float n = Mathf.PerlinNoise(x * 0.5f, y * 0.5f);
        float sp = (((x * 7 + y * 13) % 11) / 11f) * 0.08f;
        return Color.Lerp(new Color(0.28f, 0.5f, 0.22f), new Color(0.46f, 0.72f, 0.34f), n) + new Color(0f, sp, 0f);
    }

    // ============ MATERIAL & BENTUK ============
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
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
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

    Color WarnaBata(System.Random rnd)
    {
        Color[] c = {
            new Color(0.62f,0.5f,0.44f), new Color(0.7f,0.55f,0.48f),
            new Color(0.58f,0.46f,0.42f), new Color(0.68f,0.58f,0.5f)
        };
        return c[rnd.Next(c.Length)];
    }

    // fasad gedung: grid jendela, sebagian menyala (kuning/biru), sisanya gelap + rangka beton
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

    // material fasad: jendela sebagai albedo + emisi (jendela terang ikut menyala)
    Material MatGedung(Vector2 tiling)
    {
        Material m = new Material(shaderLit) { hideFlags = HideFlags.DontSave };
        m.color = Color.white;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", Color.white);
        m.mainTexture = texJendela;
        m.mainTextureScale = tiling;
        m.EnableKeyword("_EMISSION");
        m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        if (m.HasProperty("_EmissionMap")) { m.SetTexture("_EmissionMap", texJendela); m.SetTextureScale("_EmissionMap", tiling); }
        m.SetColor("_EmissionColor", Color.white * 0.5f);
        return m;
    }

    GameObject Silinder(string nama, Vector3 pos, Vector3 skala, Color warna)
    {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        g.name = nama;
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.localPosition = pos;
        g.transform.localScale = skala;
        g.GetComponent<Renderer>().sharedMaterial = Mat(warna);
        Collider c = g.GetComponent<Collider>();
        if (c != null) Hancur(c);
        return g;
    }

    Color TendaWarna(System.Random rnd)
    {
        Color[] c = { new Color(0.85f,0.3f,0.3f), new Color(0.3f,0.55f,0.8f), new Color(0.3f,0.6f,0.4f), new Color(0.85f,0.7f,0.3f) };
        return c[rnd.Next(c.Length)];
    }

    // atap genteng miring (dua bidang membentuk pelana)
    void AtapMiring(float x, float h, float z, float w, float d)
    {
        float sy = h + 0.35f;
        GameObject a = Kotak("AtapKi", new Vector3(x - w * 0.2f, sy, z), new Vector3(w * 0.62f, 0.14f, d + 0.3f), new Color(0.8f, 0.4f, 0.24f), 0f, texGenteng, new Vector2(w * 0.4f, d * 0.4f));
        a.transform.localRotation = Quaternion.Euler(0, 0, 34f);
        GameObject b = Kotak("AtapKa", new Vector3(x + w * 0.2f, sy, z), new Vector3(w * 0.62f, 0.14f, d + 0.3f), new Color(0.8f, 0.4f, 0.24f), 0f, texGenteng, new Vector2(w * 0.4f, d * 0.4f));
        b.transform.localRotation = Quaternion.Euler(0, 0, -34f);
    }

    // properti di atap gedung: tangki air / antena berlampu / kotak AC
    void AtapProps(System.Random rnd, float x, float h, float z, float w, float d)
    {
        int r = rnd.Next(4);
        if (r == 0)
        {
            Silinder("TangkiAir", new Vector3(x - w * 0.18f, h + 0.9f, z), new Vector3(1.1f, 0.55f, 1.1f), new Color(0.55f, 0.4f, 0.3f));
        }
        else if (r == 1)
        {
            Kotak("Antena", new Vector3(x, h + 1.4f, z), new Vector3(0.1f, 2.4f, 0.1f), new Color(0.2f, 0.2f, 0.22f));
            GameObject l = Kotak("LampuAntena", new Vector3(x, h + 2.6f, z), new Vector3(0.24f, 0.24f, 0.24f), new Color(1f, 0.2f, 0.2f), 1.6f);
            lampuMat.Add(l.GetComponent<Renderer>().sharedMaterial);
        }
        else if (r == 2)
        {
            Kotak("AC1", new Vector3(x + w * 0.2f, h + 0.5f, z - d * 0.2f), new Vector3(1f, 0.7f, 1f), new Color(0.5f, 0.5f, 0.54f));
            Kotak("AC2", new Vector3(x - w * 0.22f, h + 0.4f, z + d * 0.2f), new Vector3(0.8f, 0.5f, 0.8f), new Color(0.48f, 0.48f, 0.52f));
        }
    }

    void Gedung(System.Random rnd, int sisi, float z, float baseX)
    {
        int tipe = rnd.Next(3);
        float h = 6f + (float)rnd.NextDouble() * 9f;
        float w = 4f + (float)rnd.NextDouble() * 2.5f;
        float d = w * 0.92f;
        float x = sisi * (baseX + w * 0.5f);
        float faceX = x - sisi * (w * 0.5f);

        if (tipe == 1) // ruko bata: bata + atap genteng miring + tenda + papan nama
        {
            h = 4.5f + (float)rnd.NextDouble() * 2.5f;
            Kotak("Gedung", new Vector3(x, h * 0.5f, z), new Vector3(w, h, d), WarnaBata(rnd), 0f, texBata, new Vector2(w * 0.5f, h * 0.5f));
            AtapMiring(x, h, z, w, d);
            GameObject tenda = Kotak("Tenda", new Vector3(faceX - sisi * 0.5f, 2.3f, z), new Vector3(1f, 0.08f, d * 0.75f), TendaWarna(rnd));
            tenda.transform.localRotation = Quaternion.Euler(0, 0, sisi * 22f);
            Kotak("Papan", new Vector3(faceX - sisi * 0.05f, h * 0.72f, z), new Vector3(0.05f, 0.7f, d * 0.7f), new Color(0.9f, 0.85f, 0.4f), 0.3f);
        }
        else // menara kaca: fasad jendela menyala + lobby + atap datar + properti atap
        {
            int kolom = Mathf.Max(2, Mathf.RoundToInt(w / 1.5f));
            int lantai = Mathf.Max(4, Mathf.RoundToInt(h / 1.6f));
            GameObject bod = Kotak("Gedung", new Vector3(x, h * 0.5f, z), new Vector3(w, h, d), Color.white);
            bod.GetComponent<Renderer>().sharedMaterial = MatGedung(new Vector2(kolom, lantai));
            Kotak("Lobby", new Vector3(x, 1f, z), new Vector3(w + 0.15f, 2f, d + 0.15f), new Color(0.34f, 0.34f, 0.4f));
            Kotak("Atap", new Vector3(x, h + 0.12f, z), new Vector3(w + 0.25f, 0.25f, d + 0.25f), new Color(0.28f, 0.28f, 0.32f));
            AtapProps(rnd, x, h, z, w, d);
        }

        Kotak("Pintu", new Vector3(faceX - sisi * 0.04f, 0.9f, z), new Vector3(0.08f, 1.7f, 1.1f), new Color(0.18f, 0.15f, 0.14f));
    }

    void LampuJalan(int sisi, float z)
    {
        float x = sisi * 6.2f;
        Kotak("Tiang", new Vector3(x, 2f, z), new Vector3(0.2f, 4f, 0.2f), new Color(0.15f, 0.15f, 0.17f));
        Kotak("Lengan", new Vector3(x - sisi * 0.45f, 3.9f, z), new Vector3(1f, 0.15f, 0.15f), new Color(0.15f, 0.15f, 0.17f));
        GameObject lamp = Kotak("KepalaLampu", new Vector3(x - sisi * 0.85f, 3.75f, z), new Vector3(0.4f, 0.25f, 0.4f), new Color(1f, 0.85f, 0.5f), 1.6f);
        lampuMat.Add(lamp.GetComponent<Renderer>().sharedMaterial);
    }

    // ============ TEKSTUR JALAN/RUMPUT + ANIMASI (Play) ============
    void Update()
    {
        if (transform.Find("Kota") == null) { Bangun(); return; }
        if (!Application.isPlaying) return;

        if (!sudahTekstur)
        {
            sudahTekstur = true;
            SetTeksturObjek("Jalan", texAspal, new Vector2(4f, 90f));
            SetTeksturObjek("RumputKiri", texRumput, new Vector2(10f, 90f));
            SetTeksturObjek("RumputKanan", texRumput, new Vector2(10f, 90f));
        }

        // lampu jalan berdenyut halus
        float k = 1.3f + 0.4f * (Mathf.Sin(Time.time * 3f) * 0.5f + 0.5f);
        Color warm = new Color(1f, 0.85f, 0.5f);
        for (int i = 0; i < lampuMat.Count; i++)
            if (lampuMat[i] != null) lampuMat[i].SetColor("_EmissionColor", warm * k);
    }

    void SetTeksturObjek(string nama, Texture2D tex, Vector2 tiling)
    {
        GameObject g = GameObject.Find(nama);
        if (g == null) return;
        Renderer r = g.GetComponent<Renderer>();
        if (r == null) return;
        Material m = new Material(r.sharedMaterial) { hideFlags = HideFlags.DontSave };
        m.mainTexture = tex;
        m.mainTextureScale = tiling;
        r.sharedMaterial = m;
    }
}
