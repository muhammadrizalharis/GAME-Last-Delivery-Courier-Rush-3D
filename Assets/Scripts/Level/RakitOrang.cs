using UnityEngine;

// Merakit ORANG dari primitif kapsul/bola supaya mirip manusia (bukan kotak-kotak). [ExecuteAlways] -> tampil di editor.
// Kaki & lengan berporos di pinggul/bahu -> MELANGKAH otomatis saat berpindah (Play). Kaki di y=0 (napak di permukaan).
[ExecuteAlways]
public class RakitOrang : MonoBehaviour
{
    public Color warna = new Color(0.8f, 0.3f, 0.3f);

    Transform root, tungkaiKi, tungkaiKa, lenganKi, lenganKa;
    Vector3 posTerakhir;
    float gerak, fasa;

    void OnEnable()
    {
        Bangun();
        posTerakhir = transform.position;
        fasa = Random.value * 10f;
    }

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
        Material baju = Buat(s, warna);
        Material kulit = Buat(s, new Color(0.95f, 0.76f, 0.60f));
        Material celana = Buat(s, new Color(0.22f, 0.24f, 0.30f));
        Material sepatu = Buat(s, new Color(0.12f, 0.11f, 0.12f));
        Material rambut = Buat(s, new Color(0.16f, 0.12f, 0.09f));
        Material putih = Buat(s, Color.white);
        Material hitam = Buat(s, new Color(0.08f, 0.07f, 0.07f));
        Material mulut = Buat(s, new Color(0.55f, 0.28f, 0.26f));

        GameObject vis = new GameObject("Visual");
        vis.hideFlags = HideFlags.DontSave;
        vis.transform.SetParent(transform, false);
        root = vis.transform;

        // ---- KAKI (berporos di pinggul supaya bisa melangkah) ----
        tungkaiKi = Poros("TungkaiKiri", new Vector3(-0.13f, 0.82f, 0f));
        Bag(tungkaiKi, PrimitiveType.Capsule, new Vector3(0f, -0.4f, 0f), new Vector3(0.24f, 0.42f, 0.26f), celana);
        Bag(tungkaiKi, PrimitiveType.Cube, new Vector3(0f, -0.76f, 0.05f), new Vector3(0.26f, 0.12f, 0.36f), sepatu);
        tungkaiKa = Poros("TungkaiKanan", new Vector3(0.13f, 0.82f, 0f));
        Bag(tungkaiKa, PrimitiveType.Capsule, new Vector3(0f, -0.4f, 0f), new Vector3(0.24f, 0.42f, 0.26f), celana);
        Bag(tungkaiKa, PrimitiveType.Cube, new Vector3(0f, -0.76f, 0.05f), new Vector3(0.26f, 0.12f, 0.36f), sepatu);

        // ---- PINGGUL & BADAN (kapsul membulat, bukan kotak) ----
        Bag(root, PrimitiveType.Capsule, new Vector3(0f, 0.9f, 0f), new Vector3(0.5f, 0.28f, 0.36f), celana);
        Bag(root, PrimitiveType.Capsule, new Vector3(0f, 1.2f, 0f), new Vector3(0.6f, 0.5f, 0.4f), baju);

        // ---- LENGAN (berporos di bahu, berayun berlawanan kaki) ----
        lenganKi = Poros("LenganKiri", new Vector3(-0.34f, 1.5f, 0f));
        Bag(lenganKi, PrimitiveType.Capsule, new Vector3(0f, -0.32f, 0f), new Vector3(0.17f, 0.37f, 0.19f), baju);
        Bag(lenganKi, PrimitiveType.Sphere, new Vector3(0f, -0.66f, 0f), new Vector3(0.15f, 0.15f, 0.15f), kulit);
        lenganKa = Poros("LenganKanan", new Vector3(0.34f, 1.5f, 0f));
        Bag(lenganKa, PrimitiveType.Capsule, new Vector3(0f, -0.32f, 0f), new Vector3(0.17f, 0.37f, 0.19f), baju);
        Bag(lenganKa, PrimitiveType.Sphere, new Vector3(0f, -0.66f, 0f), new Vector3(0.15f, 0.15f, 0.15f), kulit);

        // ---- LEHER, KEPALA, RAMBUT ----
        Bag(root, PrimitiveType.Cylinder, new Vector3(0f, 1.6f, 0f), new Vector3(0.16f, 0.09f, 0.16f), kulit);
        Bag(root, PrimitiveType.Sphere, new Vector3(0f, 1.82f, 0f), new Vector3(0.42f, 0.48f, 0.42f), kulit);
        Bag(root, PrimitiveType.Sphere, new Vector3(0f, 1.9f, -0.03f), new Vector3(0.46f, 0.4f, 0.47f), rambut);

        // ---- WAJAH (mata + hidung + mulut) menghadap +z ----
        Bag(root, PrimitiveType.Sphere, new Vector3(-0.1f, 1.86f, 0.17f), new Vector3(0.12f, 0.15f, 0.09f), putih);
        Bag(root, PrimitiveType.Sphere, new Vector3(0.1f, 1.86f, 0.17f), new Vector3(0.12f, 0.15f, 0.09f), putih);
        Bag(root, PrimitiveType.Sphere, new Vector3(-0.1f, 1.86f, 0.22f), new Vector3(0.06f, 0.08f, 0.05f), hitam);
        Bag(root, PrimitiveType.Sphere, new Vector3(0.1f, 1.86f, 0.22f), new Vector3(0.06f, 0.08f, 0.05f), hitam);
        Bag(root, PrimitiveType.Sphere, new Vector3(0f, 1.79f, 0.21f), new Vector3(0.09f, 0.1f, 0.11f), kulit);
        Bag(root, PrimitiveType.Cube, new Vector3(0f, 1.71f, 0.19f), new Vector3(0.15f, 0.04f, 0.06f), mulut);

        gerak = 0f;
    }

    Transform Poros(string nama, Vector3 lp)
    {
        GameObject g = new GameObject(nama);
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.localPosition = lp;
        return g.transform;
    }

    Material Buat(Shader s, Color c)
    {
        Material m = new Material(s);
        m.hideFlags = HideFlags.DontSave;
        m.color = c;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
        return m;
    }

    void Bag(Transform parent, PrimitiveType t, Vector3 lp, Vector3 ls, Material m)
    {
        GameObject g = GameObject.CreatePrimitive(t);
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(parent, false);
        g.transform.localPosition = lp;
        g.transform.localScale = ls;
        g.GetComponent<Renderer>().sharedMaterial = m;
        Collider c = g.GetComponent<Collider>();
        if (c != null) Hancur(c);
    }

    void Update()
    {
        if (transform.Find("Visual") == null) { Bangun(); posTerakhir = transform.position; }
        if (!Application.isPlaying || root == null) return;

        float dt = Time.deltaTime;
        if (dt <= 0f) return;

        Vector3 d = transform.position - posTerakhir;
        posTerakhir = transform.position;
        float laju = new Vector2(d.x, d.z).magnitude / dt;    // laju mendatar -> blend animasi jalan
        gerak = Mathf.MoveTowards(gerak, laju > 0.4f ? 1f : 0f, dt * 5f);

        float t = (Time.time + fasa) * 8f;
        float ayun = Mathf.Sin(t) * 38f * gerak;
        if (tungkaiKi) tungkaiKi.localRotation = Quaternion.Euler(ayun, 0f, 0f);
        if (tungkaiKa) tungkaiKa.localRotation = Quaternion.Euler(-ayun, 0f, 0f);
        if (lenganKi) lenganKi.localRotation = Quaternion.Euler(-ayun * 0.7f, 0f, 0f);
        if (lenganKa) lenganKa.localRotation = Quaternion.Euler(ayun * 0.7f, 0f, 0f);
        root.localPosition = new Vector3(0f, Mathf.Abs(Mathf.Sin(t)) * 0.05f * gerak, 0f);
    }
}
