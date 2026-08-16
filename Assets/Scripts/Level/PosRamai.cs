using UnityEngine;

// Meramaikan area POS PAKET: tumpukan paket, rak, troli, dan ORANG (pegawai, pelanggan yang berinteraksi,
// kelompok mengobrol, orang lalu-lalang, dan yang masuk bangunan). Pakai RakitOrang (visual) + OrangPos (perilaku).
// [ExecuteAlways] -> orang & paket tampil di editor; gerak orang & animasi saat Play. Objek DontSave.
[ExecuteAlways]
public class PosRamai : MonoBehaviour
{
    Shader shaderLit;
    Transform root;

    void OnEnable() { Bangun(); }

    void Hancur(Object o)
    {
        if (o == null) return;
        if (Application.isPlaying) Destroy(o); else DestroyImmediate(o);
    }

    void Bangun()
    {
        Transform lama = transform.Find("PosRamai");
        if (lama != null) Hancur(lama.gameObject);

        shaderLit = Shader.Find("Universal Render Pipeline/Lit");
        if (shaderLit == null) shaderLit = Shader.Find("Standard");

        GameObject r = new GameObject("PosRamai");
        r.hideFlags = HideFlags.DontSave;
        r.transform.SetParent(transform, false);
        root = r.transform;

        Bounds plaza = Cari("LantaiPos", new Bounds(new Vector3(17, 0.1f, 5), new Vector3(13, 0.3f, 15)));
        Bounds meja = Cari("Meja", new Bounds(new Vector3(19, 0.85f, 9), new Vector3(1.6f, 1.5f, 5)));
        Vector3 pc = plaza.center, pe = plaza.extents;
        float y = plaza.max.y;
        Vector3 mc = meja.center; mc.y = y;

        System.Random rnd = new System.Random(99);
        Color[] baju = {
            new Color(0.85f,0.35f,0.3f), new Color(0.3f,0.55f,0.85f), new Color(0.35f,0.65f,0.4f),
            new Color(0.9f,0.8f,0.4f), new Color(0.6f,0.45f,0.8f), new Color(0.9f,0.55f,0.3f)
        };
        Color seragam = new Color(0.15f, 0.35f, 0.62f); // pegawai pos

        // ---- PAKET & PERABOT (ditata rapi di tepi ruangan biar tengah lega) ----
        // ===== GUDANG POS PENUH PAKET (banyak rak + tumpukan di sepanjang dinding) =====
        for (int i = 0; i < 4; i++)
            Rak(new Vector3(pc.x - pe.x * 0.5f + i * pe.x * 0.4f, y, pc.z + pe.z * 0.85f), rnd);        // dinding belakang
        for (int i = 0; i < 3; i++)
            Rak(new Vector3(pc.x + pe.x * 0.78f, y, pc.z + pe.z * (0.45f - i * 0.4f)), rnd);            // dinding kanan
        for (int i = 0; i < 4; i++)
            TumpukPaket(new Vector3(pc.x - pe.x * 0.8f, y, pc.z + pe.z * (0.85f - i * 0.2f)), 3 + (i % 3), rnd);  // dinding kiri (belakang pintu)
        // troli
        Troli(new Vector3(mc.x + 1.6f, y, mc.z - 2.6f), rnd);
        Troli(new Vector3(pc.x - pe.x * 0.3f, y, pc.z - pe.z * 0.35f), rnd);

        // ---- ORANG (secukupnya biar pintu pos tidak sumpek; keramaian utama ada di luar/HiasKota) ----
        // 2 pegawai di belakang meja, menghadap pelanggan
        Orang(new Vector3(mc.x + 1.0f, y, mc.z - 1.2f), seragam, 0, -90f, Vector3.forward, 0f);
        Orang(new Vector3(mc.x + 1.0f, y, mc.z + 1.3f), seragam, 0, -90f, Vector3.forward, 0f);
        // 2 pelanggan di depan meja (berinteraksi), menghadap pegawai
        Orang(new Vector3(mc.x - 1.5f, y, mc.z - 0.8f), baju[0], 0, 90f, Vector3.forward, 0f);
        Orang(new Vector3(mc.x - 1.6f, y, mc.z + 1.0f), baju[1], 0, 90f, Vector3.forward, 0f);
        // 1 petugas berjalan mengangkut paket, tetap di dalam lantai pos (tidak melayang)
        Orang(new Vector3(pc.x + pe.x * 0.15f, y, pc.z - pe.z * 0.2f), seragam, 1, 0f, Vector3.forward, Mathf.Min(2.2f, pe.z * 0.35f));

        // ---- komputer di meja layanan (2 unit), layar & keyboard menghadap PETUGAS (+x) ----
        Komputer(new Vector3(mc.x, meja.max.y, mc.z - 1.3f), 1f);
        Komputer(new Vector3(mc.x, meja.max.y, mc.z + 1.6f), 1f);

        // ---- KURIR LAIN yang juga ambil/antar paket (bawa kardus, jalan masuk-keluar) ----
        KurirLain(new Vector3(pc.x - pe.x * 0.45f, y, pc.z - pe.z * 0.15f), new Color(0.9f, 0.55f, 0.2f), Vector3.right, 3f);
        KurirLain(new Vector3(pc.x - pe.x * 0.6f, y, pc.z + pe.z * 0.25f), new Color(0.35f, 0.6f, 0.85f), Vector3.forward, 3.2f);
    }

    Bounds Cari(string nama, Bounds fallback)
    {
        GameObject g = GameObject.Find(nama);
        if (g != null)
        {
            Renderer r = g.GetComponent<Renderer>();
            if (r != null) return r.bounds;
        }
        return fallback;
    }

    // ---- ORANG (RakitOrang visual + OrangPos perilaku) ----
    void Orang(Vector3 pos, Color warna, int mode, float hadap, Vector3 arah, float jarak)
    {
        GameObject g = new GameObject("Orang");
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.position = pos;
        g.SetActive(false);                       // warna harus di-set SEBELUM RakitOrang.OnEnable membangun
        RakitOrang ro = g.AddComponent<RakitOrang>();
        ro.warna = warna;
        OrangPos op = g.AddComponent<OrangPos>();
        op.mode = mode; op.hadap = hadap; op.arah = arah; op.jarak = jarak;
        g.SetActive(true);
    }

    // ---- kurir lain: orang membawa kardus paket di punggung, jalan masuk-keluar pos ----
    void KurirLain(Vector3 pos, Color baju, Vector3 arah, float jarak)
    {
        GameObject g = new GameObject("KurirLain");
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.position = pos;
        g.SetActive(false);
        RakitOrang ro = g.AddComponent<RakitOrang>();
        ro.warna = baju;
        OrangPos op = g.AddComponent<OrangPos>();
        op.mode = 1; op.arah = arah; op.jarak = jarak; op.hadap = 0f;
        g.SetActive(true);
        // kardus di punggung (child root kurir -> ikut gerak & putar bersama kurir)
        GameObject tas = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tas.name = "TasKurir"; tas.hideFlags = HideFlags.DontSave;
        tas.transform.SetParent(g.transform, false);
        tas.transform.localPosition = new Vector3(0f, 1.3f, -0.34f);
        tas.transform.localScale = new Vector3(0.52f, 0.56f, 0.4f);
        tas.GetComponent<Renderer>().sharedMaterial = Mat(new Color(0.74f, 0.55f, 0.33f));
        Collider c = tas.GetComponent<Collider>();
        if (c != null) Hancur(c);
    }

    // ---- PAKET & PERABOT ----
    void TumpukPaket(Vector3 pos, int n, System.Random rnd)
    {
        float ty = pos.y;
        for (int i = 0; i < n; i++)
        {
            float sz = 0.55f + (float)rnd.NextDouble() * 0.3f;
            Color k = Color.Lerp(new Color(0.72f, 0.53f, 0.32f), new Color(0.82f, 0.64f, 0.42f), (float)rnd.NextDouble());
            float gx = ((float)rnd.NextDouble() - 0.5f) * 0.2f;
            Vector3 p = new Vector3(pos.x + gx, ty + sz * 0.4f + i * sz * 0.85f, pos.z + gx);
            Kotak("Paket", p, new Vector3(sz, sz * 0.8f, sz), k);
            Kotak("Lakban", p + Vector3.up * (sz * 0.41f), new Vector3(sz * 1.02f, 0.05f, 0.16f), new Color(0.86f, 0.8f, 0.62f));
        }
    }

    void Rak(Vector3 pos, System.Random rnd)
    {
        Color gelap = new Color(0.35f, 0.28f, 0.22f);
        Color kayu = new Color(0.55f, 0.4f, 0.26f);
        float w = 2.4f, h = 2.6f, d = 0.9f;
        Kotak("RakKi", pos + new Vector3(-w * 0.5f, h * 0.5f, 0), new Vector3(0.12f, h, d), gelap);
        Kotak("RakKa", pos + new Vector3(w * 0.5f, h * 0.5f, 0), new Vector3(0.12f, h, d), gelap);
        for (int lv = 0; lv < 3; lv++)
        {
            float sy = 0.45f + lv * 0.95f;
            Kotak("Papan", pos + new Vector3(0, sy, 0), new Vector3(w, 0.08f, d), kayu);
            for (int i = -1; i <= 1; i++)
            {
                Color k = Color.Lerp(new Color(0.72f, 0.53f, 0.32f), new Color(0.82f, 0.64f, 0.42f), (float)rnd.NextDouble());
                Kotak("Paket", pos + new Vector3(i * 0.72f, sy + 0.33f, 0), new Vector3(0.56f, 0.5f, 0.62f), k);
            }
        }
    }

    void Troli(Vector3 pos, System.Random rnd)
    {
        Color logam = new Color(0.4f, 0.42f, 0.46f);
        Kotak("TroliAlas", pos + new Vector3(0, 0.35f, 0), new Vector3(1.3f, 0.12f, 0.9f), logam);
        Kotak("TroliGagang", pos + new Vector3(-0.7f, 0.9f, 0), new Vector3(0.08f, 1.1f, 0.08f), logam);
        Silinder("Roda1", pos + new Vector3(-0.5f, 0.18f, 0.35f), new Vector3(0.18f, 0.06f, 0.18f), new Color(0.1f, 0.1f, 0.12f), Quaternion.Euler(90, 0, 0));
        Silinder("Roda2", pos + new Vector3(0.5f, 0.18f, 0.35f), new Vector3(0.18f, 0.06f, 0.18f), new Color(0.1f, 0.1f, 0.12f), Quaternion.Euler(90, 0, 0));
        for (int i = 0; i < 3; i++)
        {
            Color k = Color.Lerp(new Color(0.72f, 0.53f, 0.32f), new Color(0.82f, 0.64f, 0.42f), (float)rnd.NextDouble());
            Kotak("Paket", pos + new Vector3(0, 0.7f + i * 0.5f, 0), new Vector3(0.7f, 0.5f, 0.7f), k);
        }
    }

    // ---- komputer meja: layar & keyboard menghadap PETUGAS (hadapX) biar tak kebalik ----
    void Komputer(Vector3 dasar, float hadapX)
    {
        Color body = new Color(0.12f, 0.12f, 0.14f), scr = new Color(0.42f, 0.68f, 0.85f), kb = new Color(0.16f, 0.16f, 0.2f);
        Kotak("Monitor", dasar + new Vector3(0f, 0.34f, 0f), new Vector3(0.07f, 0.44f, 0.64f), body);
        Kotak("Layar", dasar + new Vector3(hadapX * 0.038f, 0.34f, 0f), new Vector3(0.02f, 0.34f, 0.54f), scr);
        Kotak("StandLeher", dasar + new Vector3(0f, 0.1f, 0f), new Vector3(0.08f, 0.18f, 0.08f), body);
        Kotak("StandKaki", dasar + new Vector3(0f, 0.02f, 0f), new Vector3(0.26f, 0.04f, 0.3f), body);
        Kotak("Keyboard", dasar + new Vector3(hadapX * 0.3f, 0.02f, 0f), new Vector3(0.24f, 0.05f, 0.5f), kb);
        Kotak("Mouse", dasar + new Vector3(hadapX * 0.3f, 0.02f, 0.4f), new Vector3(0.1f, 0.05f, 0.14f), kb);
    }

    // ---- HELPER PRIMITIF ----
    Material Mat(Color c)
    {
        Material m = new Material(shaderLit) { hideFlags = HideFlags.DontSave };
        m.color = c;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
        return m;
    }

    void Kotak(string n, Vector3 pos, Vector3 skala, Color warna)
    {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        g.name = n; g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.position = pos; g.transform.localScale = skala;
        g.GetComponent<Renderer>().sharedMaterial = Mat(warna);
        Collider c = g.GetComponent<Collider>();
        if (c != null) Hancur(c);
    }

    void Silinder(string n, Vector3 pos, Vector3 skala, Color warna, Quaternion rot)
    {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        g.name = n; g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.position = pos; g.transform.localScale = skala; g.transform.localRotation = rot;
        g.GetComponent<Renderer>().sharedMaterial = Mat(warna);
        Collider c = g.GetComponent<Collider>();
        if (c != null) Hancur(c);
    }
}
