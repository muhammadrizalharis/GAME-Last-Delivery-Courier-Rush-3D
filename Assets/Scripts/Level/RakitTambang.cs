using UnityEngine;
using System.Collections.Generic;

// Detail TAMBANG untuk Level 3 (terowongan): mengubah terowongan polos jadi tambang sungguhan.
// Rangka penyangga kayu (timber set), lentera dinding menyala, lori/gerobak tambang di rel samping,
// urat bijih menyala, peti/tong/puing/papan, dan PENAMBANG yang bekerja di sisi. Semua di luar jalur main (x>|4.5|).
// [ExecuteAlways] -> tampil di editor. Kode + primitif. Objek DontSave, tanpa collider (gameplay tak berubah).
[ExecuteAlways]
public class RakitTambang : MonoBehaviour
{
    public float mulaiZ = -20f;
    public float panjang = 380f;
    public float lebar = 6.3f;   // setengah lebar (dinding di x = ±lebar)
    public float tinggi = 8f;

    Shader shaderLit;
    Transform root;
    readonly List<Material> apiMat = new List<Material>();

    void OnEnable() { Bangun(); }

    void Hancur(Object o) { if (o == null) return; if (Application.isPlaying) Destroy(o); else DestroyImmediate(o); }

    void Bangun()
    {
        Transform lama = transform.Find("Tambang");
        if (lama != null) Hancur(lama.gameObject);
        shaderLit = Shader.Find("Universal Render Pipeline/Lit");
        if (shaderLit == null) shaderLit = Shader.Find("Standard");
        apiMat.Clear();

        GameObject r = new GameObject("Tambang");
        r.hideFlags = HideFlags.DontSave;
        r.transform.SetParent(transform, false);
        root = r.transform;

        System.Random rnd = new System.Random(303);

        for (float z = mulaiZ + 4f; z <= mulaiZ + panjang; z += 11f)
            RangkaKayu(z);

        for (float z = mulaiZ + 10f; z <= mulaiZ + panjang; z += 15f)
        {
            int sisi = (Mathf.RoundToInt(z) / 15) % 2 == 0 ? -1 : 1;
            Lentera(new Vector3(sisi * (lebar - 0.25f), 3.2f, z));
        }

        for (int i = 0; i < 34; i++)
            UratBijih(rnd);

        // lori/gerobak tambang di rel samping (dekorasi, statis)
        Lori(new Vector3(-lebar + 1.2f, 0f, mulaiZ + 45f));
        Lori(new Vector3(lebar - 1.2f, 0f, mulaiZ + 130f));
        Lori(new Vector3(-lebar + 1.2f, 0f, mulaiZ + 250f));

        for (int i = 0; i < 26; i++)
            PropSamping(rnd);

        // penambang bekerja di sisi (RakitPenambang), menghadap dinding
        for (float z = mulaiZ + 30f; z <= mulaiZ + panjang - 30f; z += 62f)
        {
            Penambang(new Vector3(-lebar + 1.1f, 0f, z), 90f);         // hadap dinding kiri (+x? -> sisi kiri di -x, hadap -x)
            Penambang(new Vector3(lebar - 1.1f, 0f, z + 30f), -90f);
        }
    }

    // ---- RANGKA PENYANGGA KAYU (timber set): 2 tiang + palang + siku ----
    void RangkaKayu(float z)
    {
        Color kayu = new Color(0.34f, 0.24f, 0.15f), kayu2 = new Color(0.28f, 0.2f, 0.13f);
        float x = lebar - 0.35f;
        Kotak("TiangKi", new Vector3(-x, tinggi * 0.5f, z), new Vector3(0.4f, tinggi, 0.4f), kayu, 0f);
        Kotak("TiangKa", new Vector3(x, tinggi * 0.5f, z), new Vector3(0.4f, tinggi, 0.4f), kayu, 0f);
        Kotak("Palang", new Vector3(0f, tinggi - 0.25f, z), new Vector3(2f * x + 0.4f, 0.42f, 0.42f), kayu2, 0f);
        GameObject sk = Kotak("SikuKi", new Vector3(-x + 0.7f, tinggi - 0.9f, z), new Vector3(1.6f, 0.28f, 0.28f), kayu2, 0f);
        sk.transform.localRotation = Quaternion.Euler(0f, 0f, 38f);
        GameObject sk2 = Kotak("SikuKa", new Vector3(x - 0.7f, tinggi - 0.9f, z), new Vector3(1.6f, 0.28f, 0.28f), kayu2, 0f);
        sk2.transform.localRotation = Quaternion.Euler(0f, 0f, -38f);
    }

    // ---- LENTERA dinding (bracket + api menyala) ----
    void Lentera(Vector3 pos)
    {
        float arah = pos.x > 0 ? -1f : 1f;
        Kotak("Bracket", pos + new Vector3(arah * 0.25f, 0f, 0f), new Vector3(0.5f, 0.1f, 0.1f), new Color(0.2f, 0.17f, 0.13f), 0f);
        GameObject api = Bola("Api", pos + new Vector3(arah * 0.5f, 0.05f, 0f), new Vector3(0.34f, 0.42f, 0.34f), new Color(1f, 0.6f, 0.2f), 2.4f);
        apiMat.Add(api.GetComponent<Renderer>().sharedMaterial);
    }

    // ---- URAT BIJIH menyala di dinding ----
    void UratBijih(System.Random rnd)
    {
        int sisi = rnd.Next(2) == 0 ? -1 : 1;
        float z = mulaiZ + (float)rnd.NextDouble() * panjang;
        float y = 1.5f + (float)rnd.NextDouble() * (tinggi - 3f);
        Color[] bijih = { new Color(1f, 0.8f, 0.25f), new Color(0.4f, 0.85f, 1f), new Color(0.6f, 1f, 0.55f), new Color(1f, 0.5f, 0.35f) };
        Color c = bijih[rnd.Next(bijih.Length)];
        GameObject g = Kotak("Bijih", new Vector3(sisi * (lebar - 0.12f), y, z), new Vector3(0.1f, 0.3f + (float)rnd.NextDouble() * 0.6f, 0.3f + (float)rnd.NextDouble() * 0.8f), c, 1.3f);
        g.transform.localRotation = Quaternion.Euler(0f, 0f, (float)(rnd.NextDouble() * 60 - 30));
    }

    // ---- LORI/GEROBAK TAMBANG (badan + roda + bijih) di rel samping ----
    void Lori(Vector3 pos)
    {
        Color besi = new Color(0.28f, 0.29f, 0.32f), kayu = new Color(0.3f, 0.22f, 0.14f), gelap = new Color(0.12f, 0.12f, 0.13f);
        // rel samping pendek
        Kotak("RelSamping", pos + new Vector3(0f, 0.08f, 0f), new Vector3(1.4f, 0.08f, 6f), gelap, 0f);
        Kotak("BadanLori", pos + new Vector3(0f, 0.75f, 0f), new Vector3(1.5f, 0.9f, 2.2f), besi, 0f);
        Kotak("BijihLori", pos + new Vector3(0f, 1.3f, 0f), new Vector3(1.3f, 0.4f, 2f), new Color(0.35f, 0.3f, 0.28f), 0f);
        Silinder("Roda1", pos + new Vector3(-0.7f, 0.35f, 0.7f), new Vector3(0.5f, 0.1f, 0.5f), gelap, Quaternion.Euler(0f, 0f, 90f));
        Silinder("Roda2", pos + new Vector3(0.7f, 0.35f, 0.7f), new Vector3(0.5f, 0.1f, 0.5f), gelap, Quaternion.Euler(0f, 0f, 90f));
        Silinder("Roda3", pos + new Vector3(-0.7f, 0.35f, -0.7f), new Vector3(0.5f, 0.1f, 0.5f), gelap, Quaternion.Euler(0f, 0f, 90f));
        Silinder("Roda4", pos + new Vector3(0.7f, 0.35f, -0.7f), new Vector3(0.5f, 0.1f, 0.5f), gelap, Quaternion.Euler(0f, 0f, 90f));
    }

    // ---- PROP SAMPING: peti, tong, puing, papan ----
    void PropSamping(System.Random rnd)
    {
        int sisi = rnd.Next(2) == 0 ? -1 : 1;
        float z = mulaiZ + 6f + (float)rnd.NextDouble() * (panjang - 12f);
        float x = sisi * (lebar - 0.7f - (float)rnd.NextDouble() * 0.6f);
        int t = rnd.Next(4);
        if (t == 0)
        {
            Kotak("Peti", new Vector3(x, 0.4f, z), new Vector3(0.9f, 0.8f, 0.9f), new Color(0.4f, 0.3f, 0.18f), 0f);
            if (rnd.Next(2) == 0) Kotak("Peti2", new Vector3(x, 1.15f, z), new Vector3(0.8f, 0.7f, 0.8f), new Color(0.36f, 0.27f, 0.16f), 0f);
        }
        else if (t == 1)
            Silinder("Tong", new Vector3(x, 0.6f, z), new Vector3(0.7f, 0.6f, 0.7f), new Color(0.5f, 0.32f, 0.14f), Quaternion.identity);
        else if (t == 2)
        {
            for (int k = 0; k < 3; k++)
                Kotak("Puing", new Vector3(x + (float)(rnd.NextDouble() * 0.6 - 0.3), 0.2f + k * 0.18f, z + (float)(rnd.NextDouble() * 0.6 - 0.3)), new Vector3(0.5f, 0.4f, 0.5f), new Color(0.22f, 0.21f, 0.19f), 0f);
        }
        else
        {
            GameObject p = Kotak("Papan", new Vector3(x, 1f, z), new Vector3(0.14f, 2f, 0.4f), new Color(0.35f, 0.25f, 0.15f), 0f);
            p.transform.localRotation = Quaternion.Euler(0f, 0f, sisi * 18f);
        }
    }

    // ---- PENAMBANG (RakitPenambang) bekerja di sisi ----
    void Penambang(Vector3 pos, float yaw)
    {
        GameObject g = new GameObject("PenambangKerja");
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.position = pos;
        g.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        g.AddComponent<RakitPenambang>();
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
        if (transform.Find("Tambang") == null) { Bangun(); return; }
        if (!Application.isPlaying) return;
        float k = 2f + 0.8f * Mathf.Sin(Time.time * 8f + 1.3f) * Mathf.Sin(Time.time * 3.1f);   // api berkedip
        Color api = new Color(1f, 0.6f, 0.2f);
        for (int i = 0; i < apiMat.Count; i++)
            if (apiMat[i] != null) apiMat[i].SetColor("_EmissionColor", api * k);
    }
}
