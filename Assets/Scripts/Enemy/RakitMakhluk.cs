using UnityEngine;

// Merakit MAKHLUK boss "Tunnel Rider": badan besar gelap, mata menyala, tanduk, taring,
// + lampu menyilaukan (bikin silau di terowongan gelap). [ExecuteAlways]. Menghadap -z (ke pemain).
[ExecuteAlways]
public class RakitMakhluk : MonoBehaviour
{
    Transform vis;

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
        Material kulit = Buat(s, new Color(0.12f, 0.1f, 0.14f), false);
        Material mata = Buat(s, new Color(1f, 0.3f, 0.1f), true);
        Material taring = Buat(s, new Color(0.9f, 0.9f, 0.85f), false);

        GameObject v = new GameObject("Visual");
        v.hideFlags = HideFlags.DontSave;
        v.transform.SetParent(transform, false);
        vis = v.transform;

        Bag(PrimitiveType.Sphere, "Badan", new Vector3(0f, 0.9f, 0f), new Vector3(2.2f, 2.0f, 2.2f), kulit);
        Bag(PrimitiveType.Sphere, "Kepala", new Vector3(0f, 1.7f, -0.6f), new Vector3(1.4f, 1.3f, 1.4f), kulit);
        Bag(PrimitiveType.Sphere, "MataKi", new Vector3(-0.4f, 1.9f, -1.2f), new Vector3(0.35f, 0.35f, 0.35f), mata);
        Bag(PrimitiveType.Sphere, "MataKa", new Vector3(0.4f, 1.9f, -1.2f), new Vector3(0.35f, 0.35f, 0.35f), mata);
        Bag(PrimitiveType.Cube, "TandukKi", new Vector3(-0.5f, 2.5f, -0.3f), new Vector3(0.22f, 0.9f, 0.22f), taring);
        Bag(PrimitiveType.Cube, "TandukKa", new Vector3(0.5f, 2.5f, -0.3f), new Vector3(0.22f, 0.9f, 0.22f), taring);
        Bag(PrimitiveType.Cube, "Mulut", new Vector3(0f, 1.3f, -1.25f), new Vector3(0.9f, 0.25f, 0.2f), taring);
        Bag(PrimitiveType.Sphere, "CakarKi", new Vector3(-1.3f, 0.7f, 0f), new Vector3(0.7f, 0.7f, 0.7f), kulit);
        Bag(PrimitiveType.Sphere, "CakarKa", new Vector3(1.3f, 0.7f, 0f), new Vector3(0.7f, 0.7f, 0.7f), kulit);

        // lampu menyilaukan di depan makhluk
        GameObject L = new GameObject("Silau");
        L.hideFlags = HideFlags.DontSave;
        L.transform.SetParent(vis, false);
        L.transform.localPosition = new Vector3(0f, 1.7f, -1.4f);
        Light li = L.AddComponent<Light>();
        li.type = LightType.Point;
        li.color = new Color(1f, 0.5f, 0.3f);
        li.intensity = 6f;
        li.range = 18f;
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
            m.SetColor("_EmissionColor", c * 2f);
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
