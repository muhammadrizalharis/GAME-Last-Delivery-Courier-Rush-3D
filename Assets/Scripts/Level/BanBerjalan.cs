using UnityEngine;

// Ban berjalan pabrik: saat pemain melintas, ia terdorong ke samping (arah). Pemain harus mengimbangi.
// Visual sabuk logam + panah menyala bergerak. [ExecuteAlways]. Collider trigger diatur di scene.
[ExecuteAlways]
public class BanBerjalan : MonoBehaviour
{
    public float arah = 1f;      // +1 dorong ke kanan, -1 ke kiri
    public float kekuatan = 4f;
    public float panjang = 12f;
    public float lebar = 6f;
    Transform root;
    Transform[] panah;

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
        Material logam = Buat(s, new Color(0.2f, 0.2f, 0.24f), false);
        Material rel = Buat(s, new Color(0.35f, 0.35f, 0.4f), false);
        Material nyala = Buat(s, new Color(0.95f, 0.7f, 0.15f), true);

        GameObject v = new GameObject("Visual");
        v.hideFlags = HideFlags.DontSave;
        v.transform.SetParent(transform, false);
        root = v.transform;

        Bag(new Vector3(0f, 0.12f, 0f), new Vector3(lebar, 0.16f, panjang), logam);
        Bag(new Vector3(-lebar / 2f, 0.25f, 0f), new Vector3(0.3f, 0.35f, panjang), rel);
        Bag(new Vector3(lebar / 2f, 0.25f, 0f), new Vector3(0.3f, 0.35f, panjang), rel);

        int n = Mathf.Max(2, Mathf.RoundToInt(panjang / 2.2f));
        panah = new Transform[n];
        for (int i = 0; i < n; i++)
        {
            float z = -panjang / 2f + (i + 0.5f) * (panjang / n);
            GameObject g = Bag(new Vector3(Random.Range(-lebar / 2f, lebar / 2f), 0.22f, z), new Vector3(1.3f, 0.06f, 0.5f), nyala);
            panah[i] = g.transform;
        }
    }

    Material Buat(Shader s, Color c, bool emisi)
    {
        Material m = new Material(s) { hideFlags = HideFlags.DontSave };
        m.color = c;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
        if (emisi && m.HasProperty("_EmissionColor")) { m.EnableKeyword("_EMISSION"); m.SetColor("_EmissionColor", c * 2f); }
        return m;
    }

    GameObject Bag(Vector3 lp, Vector3 ls, Material m)
    {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.localPosition = lp;
        g.transform.localScale = ls;
        g.GetComponent<Renderer>().sharedMaterial = m;
        Collider c = g.GetComponent<Collider>();
        if (c != null) Hancur(c);
        return g;
    }

    void Update()
    {
        if (!Application.isPlaying || panah == null) return;
        float batas = lebar / 2f;
        for (int i = 0; i < panah.Length; i++)
        {
            if (panah[i] == null) continue;
            Vector3 lp = panah[i].localPosition;
            lp.x += arah * 3.5f * Time.deltaTime;
            if (lp.x > batas) lp.x = -batas;
            if (lp.x < -batas) lp.x = batas;
            panah[i].localPosition = lp;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!Application.isPlaying) return;
        PlayerLari pl = other.GetComponent<PlayerLari>();
        if (pl != null) pl.dorongan = arah * kekuatan;
    }

    void OnTriggerExit(Collider other)
    {
        if (!Application.isPlaying) return;
        PlayerLari pl = other.GetComponent<PlayerLari>();
        if (pl != null) pl.dorongan = 0f;
    }
}
