using UnityEngine;
using System.Collections.Generic;

// Detail PASAR MALAM untuk Level 4: menambah DAGANGAN (buah/sayur/panggangan) di tiap kios,
// untaian lampu (string lights) melintang, dan spanduk warna-warni -> pasar terasa hidup & nyata.
// Menempel di objek 'Rel' bersama RakitPasar/KerumunanPasar. Di sisi jalan (x>|4|), tanpa collider.
// [ExecuteAlways]. Kode + primitif. Objek DontSave.
[ExecuteAlways]
public class RakitPasarDetail : MonoBehaviour
{
    public float mulaiZ = -20f;
    public float panjang = 380f;
    public float lebar = 6.3f;
    public float jarakStan = 18f;

    Shader shaderLit;
    Transform root;
    readonly List<Material> baraMat = new List<Material>();
    readonly List<Material> lampuMat = new List<Material>();

    void OnEnable() { Bangun(); }
    void Hancur(Object o) { if (o == null) return; if (Application.isPlaying) Destroy(o); else DestroyImmediate(o); }

    void Bangun()
    {
        Transform lama = transform.Find("PasarDetail");
        if (lama != null) Hancur(lama.gameObject);
        shaderLit = Shader.Find("Universal Render Pipeline/Lit");
        if (shaderLit == null) shaderLit = Shader.Find("Standard");
        baraMat.Clear(); lampuMat.Clear();

        GameObject r = new GameObject("PasarDetail");
        r.hideFlags = HideFlags.DontSave;
        r.transform.SetParent(transform, false);
        root = r.transform;

        System.Random rnd = new System.Random(404);
        int idx = 0;
        for (float z = mulaiZ + jarakStan * 0.5f; z <= mulaiZ + panjang; z += jarakStan)
        {
            Dagangan(-1, z, rnd, idx);
            Dagangan(1, z, rnd, idx + 1);
            idx += 2;
        }

        UntaianLampu();
        Spanduk();
    }

    // ---- DAGANGAN di kios (meja display + buah/sayur/panggangan + papan harga) ----
    void Dagangan(int sisi, float z, System.Random rnd, int idx)
    {
        float x = sisi * (lebar - 0.7f);
        Color kayu = new Color(0.45f, 0.32f, 0.2f);
        Kotak("MejaDagang", new Vector3(x, 0.45f, z), new Vector3(1.3f, 0.85f, 2.6f), kayu, 0f);
        Kotak("MejaAtas", new Vector3(x, 0.9f, z), new Vector3(1.5f, 0.1f, 2.8f), new Color(0.5f, 0.36f, 0.22f), 0f);

        int jenis = idx % 3;
        if (jenis == 2)
        {
            // panggangan: bara menyala + tusuk sate + asap-tiang
            Kotak("Panggangan", new Vector3(x, 1.0f, z), new Vector3(1f, 0.18f, 2f), new Color(0.15f, 0.14f, 0.14f), 0f);
            GameObject bara = Kotak("Bara", new Vector3(x, 1.08f, z), new Vector3(0.85f, 0.06f, 1.8f), new Color(1f, 0.4f, 0.1f), 2.2f);
            baraMat.Add(bara.GetComponent<Renderer>().sharedMaterial);
            for (int i = 0; i < 5; i++)
                Kotak("Sate", new Vector3(x - 0.2f, 1.16f, z - 0.8f + i * 0.4f), new Vector3(0.5f, 0.05f, 0.05f), new Color(0.5f, 0.28f, 0.15f), 0f);
        }
        else
        {
            // buah/sayur: 3 keranjang berisi bola warna-warni
            Color[] buah = jenis == 0
                ? new[] { new Color(0.85f, 0.2f, 0.18f), new Color(1f, 0.6f, 0.1f), new Color(0.95f, 0.8f, 0.2f) }   // apel/jeruk/mangga
                : new[] { new Color(0.3f, 0.6f, 0.2f), new Color(0.7f, 0.2f, 0.3f), new Color(0.85f, 0.75f, 0.5f) };  // sayur
            for (int b = 0; b < 3; b++)
            {
                float bz = z - 0.85f + b * 0.85f;
                Silinder("Keranjang", new Vector3(x, 1.02f, bz), new Vector3(0.62f, 0.12f, 0.62f), new Color(0.45f, 0.3f, 0.16f), Quaternion.identity);
                Color c = buah[b % buah.Length];
                for (int k = 0; k < 3; k++)
                    Bola("Buah", new Vector3(x + (float)(rnd.NextDouble() * 0.4 - 0.2), 1.16f + (k / 3) * 0.16f, bz + (float)(rnd.NextDouble() * 0.4 - 0.2)), Vector3.one * 0.2f, c, 0f);
            }
        }

        // papan harga/nama kecil (warna, tanpa teks) di atas meja
        Color[] papan = { new Color(0.8f, 0.2f, 0.2f), new Color(0.2f, 0.5f, 0.8f), new Color(0.2f, 0.6f, 0.3f), new Color(0.85f, 0.6f, 0.15f) };
        Kotak("PapanHarga", new Vector3(x - sisi * 0.1f, 1.7f, z), new Vector3(0.08f, 0.5f, 1.4f), papan[idx % papan.Length], 0.35f);
    }

    // ---- UNTAIAN LAMPU (string lights) zig-zag melintang jalan ----
    void UntaianLampu()
    {
        Color kabel = new Color(0.1f, 0.1f, 0.1f);
        float y = 5.2f;
        for (float z = mulaiZ + 5f; z <= mulaiZ + panjang; z += 6f)
        {
            bool naik = (Mathf.RoundToInt(z) / 4) % 2 == 0;
            float yy = y + (naik ? 0.5f : 0f);
            // kabel melintang
            GameObject k = Kotak("Kabel", new Vector3(0f, yy, z), new Vector3((lebar - 0.6f) * 2f, 0.05f, 0.05f), kabel, 0f);
            // bohlam kecil
            for (int i = -2; i <= 2; i++)
            {
                Color w = (i % 2 == 0) ? new Color(1f, 0.85f, 0.4f) : new Color(1f, 0.5f, 0.4f);
                GameObject b = Bola("Bohlam", new Vector3(i * (lebar - 0.6f) / 2.5f, yy - 0.15f, z), Vector3.one * 0.18f, w, 2f);
                lampuMat.Add(b.GetComponent<Renderer>().sharedMaterial);
            }
        }
    }

    // ---- SPANDUK warna-warni di gerbang/melintang ----
    void Spanduk()
    {
        Color[] warna = { new Color(0.85f, 0.2f, 0.2f), new Color(0.9f, 0.7f, 0.15f), new Color(0.2f, 0.55f, 0.75f), new Color(0.3f, 0.65f, 0.35f) };
        int i = 0;
        for (float z = mulaiZ + 24f; z <= mulaiZ + panjang; z += 46f)
        {
            Kotak("Spanduk", new Vector3(0f, 6f, z), new Vector3((lebar - 0.4f) * 2f, 0.9f, 0.1f), warna[i % warna.Length], 0.25f);
            i++;
        }
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

    GameObject Bola(string n, Vector3 p, Vector3 s, Color c, float e)
    {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        g.name = n; g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.localPosition = p; g.transform.localScale = s;
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
        if (transform.Find("PasarDetail") == null) { Bangun(); return; }
        if (!Application.isPlaying) return;
        float t = Time.time;
        float bara = 2f + 0.7f * Mathf.Sin(t * 6f) * Mathf.Sin(t * 2.3f);
        Color api = new Color(1f, 0.4f, 0.1f);
        for (int i = 0; i < baraMat.Count; i++)
            if (baraMat[i] != null) baraMat[i].SetColor("_EmissionColor", api * bara);
        for (int i = 0; i < lampuMat.Count; i++)
            if (lampuMat[i] != null)
            {
                float f = 1.6f + 0.5f * Mathf.Sin(t * 3f + i * 0.7f);
                Color w = lampuMat[i].color;
                lampuMat[i].SetColor("_EmissionColor", w * f);
            }
    }
}
