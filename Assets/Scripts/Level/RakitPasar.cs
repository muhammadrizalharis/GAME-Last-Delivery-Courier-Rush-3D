using UnityEngine;

// Membangun suasana PASAR MALAM: lantai jalan + bangunan sisi + kios beratap tenda warna-warni
// + deretan lampion menyala melintang. [ExecuteAlways]. Beda dari rel kereta / terowongan.
[ExecuteAlways]
public class RakitPasar : MonoBehaviour
{
    public float mulaiZ = -20f;
    public float panjang = 380f;
    public float lebar = 6.3f;       // setengah lebar jalan
    public float jarakStan = 18f;
    Transform root;

    static readonly Color[] festif = {
        new Color(0.92f, 0.22f, 0.22f), new Color(0.96f, 0.6f, 0.12f), new Color(0.96f, 0.86f, 0.22f),
        new Color(0.24f, 0.72f, 0.38f), new Color(0.32f, 0.52f, 0.95f), new Color(0.82f, 0.32f, 0.72f)
    };

    void OnEnable() { Bangun(); }

    void Hancur(Object o)
    {
        if (o == null) return;
        if (Application.isPlaying) Destroy(o); else DestroyImmediate(o);
    }

    void Bangun()
    {
        Transform lama = transform.Find("Pasar");
        if (lama != null) Hancur(lama.gameObject);

        Shader s = Shader.Find("Universal Render Pipeline/Lit");
        if (s == null) s = Shader.Find("Standard");
        Material lantai = Buat(s, new Color(0.3f, 0.24f, 0.19f), false);
        Material kayu = Buat(s, new Color(0.36f, 0.24f, 0.14f), false);
        Material gedung = Buat(s, new Color(0.16f, 0.15f, 0.2f), false);

        GameObject vis = new GameObject("Pasar");
        vis.hideFlags = HideFlags.DontSave;
        vis.transform.SetParent(transform, false);
        root = vis.transform;

        float tengahZ = mulaiZ + panjang * 0.5f;

        // lantai jalan pasar (tanpa collider, pemain pakai hard-floor)
        Bag(new Vector3(0, 0f, tengahZ), new Vector3(lebar * 2f, 0.2f, panjang), lantai);
        // bangunan gelap di belakang kios (kiri & kanan)
        Bag(new Vector3(-lebar - 1.2f, 3f, tengahZ), new Vector3(2f, 6f, panjang), gedung);
        Bag(new Vector3(lebar + 1.2f, 3f, tengahZ), new Vector3(2f, 6f, panjang), gedung);

        int n = Mathf.Max(1, Mathf.RoundToInt(panjang / jarakStan));
        for (int i = 0; i <= n; i++)
        {
            float z = mulaiZ + i * jarakStan;
            Material tenda = Buat(s, festif[i % festif.Length], false);
            Stan(new Vector3(-lebar + 0.4f, 0, z), -1f, kayu, tenda);
            Stan(new Vector3(lebar - 0.4f, 0, z), 1f, kayu, tenda);

            // deretan lampion menyala melintang di atas jalan
            Material lampion = Buat(s, festif[(i + 2) % festif.Length], true);
            for (int j = -2; j <= 2; j++)
                Bola(new Vector3(j * 2f, 4.5f, z), 0.34f, lampion);
            Bag(new Vector3(0, 4.75f, z), new Vector3(lebar * 2f, 0.05f, 0.05f), kayu);
        }
    }

    void Stan(Vector3 p, float sisi, Material kayu, Material tenda)
    {
        Bag(p + new Vector3(0, 0.3f, 0), new Vector3(1.4f, 0.6f, 2.4f), kayu);      // badan meja
        Bag(p + new Vector3(0, 0.62f, 0), new Vector3(1.7f, 0.12f, 2.7f), kayu);    // permukaan meja
        Bag(p + new Vector3(0, 1.4f, -1f), new Vector3(0.12f, 1.7f, 0.12f), kayu);  // tiang belakang
        Bag(p + new Vector3(0, 1.4f, 1f), new Vector3(0.12f, 1.7f, 0.12f), kayu);   // tiang depan
        GameObject a = Bag(p + new Vector3(sisi * 0.3f, 2.25f, 0), new Vector3(1.9f, 0.1f, 2.7f), tenda);
        a.transform.localRotation = Quaternion.Euler(0, 0, sisi * 18f);             // atap tenda miring
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
            m.SetColor("_EmissionColor", c * 1.8f);
        }
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
