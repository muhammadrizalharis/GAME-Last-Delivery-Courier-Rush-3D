using UnityEngine;

// Merakit BATU (bongkahan) dari primitif. [ExecuteAlways] -> tampil di editor.
[ExecuteAlways]
public class RakitBatu : MonoBehaviour
{
    public Color warna = new Color(0.5f, 0.5f, 0.53f);
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

        Material m1 = Buat(s, warna);
        Material m2 = Buat(s, new Color(warna.r * 0.8f, warna.g * 0.8f, warna.b * 0.8f));

        Bongkah(new Vector3(0, 0.2f, 0), new Vector3(1.5f, 1.2f, 1.4f), new Vector3(12, 20, 8), m1);
        Bongkah(new Vector3(-0.8f, 0.0f, 0.3f), new Vector3(1.0f, 0.9f, 1.0f), new Vector3(-15, 40, 10), m2);
        Bongkah(new Vector3(0.8f, -0.05f, -0.2f), new Vector3(0.9f, 0.8f, 0.9f), new Vector3(20, -25, -12), m2);
        Bongkah(new Vector3(0.1f, 0.7f, -0.1f), new Vector3(0.8f, 0.7f, 0.8f), new Vector3(30, 15, 20), m1);
    }

    Material Buat(Shader s, Color c)
    {
        Material m = new Material(s);
        m.hideFlags = HideFlags.DontSave;
        m.color = c;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
        return m;
    }

    void Bongkah(Vector3 lp, Vector3 ls, Vector3 rot, Material m)
    {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.localPosition = lp;
        g.transform.localScale = ls;
        g.transform.localRotation = Quaternion.Euler(rot);
        g.GetComponent<Renderer>().sharedMaterial = m;
        Collider c = g.GetComponent<Collider>();
        if (c != null) Hancur(c);
    }
}
