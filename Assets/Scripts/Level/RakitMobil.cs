using UnityEngine;

// Merakit bentuk MOBIL dari primitif. [ExecuteAlways] -> tampil di editor juga.
[ExecuteAlways]
public class RakitMobil : MonoBehaviour
{
    public Color warna = new Color(0.85f, 0.25f, 0.2f);
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

        Material body = Buat(s, warna);
        Material kaca = Buat(s, new Color(0.55f, 0.72f, 0.88f));
        Material roda = Buat(s, new Color(0.1f, 0.1f, 0.12f));
        Material lampu = Buat(s, new Color(1f, 0.9f, 0.5f));

        Bag(PrimitiveType.Cube, "Bodi", new Vector3(0, 0, 0), new Vector3(2, 0.6f, 3.4f), body);
        Bag(PrimitiveType.Cube, "Kabin", new Vector3(0, 0.5f, -0.2f), new Vector3(1.5f, 0.6f, 1.6f), kaca);
        Bag(PrimitiveType.Cube, "Lampu", new Vector3(0, 0, 1.75f), new Vector3(1.6f, 0.3f, 0.1f), lampu);
        Roda("R1", new Vector3(-0.95f, -0.35f, 1.1f), roda);
        Roda("R2", new Vector3(0.95f, -0.35f, 1.1f), roda);
        Roda("R3", new Vector3(-0.95f, -0.35f, -1.1f), roda);
        Roda("R4", new Vector3(0.95f, -0.35f, -1.1f), roda);
    }

    Material Buat(Shader s, Color c)
    {
        Material m = new Material(s);
        m.hideFlags = HideFlags.DontSave;
        m.color = c;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
        return m;
    }

    void Bag(PrimitiveType t, string n, Vector3 lp, Vector3 ls, Material m)
    {
        GameObject g = GameObject.CreatePrimitive(t);
        g.name = n;
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.localPosition = lp;
        g.transform.localScale = ls;
        g.GetComponent<Renderer>().sharedMaterial = m;
        Collider c = g.GetComponent<Collider>();
        if (c != null) Hancur(c);
    }

    void Roda(string n, Vector3 lp, Material m)
    {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        g.name = n;
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.localPosition = lp;
        g.transform.localScale = new Vector3(0.55f, 0.15f, 0.55f);
        g.transform.localRotation = Quaternion.Euler(0, 0, 90);
        g.GetComponent<Renderer>().sharedMaterial = m;
        Collider c = g.GetComponent<Collider>();
        if (c != null) Hancur(c);
    }
}
