using UnityEngine;
using System.Collections.Generic;

// Detail INDUSTRI untuk Level 5 (pabrik): mengubah pabrik kosong jadi pabrik aktif & realistis.
// PEKERJA ber-rompi + helm bekerja di mesin, panel kontrol berlayar, keran las berpercik, gantry crane,
// pipa + katup, tumpukan peti/tong/palet, dan rambu bahaya. Semua di sisi (x>|4.3|), tanpa collider.
// [ExecuteAlways]. Kode + primitif. Objek DontSave.
[ExecuteAlways]
public class RakitPabrikDetail : MonoBehaviour
{
    public float mulaiZ = -20f;
    public float panjang = 380f;
    public float lebar = 6.3f;
    public float tinggi = 7f;

    Shader shaderLit;
    Transform root;
    readonly List<Material> lasMat = new List<Material>();
    readonly List<Material> layarMat = new List<Material>();

    void OnEnable() { Bangun(); }
    void Hancur(Object o) { if (o == null) return; if (Application.isPlaying) Destroy(o); else DestroyImmediate(o); }

    void Bangun()
    {
        Transform lama = transform.Find("PabrikDetail");
        if (lama != null) Hancur(lama.gameObject);
        shaderLit = Shader.Find("Universal Render Pipeline/Lit");
        if (shaderLit == null) shaderLit = Shader.Find("Standard");
        lasMat.Clear(); layarMat.Clear();

        GameObject r = new GameObject("PabrikDetail");
        r.hideFlags = HideFlags.DontSave;
        r.transform.SetParent(transform, false);
        root = r.transform;

        System.Random rnd = new System.Random(505);

        // pekerja + panel kontrol di mesin sisi (tiap 24 m, sisi selang-seling)
        int idx = 0;
        for (float z = mulaiZ + 20f; z <= mulaiZ + panjang - 20f; z += 24f)
        {
            int sisi = (idx % 2 == 0) ? -1 : 1;
            PanelKontrol(new Vector3(sisi * (lebar - 0.9f), 0f, z));
            Pekerja(new Vector3(sisi * (lebar - 1.8f), 0f, z + 1.2f), sisi > 0 ? -90f : 90f, rnd);
            if (idx % 2 == 0) StasiunLas(new Vector3(-sisi * (lebar - 1.4f), 0f, z + 10f));
            idx++;
        }

        // gantry crane melintang tiap 60 m
        for (float z = mulaiZ + 40f; z <= mulaiZ + panjang; z += 60f)
            Gantry(z);

        // pipa + katup membujur di dinding
        Pipa(-1); Pipa(1);

        // tumpukan peti/tong/palet di sisi
        for (int i = 0; i < 22; i++) Tumpukan(rnd);

        // rambu bahaya
        RambuBahaya(new Vector3(-lebar + 0.4f, 2.4f, mulaiZ + 50f), 1, "STOP");
        RambuBahaya(new Vector3(lebar - 0.4f, 2.4f, mulaiZ + 130f), -1, "PANAS");
        RambuBahaya(new Vector3(-lebar + 0.4f, 2.4f, mulaiZ + 230f), 1, "STOP");
    }

    // ---- PEKERJA (RakitOrang rompi oranye + helm) ----
    void Pekerja(Vector3 pos, float yaw, System.Random rnd)
    {
        GameObject g = new GameObject("Pekerja");
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.position = pos;
        g.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        g.SetActive(false);
        RakitOrang ro = g.AddComponent<RakitOrang>();
        ro.warna = new Color(0.95f, 0.5f, 0.1f);                 // rompi oranye
        OrangPos op = g.AddComponent<OrangPos>();
        op.mode = 0; op.hadap = yaw;
        g.SetActive(true);
        // helm keselamatan (bola pipih putih/kuning di kepala)
        Bola("Helm", pos + new Vector3(0f, 1.98f, 0f), new Vector3(0.5f, 0.32f, 0.5f), new Color(0.95f, 0.85f, 0.2f), 0f, g.transform);
    }

    // ---- PANEL KONTROL (kabinet + layar menyala + tombol) ----
    void PanelKontrol(Vector3 dasar)
    {
        float arah = dasar.x > 0 ? -1f : 1f;
        Kotak("Kabinet", dasar + new Vector3(0f, 0.8f, 0f), new Vector3(0.5f, 1.6f, 1.6f), new Color(0.24f, 0.25f, 0.28f), 0f);
        GameObject layar = Kotak("Layar", dasar + new Vector3(arah * 0.28f, 1.3f, 0f), new Vector3(0.05f, 0.5f, 0.8f), new Color(0.3f, 0.7f, 0.5f), 1f);
        layarMat.Add(layar.GetComponent<Renderer>().sharedMaterial);
        for (int i = 0; i < 3; i++)
            Kotak("Tombol", dasar + new Vector3(arah * 0.28f, 0.85f, -0.3f + i * 0.3f), new Vector3(0.04f, 0.12f, 0.12f), i == 0 ? new Color(1f, 0.2f, 0.15f) : new Color(0.2f, 1f, 0.3f), 1.2f);
    }

    // ---- STASIUN LAS (percikan menyala berkedip) ----
    void StasiunLas(Vector3 dasar)
    {
        Kotak("MejaLas", dasar + new Vector3(0f, 0.5f, 0f), new Vector3(1f, 1f, 1.4f), new Color(0.22f, 0.22f, 0.25f), 0f);
        GameObject api = Bola("Percik", dasar + new Vector3(0f, 1.15f, 0f), new Vector3(0.22f, 0.22f, 0.22f), new Color(0.8f, 0.9f, 1f), 3f);
        lasMat.Add(api.GetComponent<Renderer>().sharedMaterial);
    }

    // ---- GANTRY CRANE (rel atas 2 sisi + jembatan + troli + kait) ----
    void Gantry(float z)
    {
        Color besi = new Color(0.3f, 0.31f, 0.34f), kuning = new Color(0.85f, 0.7f, 0.12f);
        float y = tinggi - 0.4f;
        Kotak("RelCraneKi", new Vector3(-lebar + 0.4f, y, z), new Vector3(0.3f, 0.3f, 5f), besi, 0f);
        Kotak("RelCraneKa", new Vector3(lebar - 0.4f, y, z), new Vector3(0.3f, 0.3f, 5f), besi, 0f);
        Kotak("Jembatan", new Vector3(0f, y - 0.1f, z), new Vector3((lebar - 0.4f) * 2f, 0.4f, 0.7f), kuning, 0f);
        float tx = (float)((z * 13) % 5 - 2.5);
        Kotak("Troli", new Vector3(tx, y - 0.45f, z), new Vector3(0.9f, 0.5f, 0.9f), besi, 0f);
        Kotak("Kabel", new Vector3(tx, y - 1.6f, z), new Vector3(0.06f, 2f, 0.06f), new Color(0.1f, 0.1f, 0.12f), 0f);
        Kotak("Kait", new Vector3(tx, y - 2.7f, z), new Vector3(0.3f, 0.4f, 0.3f), besi, 0f);
    }

    // ---- PIPA + KATUP di dinding ----
    void Pipa(int sisi)
    {
        float x = sisi * (lebar - 0.18f);
        Color pipa = new Color(0.45f, 0.35f, 0.2f), pipa2 = new Color(0.35f, 0.4f, 0.45f);
        Kotak("PipaBesar", new Vector3(x, 5.4f, mulaiZ + panjang * 0.5f), new Vector3(0.32f, 0.32f, panjang), pipa, 0f);
        Kotak("PipaKecil", new Vector3(x, 4.8f, mulaiZ + panjang * 0.5f), new Vector3(0.2f, 0.2f, panjang), pipa2, 0f);
        for (float z = mulaiZ + 12f; z <= mulaiZ + panjang; z += 24f)
        {
            Silinder("Katup", new Vector3(x, 5.4f, z), new Vector3(0.4f, 0.08f, 0.4f), new Color(0.6f, 0.2f, 0.15f), Quaternion.Euler(90f, 0f, 0f));
            Kotak("Sanggah", new Vector3(x, 5.1f, z), new Vector3(0.12f, 1.2f, 0.12f), pipa2, 0f);
        }
    }

    // ---- TUMPUKAN peti / tong / palet ----
    void Tumpukan(System.Random rnd)
    {
        int sisi = rnd.Next(2) == 0 ? -1 : 1;
        float z = mulaiZ + 8f + (float)rnd.NextDouble() * (panjang - 16f);
        float x = sisi * (lebar - 0.9f - (float)rnd.NextDouble() * 0.5f);
        int t = rnd.Next(3);
        if (t == 0)
        {
            Kotak("Palet", new Vector3(x, 0.12f, z), new Vector3(1.3f, 0.2f, 1.3f), new Color(0.4f, 0.3f, 0.18f), 0f);
            int n = 1 + rnd.Next(3);
            for (int k = 0; k < n; k++)
                Kotak("Peti", new Vector3(x, 0.6f + k * 0.75f, z), new Vector3(1.1f, 0.7f, 1.1f), new Color(0.5f, 0.38f, 0.2f), 0f);
        }
        else if (t == 1)
        {
            for (int k = 0; k < 2; k++)
                Silinder("Tong", new Vector3(x + (k - 0.5f) * 0.75f, 0.55f, z), new Vector3(0.7f, 0.55f, 0.7f), k == 0 ? new Color(0.8f, 0.4f, 0.1f) : new Color(0.15f, 0.35f, 0.65f), Quaternion.identity);
        }
        else
        {
            Kotak("Drum", new Vector3(x, 0.5f, z), new Vector3(0.9f, 1f, 0.9f), new Color(0.7f, 0.6f, 0.15f), 0f);
            Kotak("DrumTutup", new Vector3(x, 1.02f, z), new Vector3(0.94f, 0.08f, 0.94f), new Color(0.5f, 0.42f, 0.1f), 0f);
        }
    }

    // ---- RAMBU BAHAYA (papan kuning + teks huruf-balok) ----
    void RambuBahaya(Vector3 pos, int hadap, string teks)
    {
        Kotak("TiangRambu", pos + new Vector3(0f, -1.2f, 0f), new Vector3(0.12f, 2.4f, 0.12f), new Color(0.25f, 0.25f, 0.28f), 0f);
        Kotak("PapanRambu", pos, new Vector3(0.1f, 1f, 2f), new Color(0.95f, 0.8f, 0.1f), 0.5f);
        // garis miring hitam (chevron)
        for (int i = -2; i <= 2; i++)
        {
            GameObject g = Kotak("Chevron", pos + new Vector3(hadap * 0.06f, 0f, i * 0.4f), new Vector3(0.04f, 0.9f, 0.14f), new Color(0.08f, 0.08f, 0.08f), 0f);
            g.transform.localRotation = Quaternion.Euler(30f, 0f, 0f);
        }
        HurufBalok.Tulis(root, teks, pos + new Vector3(hadap * 0.09f, 0.62f, 0f), 0.34f, hadap, new Color(0.1f, 0.08f, 0.05f));
    }

    // ============ HELPER ============
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

    GameObject Kotak(string n, Vector3 p, Vector3 s, Color c, float e)
    {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        g.name = n; g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.localPosition = p; g.transform.localScale = s;
        g.GetComponent<Renderer>().sharedMaterial = Mat(c, e);
        Collider col = g.GetComponent<Collider>(); if (col != null) Hancur(col);
        return g;
    }

    GameObject Bola(string n, Vector3 p, Vector3 s, Color c, float e, Transform induk = null)
    {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        g.name = n; g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(induk != null ? induk : root, false);
        if (induk != null) g.transform.position = p; else g.transform.localPosition = p;
        g.transform.localScale = s;
        g.GetComponent<Renderer>().sharedMaterial = Mat(c, e);
        Collider col = g.GetComponent<Collider>(); if (col != null) Hancur(col);
        return g;
    }

    void Silinder(string n, Vector3 p, Vector3 s, Color c, Quaternion rot)
    {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        g.name = n; g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.localPosition = p; g.transform.localScale = s; g.transform.localRotation = rot;
        g.GetComponent<Renderer>().sharedMaterial = Mat(c, 0f);
        Collider col = g.GetComponent<Collider>(); if (col != null) Hancur(col);
    }

    void Update()
    {
        if (transform.Find("PabrikDetail") == null) { Bangun(); return; }
        if (!Application.isPlaying) return;
        float t = Time.time;
        float las = (Mathf.Sin(t * 30f) > 0.3f) ? 4.5f : 0.4f;      // percik las berkedip cepat
        Color biru = new Color(0.8f, 0.9f, 1f);
        for (int i = 0; i < lasMat.Count; i++)
            if (lasMat[i] != null) lasMat[i].SetColor("_EmissionColor", biru * las);
        float lyr = 0.8f + 0.5f * Mathf.Sin(t * 2.4f);
        Color hijau = new Color(0.3f, 0.7f, 0.5f);
        for (int i = 0; i < layarMat.Count; i++)
            if (layarMat[i] != null) layarMat[i].SetColor("_EmissionColor", hijau * lyr);
    }
}
