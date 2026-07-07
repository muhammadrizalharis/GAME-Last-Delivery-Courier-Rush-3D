using UnityEngine;

// Merakit PENAMBANG (NPC) dari primitif: overall + helm berlampu + beliung. [ExecuteAlways].
// Digerakkan MobilJalan (lari ke arah pemain). Ada gerak naik-turun (bob) biar seolah berlari.
[ExecuteAlways]
public class RakitPenambang : MonoBehaviour
{
    Transform vis;
    float bobT;

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
        Material baju = Buat(s, new Color(0.25f, 0.35f, 0.5f), false);
        Material kulit = Buat(s, new Color(0.9f, 0.72f, 0.55f), false);
        Material helm = Buat(s, new Color(0.95f, 0.8f, 0.15f), false);
        Material lampu = Buat(s, new Color(1f, 0.95f, 0.7f), true);
        Material gelap = Buat(s, new Color(0.15f, 0.15f, 0.15f), false);

        GameObject v = new GameObject("Visual");
        v.hideFlags = HideFlags.DontSave;
        v.transform.SetParent(transform, false);
        vis = v.transform;

        Bag(PrimitiveType.Cube, "KakiKi", new Vector3(-0.16f, -0.55f, 0f), new Vector3(0.24f, 0.9f, 0.28f), gelap);
        Bag(PrimitiveType.Cube, "KakiKa", new Vector3(0.16f, -0.55f, 0f), new Vector3(0.24f, 0.9f, 0.28f), gelap);
        Bag(PrimitiveType.Cube, "Badan", new Vector3(0f, 0.15f, 0f), new Vector3(0.6f, 0.8f, 0.38f), baju);
        Bag(PrimitiveType.Cube, "LenganKi", new Vector3(-0.4f, 0.2f, 0f), new Vector3(0.16f, 0.66f, 0.22f), baju);
        Bag(PrimitiveType.Cube, "LenganKa", new Vector3(0.4f, 0.2f, 0f), new Vector3(0.16f, 0.66f, 0.22f), baju);
        Bag(PrimitiveType.Sphere, "Kepala", new Vector3(0f, 0.75f, 0f), new Vector3(0.46f, 0.5f, 0.46f), kulit);
        // helm + lampu helm menghadap -z (ke arah pemain)
        Bag(PrimitiveType.Sphere, "Helm", new Vector3(0f, 0.92f, 0f), new Vector3(0.54f, 0.4f, 0.54f), helm);
        Bag(PrimitiveType.Cube, "LampuHelm", new Vector3(0f, 0.9f, -0.28f), new Vector3(0.16f, 0.16f, 0.1f), lampu);
        // beliung (pickaxe) sederhana
        Bag(PrimitiveType.Cube, "GagangBeliung", new Vector3(0.5f, 0.1f, 0.1f), new Vector3(0.06f, 0.8f, 0.06f), gelap);
        Bag(PrimitiveType.Cube, "MataBeliung", new Vector3(0.5f, 0.5f, 0.1f), new Vector3(0.5f, 0.1f, 0.1f), gelap);
    }

    void Update()
    {
        if (!Application.isPlaying || vis == null) return;
        bobT += Time.deltaTime * 11f;
        vis.localPosition = new Vector3(0f, Mathf.Abs(Mathf.Sin(bobT)) * 0.08f, 0f);
    }

    Material Buat(Shader s, Color c, bool emisi)
    {
        Material m = new Material(s);
        m.hideFlags = HideFlags.DontSave;
        m.color = c;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
        if (emisi && m.HasProperty("_EmissionColor"))
        {
            m.EnableKeyword("_EMISSION");
            m.SetColor("_EmissionColor", c * 1.5f);
        }
        return m;
    }

    void Bag(PrimitiveType t, string n, Vector3 lp, Vector3 ls, Material m)
    {
        GameObject g = GameObject.CreatePrimitive(t);
        g.name = n;
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(vis, false);
        g.transform.localPosition = lp;
        g.transform.localScale = ls;
        g.GetComponent<Renderer>().sharedMaterial = m;
        Collider c = g.GetComponent<Collider>();
        if (c != null) Hancur(c);
    }
}
