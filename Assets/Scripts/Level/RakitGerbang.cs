using UnityEngine;

// Merakit GERBANG/PALANG rel yang TINGGI -> pemain harus MENUNDUK untuk lewat. [ExecuteAlways].
[ExecuteAlways]
public class RakitGerbang : MonoBehaviour
{
    public Color warna = new Color(0.9f, 0.75f, 0.1f);
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

        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr != null) mr.enabled = false;

        Shader s = Shader.Find("Universal Render Pipeline/Lit");
        if (s == null) s = Shader.Find("Standard");

        GameObject vis = new GameObject("Visual");
        vis.hideFlags = HideFlags.DontSave;
        vis.transform.SetParent(transform, false);
        root = vis.transform;

        Material kuning = Buat(s, warna);
        Material hitam = Buat(s, new Color(0.1f, 0.1f, 0.12f));

        // dua tiang tinggi di pinggir rel
        Bag("TiangKiri", new Vector3(-4.3f, 1.2f, 0), new Vector3(0.3f, 2.4f, 0.3f), hitam);
        Bag("TiangKanan", new Vector3(4.3f, 1.2f, 0), new Vector3(0.3f, 2.4f, 0.3f), hitam);
        // palang melintang tinggi (harus DITUNDUK)
        Bag("Palang", new Vector3(0, 1.9f, 0), new Vector3(9.0f, 0.5f, 0.35f), kuning);
        // garis peringatan hitam
        for (int i = 0; i < 5; i++)
            Bag("Garis" + i, new Vector3(-3.2f + i * 1.6f, 1.9f, -0.19f), new Vector3(0.5f, 0.5f, 0.05f), hitam);
    }

    Material Buat(Shader s, Color c)
    {
        Material m = new Material(s);
        m.hideFlags = HideFlags.DontSave;
        m.color = c;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
        return m;
    }

    void Bag(string n, Vector3 lp, Vector3 ls, Material m)
    {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        g.name = n;
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.localPosition = lp;
        g.transform.localScale = ls;
        g.GetComponent<Renderer>().sharedMaterial = m;
        Collider c = g.GetComponent<Collider>();
        if (c != null) Hancur(c);
    }
}
