using UnityEngine;

// Memberi TEKSTUR PROSEDURAL (digambar via kode) ke objek lingkungan scene berdasarkan NAMA objek:
// gedung -> fasad kaca berjendela (sebagian menyala), pos/dinding/pintu -> bata, atap -> genteng,
// jalan/trotoar/lantai -> aspal, tanah/rumput -> rumput. Play-only (tidak mengubah data scene tersimpan).
// Cukup pasang 1 komponen ini di scene; tidak perlu mengatur objek satu per satu.
public class TeksturKawasan : MonoBehaviour
{
    Shader shaderLit;
    Texture2D texBata, texGenteng, texAspal, texRumput, texJendela;

    void Start()
    {
        shaderLit = Shader.Find("Universal Render Pipeline/Lit");
        if (shaderLit == null) shaderLit = Shader.Find("Standard");
        texBata = Gen(64, Bata);
        texGenteng = Gen(64, Genteng);
        texAspal = Gen(64, Aspal);
        texRumput = Gen(64, Rumput);
        texJendela = Gen(128, Jendela);
        Terapkan();
    }

    void Terapkan()
    {
        foreach (MeshRenderer r in FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None))
        {
            if (r == null) continue;
            string n = r.gameObject.name.ToLower();
            if (n.Contains("kurir") || n.Contains("paket") || n.Contains("penjaga") ||
                n.Contains("papan") || n.Contains("tembok") || n.Contains("meja") || n.Contains("box"))
                continue;

            Vector3 s = r.transform.lossyScale;
            float lebar = Mathf.Max(Mathf.Abs(s.x), Mathf.Abs(s.z));

            if (n.Contains("gedung") || n.Contains("menara"))
                Fasad(r, new Vector2(Mathf.Max(2, Mathf.Round(lebar / 1.6f)), Mathf.Max(3, Mathf.Round(Mathf.Abs(s.y) / 1.6f))));
            else if (n.Contains("atap"))
                Pakai(r, texGenteng, new Vector2(Mathf.Max(1, s.x * 0.5f), Mathf.Max(1, s.z * 0.5f)));
            else if (n.Contains("jalan") || n.Contains("trotoar") || n.Contains("lantai") || n.Contains("aspal") || n.Contains("dasar"))
                Pakai(r, texAspal, new Vector2(Mathf.Max(1, s.x * 0.35f), Mathf.Max(1, s.z * 0.35f)));
            else if (n.Contains("tanah") || n.Contains("rumput"))
                Pakai(r, texRumput, new Vector2(Mathf.Max(1, s.x * 0.4f), Mathf.Max(1, s.z * 0.4f)));
            else if (n.Contains("pos") || n.Contains("dinding") || n.Contains("pintu"))
                Pakai(r, texBata, new Vector2(Mathf.Max(1, lebar * 0.5f), Mathf.Max(1, Mathf.Abs(s.y) * 0.5f)));
        }
    }

    void Pakai(MeshRenderer r, Texture2D tex, Vector2 tiling)
    {
        Material m = new Material(r.sharedMaterial) { hideFlags = HideFlags.DontSave };
        m.mainTexture = tex;
        m.mainTextureScale = tiling;
        r.sharedMaterial = m;
    }

    void Fasad(MeshRenderer r, Vector2 tiling)
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
        r.sharedMaterial = m;
    }

    // ================= TEKSTUR PROSEDURAL (via kode) =================
    Texture2D Gen(int n, System.Func<int, int, Color> pola)
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

    Color Jendela(int x, int y)
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
}
