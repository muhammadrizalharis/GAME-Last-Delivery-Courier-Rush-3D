using UnityEngine;

// Item nyawa berbentuk HATI. [ExecuteAlways] -> tampil di editor. Ambil -> tambah nyawa.
[ExecuteAlways]
public class HeartPickup : MonoBehaviour
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
        if (lama != null) Hancur(lama.gameObject);

        Shader s = Shader.Find("Universal Render Pipeline/Lit");
        if (s == null) s = Shader.Find("Standard");
        Material m = new Material(s);
        m.hideFlags = HideFlags.DontSave;
        Color c = new Color(0.9f, 0.2f, 0.3f);
        m.color = c;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);

        GameObject vis = new GameObject("Visual");
        vis.hideFlags = HideFlags.DontSave;
        vis.transform.SetParent(transform, false);
        root = vis.transform;

        Bag(PrimitiveType.Sphere, new Vector3(-0.22f, 0.2f, 0), new Vector3(0.55f, 0.55f, 0.4f), m, Quaternion.identity);
        Bag(PrimitiveType.Sphere, new Vector3(0.22f, 0.2f, 0), new Vector3(0.55f, 0.55f, 0.4f), m, Quaternion.identity);
        Bag(PrimitiveType.Cube, new Vector3(0, -0.08f, 0), new Vector3(0.55f, 0.55f, 0.4f), m, Quaternion.Euler(0, 0, 45));
    }

    void Bag(PrimitiveType t, Vector3 lp, Vector3 ls, Material m, Quaternion rot)
    {
        GameObject g = GameObject.CreatePrimitive(t);
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.localPosition = lp;
        g.transform.localScale = ls;
        g.transform.localRotation = rot;
        g.GetComponent<Renderer>().sharedMaterial = m;
        Collider col = g.GetComponent<Collider>();
        if (col != null) Hancur(col);
    }

    void Update()
    {
        transform.Rotate(0, 80 * Time.deltaTime, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (Application.isPlaying && other.CompareTag("Player"))
        {
            if (RunGame.instance != null) RunGame.instance.TambahNyawa();
            Destroy(gameObject);
        }
    }
}
