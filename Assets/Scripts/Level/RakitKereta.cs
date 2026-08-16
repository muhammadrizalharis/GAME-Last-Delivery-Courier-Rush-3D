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
        Material kaca = Buat(s, new Color(0.09f, 0.12f, 0.17f));
        Material silver = Buat(s, new Color(0.74f, 0.76f, 0.8f));
        Material gelap = Buat(s, new Color(0.12f, 0.12f, 0.14f));
        Material logam = Buat(s, new Color(0.55f, 0.57f, 0.62f));
        Material lampu = Buat(s, new Color(1f, 0.95f, 0.75f), true);

        // ---- badan utama + moncong kabin (nose menghadap -z / arah jalan) ----
        Bag(PrimitiveType.Cube, "Bodi", new Vector3(0, 1.35f, 0f), new Vector3(2f, 2f, 3.9f), Quaternion.identity, bodi);
        Bag(PrimitiveType.Cube, "Nose", new Vector3(0, 1.3f, -2.02f), new Vector3(1.9f, 1.85f, 0.4f), Quaternion.identity, bodi);
        Bag(PrimitiveType.Cube, "Atap", new Vector3(0, 2.42f, 0.1f), new Vector3(1.92f, 0.24f, 3.75f), Quaternion.identity, silver);
        Bag(PrimitiveType.Cube, "Skirt", new Vector3(0, 0.5f, 0f), new Vector3(2.04f, 0.6f, 3.8f), Quaternion.identity, gelap);

        // ---- pita jendela (silver) + jendela gelap + pintu + garis livery, tiap sisi ----
        foreach (int sisi in new int[] { -1, 1 })
        {
            float x = sisi * 1.01f;
            Bag(PrimitiveType.Cube, "PitaKaca", new Vector3(x, 1.78f, 0f), new Vector3(0.05f, 0.72f, 3.6f), Quaternion.identity, silver);
            for (int i = -1; i <= 1; i++)
                Bag(PrimitiveType.Cube, "Kaca", new Vector3(x + sisi * 0.01f, 1.78f, i * 1.15f), new Vector3(0.06f, 0.54f, 0.85f), Quaternion.identity, kaca);
            Bag(PrimitiveType.Cube, "Pintu", new Vector3(x + sisi * 0.01f, 1.15f, 1.6f), new Vector3(0.06f, 1.6f, 0.6f), Quaternion.identity, gelap);
            Bag(PrimitiveType.Cube, "Pintu2", new Vector3(x + sisi * 0.01f, 1.15f, -1.6f), new Vector3(0.06f, 1.6f, 0.6f), Quaternion.identity, gelap);
            Bag(PrimitiveType.Cube, "Livery", new Vector3(x + sisi * 0.015f, 1.02f, 0f), new Vector3(0.04f, 0.22f, 3.7f), Quaternion.identity, gelap);
        }

        // ---- kaca depan miring + papan tujuan menyala + lampu ----
        Bag(PrimitiveType.Cube, "KacaDepan", new Vector3(0, 1.98f, -2.02f), new Vector3(1.6f, 0.9f, 0.1f), Quaternion.Euler(16f, 0, 0), kaca);
        Bag(PrimitiveType.Cube, "PapanTujuan", new Vector3(0, 2.34f, -2.05f), new Vector3(1f, 0.24f, 0.06f), Quaternion.identity, lampu);
        Bag(PrimitiveType.Cube, "LampuKi", new Vector3(-0.62f, 0.8f, -2.18f), new Vector3(0.3f, 0.24f, 0.08f), Quaternion.identity, lampu);
        Bag(PrimitiveType.Cube, "LampuKa", new Vector3(0.62f, 0.8f, -2.18f), new Vector3(0.3f, 0.24f, 0.08f), Quaternion.identity, lampu);

        // ---- atap: unit AC + PANTOGRAF (menjulur ke kabel catenary) ----
        Bag(PrimitiveType.Cube, "UnitAC", new Vector3(0, 2.62f, 1f), new Vector3(1.3f, 0.28f, 1.1f), Quaternion.identity, gelap);
        Pantograf(new Vector3(0, 2.56f, -0.7f), logam);

        // ---- bogie beroda + kopling depan/belakang ----
        Bogie(1.35f, gelap, logam);
        Bogie(-1.35f, gelap, logam);
        Bag(PrimitiveType.Cube, "KoplingDpn", new Vector3(0, 0.45f, -2.12f), new Vector3(0.32f, 0.32f, 0.35f), Quaternion.identity, gelap);
        Bag(PrimitiveType.Cube, "KoplingBlk", new Vector3(0, 0.45f, 2.12f), new Vector3(0.32f, 0.32f, 0.35f), Quaternion.identity, gelap);
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

    void Bogie(float z, Material gelap, Material logam)
    {
        Bag(PrimitiveType.Cube, "BogieFrame", new Vector3(0, 0.55f, z), new Vector3(1.7f, 0.4f, 1.7f), Quaternion.identity, gelap);
        foreach (int sisi in new int[] { -1, 1 })
            foreach (float dz in new float[] { -0.55f, 0.55f })
                Roda("Roda", new Vector3(sisi * 0.92f, 0.42f, z + dz), logam);
    }

    void Pantograf(Vector3 pos, Material logam)
    {
        Bag(PrimitiveType.Cube, "PantoBase", pos + new Vector3(0, 0.05f, 0), new Vector3(0.8f, 0.12f, 0.6f), Quaternion.identity, logam);
        Bag(PrimitiveType.Cube, "PantoArm1", pos + new Vector3(0, 0.75f, -0.3f), new Vector3(0.08f, 1.6f, 0.08f), Quaternion.Euler(-36f, 0, 0), logam);
        Bag(PrimitiveType.Cube, "PantoArm2", pos + new Vector3(0, 1.6f, 0.25f), new Vector3(0.08f, 1.3f, 0.08f), Quaternion.Euler(42f, 0, 0), logam);
        Bag(PrimitiveType.Cube, "PantoBar", pos + new Vector3(0, 2.35f, 0.55f), new Vector3(1.5f, 0.06f, 0.12f), Quaternion.identity, logam);
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
