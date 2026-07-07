using UnityEngine;

// Merakit ORANG (pejalan pasar) dari primitif. [ExecuteAlways] -> tampil di editor. Menghadap +z.
[ExecuteAlways]
public class RakitOrang : MonoBehaviour
{
    public Color warna = new Color(0.8f, 0.3f, 0.3f);
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
        Material baju = Buat(s, warna);
        Material kulit = Buat(s, new Color(0.85f, 0.68f, 0.52f));
        Material gelap = Buat(s, new Color(0.2f, 0.18f, 0.16f));

        GameObject vis = new GameObject("Visual");
        vis.hideFlags = HideFlags.DontSave;
        vis.transform.SetParent(transform, false);
        root = vis.transform;

        // kaki
        Bag(PrimitiveType.Cube, new Vector3(-0.15f, 0.35f, 0), new Vector3(0.22f, 0.7f, 0.25f), gelap);
        Bag(PrimitiveType.Cube, new Vector3(0.15f, 0.35f, 0), new Vector3(0.22f, 0.7f, 0.25f), gelap);
        // badan
        Bag(PrimitiveType.Cube, new Vector3(0, 1.05f, 0), new Vector3(0.62f, 0.8f, 0.36f), baju);
        // lengan
        Bag(PrimitiveType.Cube, new Vector3(-0.42f, 1.05f, 0), new Vector3(0.18f, 0.72f, 0.22f), baju);
        Bag(PrimitiveType.Cube, new Vector3(0.42f, 1.05f, 0), new Vector3(0.18f, 0.72f, 0.22f), baju);
        // kepala
        Bag(PrimitiveType.Sphere, new Vector3(0, 1.68f, 0), new Vector3(0.44f, 0.5f, 0.44f), kulit);
        // caping/topi lebar
        Bag(PrimitiveType.Cylinder, new Vector3(0, 1.98f, 0), new Vector3(0.72f, 0.07f, 0.72f), gelap);
    }

    Material Buat(Shader s, Color c)
    {
        Material m = new Material(s);
        m.hideFlags = HideFlags.DontSave;
        m.color = c;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
        return m;
    }

    void Bag(PrimitiveType t, Vector3 lp, Vector3 ls, Material m)
    {
        GameObject g = GameObject.CreatePrimitive(t);
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.localPosition = lp;
        g.transform.localScale = ls;
        g.GetComponent<Renderer>().sharedMaterial = m;
        Collider c = g.GetComponent<Collider>();
        if (c != null) Hancur(c);
    }
}
