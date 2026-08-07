using UnityEngine;

// Merakit GEROBAK pasar (badan kayu + 4 roda + tenda + dagangan + gagang) dari primitif.
// Dibangun di depan pendorong (offsetZ). [ExecuteAlways].
[ExecuteAlways]
public class RakitGerobak : MonoBehaviour
{
    public Color warna = new Color(0.9f, 0.4f, 0.2f);
    public float offsetZ = 1.4f;
    Transform root;

    void OnEnable() { Bangun(); }

    void Hancur(Object o)
    {
        if (o == null) return;
        if (Application.isPlaying) Destroy(o); else DestroyImmediate(o);
    }

    void Bangun()
    {
        Transform lama = transform.Find("Gerobak");
        if (lama) Hancur(lama.gameObject);

        Shader s = Shader.Find("Universal Render Pipeline/Lit");
        if (s == null) s = Shader.Find("Standard");
        Material kayu = Buat(s, new Color(0.45f, 0.3f, 0.16f));
        Material roda = Buat(s, new Color(0.14f, 0.13f, 0.12f));
        Material tenda = Buat(s, warna);
        Material buahA = Buat(s, new Color(0.9f, 0.72f, 0.2f));
        Material buahB = Buat(s, new Color(0.85f, 0.25f, 0.25f));

        GameObject v = new GameObject("Gerobak");
        v.hideFlags = HideFlags.DontSave;
        v.transform.SetParent(transform, false);
        root = v.transform;

        float z = offsetZ;
        Bag(new Vector3(0, 0.6f, z), new Vector3(1.0f, 0.5f, 1.4f), kayu);      // badan
        Bag(new Vector3(0, 0.9f, z), new Vector3(1.15f, 0.1f, 1.55f), kayu);    // permukaan
        Roda(new Vector3(-0.55f, 0.3f, z + 0.35f), roda);
        Roda(new Vector3(0.55f, 0.3f, z + 0.35f), roda);
        Roda(new Vector3(-0.55f, 0.3f, z - 0.35f), roda);
        Roda(new Vector3(0.55f, 0.3f, z - 0.35f), roda);
        Bag(new Vector3(-0.45f, 1.45f, z - 0.55f), new Vector3(0.08f, 1.1f, 0.08f), kayu); // tiang
        Bag(new Vector3(0.45f, 1.45f, z - 0.55f), new Vector3(0.08f, 1.1f, 0.08f), kayu);
        Bag(new Vector3(-0.45f, 1.45f, z + 0.55f), new Vector3(0.08f, 1.1f, 0.08f), kayu);
        Bag(new Vector3(0.45f, 1.45f, z + 0.55f), new Vector3(0.08f, 1.1f, 0.08f), kayu);
        Bag(new Vector3(0, 2.0f, z), new Vector3(1.35f, 0.08f, 1.75f), tenda);  // atap tenda
        Bola(new Vector3(-0.25f, 1.05f, z - 0.3f), 0.28f, buahB);               // dagangan
        Bola(new Vector3(0.25f, 1.05f, z), 0.28f, buahA);
        Bola(new Vector3(0, 1.05f, z + 0.35f), 0.28f, buahB);
        Bag(new Vector3(-0.35f, 0.75f, z - 1.0f), new Vector3(0.08f, 0.08f, 0.85f), kayu); // gagang
        Bag(new Vector3(0.35f, 0.75f, z - 1.0f), new Vector3(0.08f, 0.08f, 0.85f), kayu);
    }

    Material Buat(Shader s, Color c)
    {
        Material m = new Material(s) { hideFlags = HideFlags.DontSave };
        m.color = c;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
        return m;
    }

    void Bag(Vector3 lp, Vector3 ls, Material m)
    {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.localPosition = lp;
        g.transform.localScale = ls;
        g.GetComponent<Renderer>().sharedMaterial = m;
        Collider c = g.GetComponent<Collider>();
        if (c != null) Hancur(c);
    }

    void Roda(Vector3 lp, Material m)
    {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.localPosition = lp;
        g.transform.localScale = new Vector3(0.5f, 0.08f, 0.5f);
        g.transform.localRotation = Quaternion.Euler(0, 0, 90);
        g.GetComponent<Renderer>().sharedMaterial = m;
        Collider c = g.GetComponent<Collider>();
        if (c != null) Hancur(c);
    }

    void Bola(Vector3 lp, float d, Material m)
    {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.localPosition = lp;
        g.transform.localScale = new Vector3(d, d, d);
        g.GetComponent<Renderer>().sharedMaterial = m;
        Collider c = g.GetComponent<Collider>();
        if (c != null) Hancur(c);
    }
}
