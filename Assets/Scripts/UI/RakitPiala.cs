using UnityEngine;

// Piala emas untuk layar TAMAT. Dibangun dari primitif, berputar pelan saat Play.
[ExecuteAlways]
public class RakitPiala : MonoBehaviour
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
        if (lama) Hancur(lama.gameObject);

        Shader s = Shader.Find("Universal Render Pipeline/Lit");
        if (s == null) s = Shader.Find("Standard");
        Material emas = Buat(s, new Color(1f, 0.82f, 0.2f));
        Material alas = Buat(s, new Color(0.22f, 0.14f, 0.07f));

        GameObject v = new GameObject("Visual");
        v.hideFlags = HideFlags.DontSave;
        v.transform.SetParent(transform, false);
        root = v.transform;

        // mangkuk piala
        Bag(PrimitiveType.Cylinder, new Vector3(0, 1.15f, 0), new Vector3(1.1f, 0.5f, 1.1f), Quaternion.identity, emas);
        Bag(PrimitiveType.Sphere, new Vector3(0, 0.82f, 0), new Vector3(1.0f, 0.95f, 1.0f), Quaternion.identity, emas);
        // batang
        Bag(PrimitiveType.Cylinder, new Vector3(0, 0.38f, 0), new Vector3(0.22f, 0.42f, 0.22f), Quaternion.identity, emas);
        // gagang kiri & kanan
        Bag(PrimitiveType.Cylinder, new Vector3(-0.72f, 1.2f, 0), new Vector3(0.12f, 0.5f, 0.12f), Quaternion.Euler(0, 0, 38), emas);
        Bag(PrimitiveType.Cylinder, new Vector3(0.72f, 1.2f, 0), new Vector3(0.12f, 0.5f, 0.12f), Quaternion.Euler(0, 0, -38), emas);
        // dasar dua tingkat
        Bag(PrimitiveType.Cylinder, new Vector3(0, 0.12f, 0), new Vector3(0.85f, 0.12f, 0.85f), Quaternion.identity, alas);
        Bag(PrimitiveType.Cube, new Vector3(0, -0.02f, 0), new Vector3(1.0f, 0.2f, 1.0f), Quaternion.identity, alas);
    }

    Material Buat(Shader s, Color c)
    {
        Material m = new Material(s) { hideFlags = HideFlags.DontSave };
        m.color = c;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
        if (m.HasProperty("_Metallic")) m.SetFloat("_Metallic", 0.6f);
        if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", 0.75f);
        return m;
    }

    void Bag(PrimitiveType t, Vector3 lp, Vector3 ls, Quaternion rot, Material m)
    {
        GameObject g = GameObject.CreatePrimitive(t);
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.localPosition = lp;
        g.transform.localScale = ls;
        g.transform.localRotation = rot;
        g.GetComponent<Renderer>().sharedMaterial = m;
        Collider c = g.GetComponent<Collider>();
        if (c != null) Hancur(c);
    }

    void Update()
    {
        if (Application.isPlaying) transform.Rotate(0f, 45f * Time.deltaTime, 0f);
    }
}
