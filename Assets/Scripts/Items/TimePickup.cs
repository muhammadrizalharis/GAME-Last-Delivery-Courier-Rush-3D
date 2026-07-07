using UnityEngine;

// Item tambah WAKTU berbentuk jam (dari primitif). [ExecuteAlways] -> tampil di editor. Ambil -> tambah detik.
[ExecuteAlways]
public class TimePickup : MonoBehaviour
{
    public float tambahDetik = 10f;
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
        Material muka = Buat(s, new Color(0.35f, 0.8f, 0.95f));
        Material jarum = Buat(s, new Color(0.1f, 0.15f, 0.25f));

        GameObject vis = new GameObject("Visual");
        vis.hideFlags = HideFlags.DontSave;
        vis.transform.SetParent(transform, false);
        root = vis.transform;

        // muka jam (silinder pipih menghadap depan)
        Bag(PrimitiveType.Cylinder, new Vector3(0, 0, 0), new Vector3(0.8f, 0.08f, 0.8f), Quaternion.Euler(90, 0, 0), muka);
        // jarum jam
        Bag(PrimitiveType.Cube, new Vector3(0, 0.12f, -0.08f), new Vector3(0.07f, 0.4f, 0.05f), Quaternion.identity, jarum);
        Bag(PrimitiveType.Cube, new Vector3(0.14f, 0f, -0.08f), new Vector3(0.28f, 0.07f, 0.05f), Quaternion.identity, jarum);
    }

    Material Buat(Shader s, Color c)
    {
        Material m = new Material(s);
        m.hideFlags = HideFlags.DontSave;
        m.color = c;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
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
        Collider col = g.GetComponent<Collider>();
        if (col != null) Hancur(col);
    }

    void Update()
    {
        transform.Rotate(0, 70 * Time.deltaTime, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (Application.isPlaying && other.CompareTag("Player"))
        {
            if (RunGame.instance != null) RunGame.instance.TambahWaktu(tambahDetik);
            Destroy(gameObject);
        }
    }
}
