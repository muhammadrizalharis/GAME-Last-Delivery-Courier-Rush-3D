using UnityEngine;

// Membangun langit-langit TEROWONGAN gelap (plafon + rusuk + lampu redup) sepanjang lintasan. [ExecuteAlways].
[ExecuteAlways]
public class RakitTerowongan : MonoBehaviour
{
    public float mulaiZ = -20f;
    public float panjang = 380f;
    public float lebar = 6.3f;   // setengah lebar
    public float tinggi = 4f;    // tinggi plafon
    public float jarakRusuk = 8f;
    Transform root;

    void OnEnable() { Bangun(); }

    void Hancur(Object o)
    {
        if (o == null) return;
        if (Application.isPlaying) Destroy(o); else DestroyImmediate(o);
    }

    void Bangun()
    {
        Transform lama = transform.Find("Terowongan");
        if (lama != null) Hancur(lama.gameObject);

        Shader s = Shader.Find("Universal Render Pipeline/Lit");
        if (s == null) s = Shader.Find("Standard");
        Material batu = Buat(s, new Color(0.14f, 0.14f, 0.16f), false);
        Material rusuk = Buat(s, new Color(0.1f, 0.1f, 0.11f), false);
        Material lampu = Buat(s, new Color(0.55f, 0.48f, 0.3f), true);
        Material lantai = Buat(s, new Color(0.2f, 0.19f, 0.17f), false);
        Material dinding = Buat(s, new Color(0.17f, 0.16f, 0.15f), false);
        Material garis = Buat(s, new Color(0.55f, 0.5f, 0.28f), true);

        GameObject vis = new GameObject("Terowongan");
        vis.hideFlags = HideFlags.DontSave;
        vis.transform.SetParent(transform, false);
        root = vis.transform;

        float tengahZ = mulaiZ + panjang * 0.5f;

        // lantai batu + dinding batu + garis jalur (beda dari rel kereta Level 2)
        Bag(new Vector3(0, 0f, tengahZ), new Vector3(lebar * 2f, 0.3f, panjang), lantai);
        Bag(new Vector3(-lebar, tinggi * 0.5f, tengahZ), new Vector3(0.5f, tinggi, panjang), dinding);
        Bag(new Vector3(lebar, tinggi * 0.5f, tengahZ), new Vector3(0.5f, tinggi, panjang), dinding);
        Bag(new Vector3(-1.5f, 0.16f, tengahZ), new Vector3(0.1f, 0.05f, panjang), garis);
        Bag(new Vector3(1.5f, 0.16f, tengahZ), new Vector3(0.1f, 0.05f, panjang), garis);

        // plafon menutup atas (biar gelap, tak tembus langit)
        Bag(new Vector3(0, tinggi, tengahZ), new Vector3(lebar * 2f, 0.4f, panjang), batu);
        // sudut atas kiri-kanan (kesan melengkung)
        Bag(new Vector3(-lebar + 0.5f, tinggi - 0.6f, tengahZ), new Vector3(1.4f, 0.4f, panjang), batu);
        Bag(new Vector3(lebar - 0.5f, tinggi - 0.6f, tengahZ), new Vector3(1.4f, 0.4f, panjang), batu);

        // rusuk + lampu redup tiap interval
        int n = Mathf.Max(1, Mathf.RoundToInt(panjang / jarakRusuk));
        for (int i = 0; i <= n; i++)
        {
            float z = mulaiZ + i * jarakRusuk;
            Bag(new Vector3(0, tinggi - 0.2f, z), new Vector3(lebar * 2f + 0.4f, 0.6f, 0.5f), rusuk);
            Bag(new Vector3(0, tinggi - 0.75f, z), new Vector3(0.5f, 0.18f, 0.5f), lampu);
        }
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
            m.SetColor("_EmissionColor", c * 1.6f);
        }
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
}
