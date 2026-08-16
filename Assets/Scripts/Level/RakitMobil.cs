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
        Material bodi2 = Buat(s, warna * 0.82f);
        Material kaca = Buat(s, new Color(0.12f, 0.15f, 0.22f));
        Material ban = Buat(s, new Color(0.07f, 0.07f, 0.09f));
        Material velg = Buat(s, new Color(0.72f, 0.74f, 0.78f));
        Material lampu = Buat(s, new Color(1f, 0.96f, 0.7f), true);
        Material rem = Buat(s, new Color(1f, 0.2f, 0.15f), true);
        Material gelap = Buat(s, new Color(0.16f, 0.16f, 0.18f));

        // ---- badan mobil bertingkat (sasis + kap + bagasi + kabin + atap) ----
        Bag(PrimitiveType.Cube, "Sasis", new Vector3(0, 0.02f, 0f), new Vector3(1.9f, 0.5f, 3.6f), body);
        Bag(PrimitiveType.Cube, "Rocker", new Vector3(0, -0.2f, 0f), new Vector3(1.94f, 0.22f, 3.3f), gelap);
        Bag(PrimitiveType.Cube, "KapDepan", new Vector3(0, 0.26f, -1.35f), new Vector3(1.82f, 0.12f, 1f), body);
        Bag(PrimitiveType.Cube, "Bagasi", new Vector3(0, 0.26f, 1.4f), new Vector3(1.82f, 0.12f, 0.9f), body);
        Bag(PrimitiveType.Cube, "Kabin", new Vector3(0, 0.52f, 0.15f), new Vector3(1.64f, 0.5f, 1.7f), body);
        Bag(PrimitiveType.Cube, "Atap", new Vector3(0, 0.76f, 0.2f), new Vector3(1.5f, 0.12f, 1.5f), bodi2);

        // ---- kaca: windshield miring + kaca belakang + kaca samping ----
        GameObject kd = Bag(PrimitiveType.Cube, "KacaDepan", new Vector3(0, 0.52f, -0.82f), new Vector3(1.5f, 0.62f, 0.09f), kaca);
        kd.transform.localRotation = Quaternion.Euler(32f, 0f, 0f);
        GameObject kb = Bag(PrimitiveType.Cube, "KacaBelakang", new Vector3(0, 0.52f, 1.05f), new Vector3(1.5f, 0.56f, 0.09f), kaca);
        kb.transform.localRotation = Quaternion.Euler(-30f, 0f, 0f);
        Bag(PrimitiveType.Cube, "KacaKiri", new Vector3(-0.83f, 0.54f, 0.18f), new Vector3(0.08f, 0.34f, 1.4f), kaca);
        Bag(PrimitiveType.Cube, "KacaKanan", new Vector3(0.83f, 0.54f, 0.18f), new Vector3(0.08f, 0.34f, 1.4f), kaca);

        // ---- lampu depan + gril + bumper depan (nose menghadap -z, ke pemain) ----
        Bag(PrimitiveType.Cube, "LampuKi", new Vector3(-0.62f, 0.12f, -1.83f), new Vector3(0.4f, 0.2f, 0.08f), lampu);
        Bag(PrimitiveType.Cube, "LampuKa", new Vector3(0.62f, 0.12f, -1.83f), new Vector3(0.4f, 0.2f, 0.08f), lampu);
        Bag(PrimitiveType.Cube, "Gril", new Vector3(0, 0.02f, -1.84f), new Vector3(0.95f, 0.26f, 0.06f), gelap);
        Bag(PrimitiveType.Cube, "BumperDepan", new Vector3(0, -0.16f, -1.83f), new Vector3(1.88f, 0.26f, 0.16f), gelap);

        // ---- lampu rem + bumper belakang ----
        Bag(PrimitiveType.Cube, "RemKi", new Vector3(-0.62f, 0.16f, 1.83f), new Vector3(0.42f, 0.22f, 0.08f), rem);
        Bag(PrimitiveType.Cube, "RemKa", new Vector3(0.62f, 0.16f, 1.83f), new Vector3(0.42f, 0.22f, 0.08f), rem);
        Bag(PrimitiveType.Cube, "BumperBelakang", new Vector3(0, -0.16f, 1.83f), new Vector3(1.88f, 0.26f, 0.16f), gelap);

        // ---- spion ----
        Bag(PrimitiveType.Cube, "SpionKi", new Vector3(-0.92f, 0.5f, -0.5f), new Vector3(0.14f, 0.1f, 0.2f), bodi2);
        Bag(PrimitiveType.Cube, "SpionKa", new Vector3(0.92f, 0.5f, -0.5f), new Vector3(0.14f, 0.1f, 0.2f), bodi2);

        // ---- roda (ban + velg) ----
        Roda("R1", new Vector3(-0.97f, -0.25f, 1.15f), ban, velg);
        Roda("R2", new Vector3(0.97f, -0.25f, 1.15f), ban, velg);
        Roda("R3", new Vector3(-0.97f, -0.25f, -1.15f), ban, velg);
        Roda("R4", new Vector3(0.97f, -0.25f, -1.15f), ban, velg);
    }

    Material Buat(Shader s, Color c, bool emisi = false)
    {
        Material m = new Material(s);
        m.hideFlags = HideFlags.DontSave;
        m.color = c;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
        if (emisi)
        {
            m.EnableKeyword("_EMISSION");
            m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            m.SetColor("_EmissionColor", c * 1.4f);
        }
        return m;
    }

    GameObject Bag(PrimitiveType t, string n, Vector3 lp, Vector3 ls, Material m)
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
        return g;
    }

    void Roda(string n, Vector3 lp, Material ban, Material velg)
    {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        g.name = n;
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.localPosition = lp;
        g.transform.localScale = new Vector3(0.62f, 0.17f, 0.62f);
        g.transform.localRotation = Quaternion.Euler(0, 0, 90);
        g.GetComponent<Renderer>().sharedMaterial = ban;
        Collider c = g.GetComponent<Collider>();
        if (c != null) Hancur(c);
        GameObject h = GameObject.CreatePrimitive(PrimitiveType.Cylinder);   // velg di tengah roda
        h.name = n + "Velg";
        h.hideFlags = HideFlags.DontSave;
        h.transform.SetParent(root, false);
        h.transform.localPosition = lp;
        h.transform.localScale = new Vector3(0.34f, 0.19f, 0.34f);
        h.transform.localRotation = Quaternion.Euler(0, 0, 90);
        h.GetComponent<Renderer>().sharedMaterial = velg;
        Collider c2 = h.GetComponent<Collider>();
        if (c2 != null) Hancur(c2);
    }
}
