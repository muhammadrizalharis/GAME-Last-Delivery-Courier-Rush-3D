using UnityEngine;

// Merakit PALANG PENGHALANG. [ExecuteAlways] -> tampil di editor juga.
[ExecuteAlways]
public class RakitPapan : MonoBehaviour
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

        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr != null) mr.enabled = false;

        Shader s = Shader.Find("Universal Render Pipeline/Lit");
        if (s == null) s = Shader.Find("Standard");

        GameObject vis = new GameObject("Visual");
        vis.hideFlags = HideFlags.DontSave;
        vis.transform.SetParent(transform, false);
        root = vis.transform;

        Material putih = Buat(s, new Color(0.95f, 0.95f, 0.95f));
        Material merah = Buat(s, new Color(0.85f, 0.2f, 0.2f));

        Bag("TiangKiri", new Vector3(-0.9f, -0.3f, 0), new Vector3(0.18f, 1.6f, 0.18f), putih);
        Bag("TiangKanan", new Vector3(0.9f, -0.3f, 0), new Vector3(0.18f, 1.6f, 0.18f), putih);
        for (int i = 0; i < 5; i++)
        {
            Material m = (i % 2 == 0) ? merah : putih;
            Bag("Garis" + i, new Vector3(-0.8f + i * 0.4f, 0.35f, 0), new Vector3(0.4f, 0.5f, 0.22f), m);
        }
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
