using UnityEngine;

// Membangun GEMBOK TERBUKA dari primitif. [ExecuteAlways] -> tampil di editor.
[ExecuteAlways]
public class GembokTerbuka : MonoBehaviour
{
    Transform root;

    void OnEnable() { Bangun(); }

    void Hancur(Object o)
    {
        if (o == null) return;
        if (Application.isPlaying) Destroy(o); else DestroyImmediate(o);
    }

    void Bangun()
    {
        Transform lama = transform.Find("Visual");
        if (lama != null) Hancur(lama.gameObject);

        Shader s = Shader.Find("Universal Render Pipeline/Lit");
        if (s == null) s = Shader.Find("Standard");
        Material emas = Buat(s, new Color(0.96f, 0.79f, 0.22f), 0.35f, 0.55f);
        Material gelap = Buat(s, new Color(0.1f, 0.1f, 0.12f), 0f, 0.2f);
        Material logam = Buat(s, new Color(0.82f, 0.84f, 0.9f), 0.8f, 0.7f);

        GameObject vis = new GameObject("Visual");
        vis.hideFlags = HideFlags.DontSave;
        vis.transform.SetParent(transform, false);
        root = vis.transform;

        Badan(root, emas);
        LubangKunci(root, gelap);
        Gagang(root, logam);
    }

    // Badan gembok berbentuk persegi membulat (dua kotak silang + 4 sudut silinder)
    void Badan(Transform p, Material m)
    {
        float w = 1.5f, h = 1.7f, d = 0.55f, rc = 0.28f;
        Bag(p, PrimitiveType.Cube, new Vector3(0, 0, 0), new Vector3(w, h - 2 * rc, d), Quaternion.identity, m);
        Bag(p, PrimitiveType.Cube, new Vector3(0, 0, 0), new Vector3(w - 2 * rc, h, d), Quaternion.identity, m);
        float cx = w / 2 - rc, cy = h / 2 - rc;
        Quaternion rz = Quaternion.Euler(90, 0, 0);
        Vector3 sc = new Vector3(rc * 2, d / 2, rc * 2);
        Bag(p, PrimitiveType.Cylinder, new Vector3(cx, cy, 0), sc, rz, m);
        Bag(p, PrimitiveType.Cylinder, new Vector3(-cx, cy, 0), sc, rz, m);
        Bag(p, PrimitiveType.Cylinder, new Vector3(cx, -cy, 0), sc, rz, m);
        Bag(p, PrimitiveType.Cylinder, new Vector3(-cx, -cy, 0), sc, rz, m);
    }

    void LubangKunci(Transform p, Material m)
    {
        float zf = -0.30f;
        Bag(p, PrimitiveType.Cylinder, new Vector3(0, 0.08f, zf), new Vector3(0.28f, 0.03f, 0.28f), Quaternion.Euler(90, 0, 0), m);
        Bag(p, PrimitiveType.Cube, new Vector3(0, -0.22f, zf), new Vector3(0.1f, 0.42f, 0.06f), Quaternion.identity, m);
    }

    // Gagang (shackle) melengkung halus dari bola-bola kecil, posisi TERBUKA
    void Gagang(Transform p, Material m)
    {
        GameObject g = new GameObject("Gagang");
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(p, false);
        g.transform.localPosition = new Vector3(-0.28f, 0.30f, 0f);
        g.transform.localRotation = Quaternion.Euler(0, 0, 16f);
        Transform gt = g.transform;

        float rt = 0.13f, hLeg = 0.9f, W = 1.0f, r = W * 0.5f;
        // kaki kiri (masuk ke badan)
        for (float y = 0f; y <= hLeg + 0.001f; y += 0.09f) Bola(gt, new Vector3(0, y, 0), rt, m);
        // lengkung atas
        int seg = 16;
        for (int i = 0; i <= seg; i++)
        {
            float a = Mathf.PI - Mathf.PI * i / seg;
            Bola(gt, new Vector3(r + r * Mathf.Cos(a), hLeg + r * Mathf.Sin(a), 0), rt, m);
        }
        // kaki kanan (pendek, terangkat keluar)
        for (float y = hLeg; y >= 0.45f - 0.001f; y -= 0.09f) Bola(gt, new Vector3(W, y, 0), rt, m);
    }

    Material Buat(Shader s, Color c, float metal, float halus)
    {
        Material m = new Material(s);
        m.hideFlags = HideFlags.DontSave;
        m.color = c;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
        if (m.HasProperty("_Metallic")) m.SetFloat("_Metallic", metal);
        if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", halus);
        return m;
    }

    void Bola(Transform p, Vector3 lp, float rad, Material m)
    {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(p, false);
        g.transform.localPosition = lp;
        g.transform.localScale = new Vector3(rad * 2, rad * 2, rad * 2);
        g.GetComponent<Renderer>().sharedMaterial = m;
        Collider c = g.GetComponent<Collider>();
        if (c != null) Hancur(c);
    }

    void Bag(Transform p, PrimitiveType t, Vector3 lp, Vector3 ls, Quaternion rot, Material m)
    {
        GameObject g = GameObject.CreatePrimitive(t);
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(p, false);
        g.transform.localPosition = lp;
        g.transform.localScale = ls;
        g.transform.localRotation = rot;
        g.GetComponent<Renderer>().sharedMaterial = m;
        Collider c = g.GetComponent<Collider>();
        if (c != null) Hancur(c);
    }

    Vector3 posAwal;
    bool punyaPos;

    void Update()
    {
        // pastikan gembok selalu ada (rebuild kalau Visual terhapus / belum sempat dibangun)
        if (transform.Find("Visual") == null) Bangun();
        if (!Application.isPlaying) return;
        if (!punyaPos) { posAwal = transform.localPosition; punyaPos = true; }
        float t = Time.time;
        transform.localRotation = Quaternion.Euler(0f, Mathf.Sin(t * 1.1f) * 16f, 0f);
        transform.localPosition = posAwal + new Vector3(0f, Mathf.Sin(t * 2.2f) * 0.05f, 0f);
    }
}
