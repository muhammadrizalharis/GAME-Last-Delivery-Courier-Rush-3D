using UnityEngine;

// Merakit LOKOMOTIF (kereta uap) dari primitif. [ExecuteAlways] -> tampil di editor.
[ExecuteAlways]
public class RakitKereta : MonoBehaviour
{
    public Color warna = new Color(0.22f, 0.28f, 0.3f);
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

        Material bodi = Buat(s, warna);
        Material gelap = Buat(s, new Color(0.1f, 0.1f, 0.12f));
        Material logam = Buat(s, new Color(0.6f, 0.62f, 0.66f));
        Material lampu = Buat(s, new Color(1f, 0.92f, 0.55f));

        // boiler (tabung depan) - sumbu Z
        Bag(PrimitiveType.Cylinder, "Boiler", new Vector3(0, 0.85f, -0.4f), new Vector3(1.5f, 1.5f, 1.5f), Quaternion.Euler(90, 0, 0), bodi);
        // kabin masinis di belakang
        Bag(PrimitiveType.Cube, "Kabin", new Vector3(0, 1.15f, 1.35f), new Vector3(1.7f, 1.7f, 1.3f), Quaternion.identity, bodi);
        // atap kabin
        Bag(PrimitiveType.Cube, "Atap", new Vector3(0, 2.0f, 1.35f), new Vector3(1.9f, 0.2f, 1.5f), Quaternion.identity, gelap);
        // cerobong asap
        Bag(PrimitiveType.Cylinder, "Cerobong", new Vector3(0, 1.95f, -1.0f), new Vector3(0.45f, 0.5f, 0.45f), Quaternion.identity, gelap);
        // kubah uap
        Bag(PrimitiveType.Sphere, "Kubah", new Vector3(0, 1.75f, 0.1f), new Vector3(0.6f, 0.5f, 0.6f), Quaternion.identity, logam);
        // lampu depan
        Bag(PrimitiveType.Cylinder, "Lampu", new Vector3(0, 0.85f, -1.28f), new Vector3(0.4f, 0.08f, 0.4f), Quaternion.Euler(90, 0, 0), lampu);
        // penyapu depan (cowcatcher)
        Bag(PrimitiveType.Cube, "Sapu", new Vector3(0, 0.15f, -1.25f), new Vector3(1.4f, 0.5f, 0.5f), Quaternion.identity, gelap);
        // roda kiri-kanan
        for (int i = 0; i < 3; i++)
        {
            float z = -0.6f + i * 0.8f;
            Roda("RodaKi" + i, new Vector3(-0.85f, 0.35f, z), logam);
            Roda("RodaKa" + i, new Vector3(0.85f, 0.35f, z), logam);
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

    void Bag(PrimitiveType t, string n, Vector3 lp, Vector3 ls, Quaternion rot, Material m)
    {
        GameObject g = GameObject.CreatePrimitive(t);
        g.name = n;
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.localPosition = lp;
        g.transform.localScale = ls;
        g.transform.localRotation = rot;
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
        g.transform.localScale = new Vector3(0.7f, 0.12f, 0.7f);
        g.transform.localRotation = Quaternion.Euler(0, 0, 90);
        g.GetComponent<Renderer>().sharedMaterial = m;
        Collider c = g.GetComponent<Collider>();
        if (c != null) Hancur(c);
    }
}
