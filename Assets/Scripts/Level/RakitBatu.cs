using UnityEngine;

// Merakit BATU (bongkahan) dari primitif. [ExecuteAlways] -> tampil di editor.
[ExecuteAlways]
public class RakitBatu : MonoBehaviour
{
    public Color warna = new Color(0.5f, 0.5f, 0.53f);
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

        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr != null) mr.enabled = false;

        Shader s = Shader.Find("Universal Render Pipeline/Lit");
        if (s == null) s = Shader.Find("Standard");

        GameObject vis = new GameObject("Visual");
        vis.hideFlags = HideFlags.DontSave;
        vis.transform.SetParent(transform, false);
        root = vis.transform;

        // palet batu lapuk: terang (kena cahaya) -> mid -> gelap (celah) supaya ada kesan bentuk
        Material terang = Buat(s, Warnai(warna, 1.15f));
        Material mid = Buat(s, warna);
        Material gelap = Buat(s, Warnai(warna, 0.72f));
        Material celah = Buat(s, Warnai(warna, 0.5f));
        Material[] acak = { mid, gelap, terang, celah };

        System.Random rnd = new System.Random(Mathf.Abs(GetInstanceID()) % 99991 + 1);

        // batu yang MENGGELINDING (Level 3) dirakit padat & seimbang di sekitar pusat -> putaran wajar
        if (GetComponent<BatuGelinding>() != null)
        {
            Bongkah(new Vector3(0f, 0.25f, 0f), new Vector3(1.55f, 1.4f, 1.5f), Tilt(rnd, 25f), mid);
            int ng = 9 + rnd.Next(4);
            for (int i = 0; i < ng; i++)
            {
                float a = (float)(rnd.NextDouble() * 6.2832);
                float rad = 0.45f + (float)rnd.NextDouble() * 0.6f;
                Vector3 p = new Vector3(Mathf.Cos(a) * rad, 0.05f + (float)rnd.NextDouble() * 0.9f, Mathf.Sin(a) * rad);
                float b = 0.5f + (float)rnd.NextDouble() * 0.7f;
                Vector3 sc = new Vector3(b * (0.85f + (float)rnd.NextDouble() * 0.4f), b * (0.8f + (float)rnd.NextDouble() * 0.35f), b * (0.85f + (float)rnd.NextDouble() * 0.4f));
                Bongkah(p, sc, Tilt(rnd, 45f), acak[rnd.Next(acak.Length)]);
            }
            return;
        }

        // batu DIAM (penghalang): membumi -> alas tanah gelap, inti besar, punggung terang, kerikil di kaki
        Bongkah(new Vector3(0f, -0.16f, 0f), new Vector3(2.2f, 0.34f, 2.0f), Tilt(rnd, 6f), Buat(s, new Color(0.26f, 0.24f, 0.21f)));
        Bongkah(new Vector3(0f, 0.5f, 0f), new Vector3(1.7f, 1.45f, 1.6f), Tilt(rnd, 15f), mid);
        Bongkah(new Vector3(-0.12f, 1.0f, 0.06f), new Vector3(1.2f, 0.8f, 1.15f), Tilt(rnd, 22f), terang);
        int nMid = 6 + rnd.Next(3);
        for (int i = 0; i < nMid; i++)
        {
            float a = (float)(i / (float)nMid * 6.2832 + rnd.NextDouble() * 0.6);
            float rad = 0.62f + (float)rnd.NextDouble() * 0.32f;
            float y = 0.35f + (float)rnd.NextDouble() * 0.72f;
            Vector3 p = new Vector3(Mathf.Cos(a) * rad, y, Mathf.Sin(a) * rad);
            float b = 0.6f + (float)rnd.NextDouble() * 0.45f;
            Vector3 sc = new Vector3(b * (0.9f + (float)rnd.NextDouble() * 0.3f), b * (0.8f + (float)rnd.NextDouble() * 0.3f), b * (0.9f + (float)rnd.NextDouble() * 0.3f));
            Material m = y > 0.85f ? terang : (y < 0.5f ? gelap : mid);
            Bongkah(p, sc, Tilt(rnd, 38f), m);
        }
        int nKecil = 5 + rnd.Next(4);
        for (int i = 0; i < nKecil; i++)
        {
            float a = (float)(rnd.NextDouble() * 6.2832);
            float rad = 0.85f + (float)rnd.NextDouble() * 0.5f;
            Vector3 p = new Vector3(Mathf.Cos(a) * rad, 0.02f + (float)rnd.NextDouble() * 0.16f, Mathf.Sin(a) * rad);
            float b = 0.22f + (float)rnd.NextDouble() * 0.28f;
            Vector3 sc = new Vector3(b, b * 0.65f, b * (0.85f + (float)rnd.NextDouble() * 0.4f));
            Bongkah(p, sc, Tilt(rnd, 60f), rnd.Next(2) == 0 ? gelap : celah);
        }
    }

    Color Warnai(Color c, float f)
    {
        return new Color(Mathf.Clamp01(c.r * f), Mathf.Clamp01(c.g * f), Mathf.Clamp01(c.b * f), 1f);
    }

    Vector3 Tilt(System.Random r, float maks)
    {
        return new Vector3(((float)r.NextDouble() * 2f - 1f) * maks, r.Next(360), ((float)r.NextDouble() * 2f - 1f) * maks);
    }

    Material Buat(Shader s, Color c)
    {
        Material m = new Material(s);
        m.hideFlags = HideFlags.DontSave;
        m.color = c;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
        if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", 0.08f);
        if (m.HasProperty("_Glossiness")) m.SetFloat("_Glossiness", 0.08f);
        if (m.HasProperty("_Metallic")) m.SetFloat("_Metallic", 0f);
        return m;
    }

    void Bongkah(Vector3 lp, Vector3 ls, Vector3 rot, Material m)
    {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.localPosition = lp;
        g.transform.localScale = ls;
        g.transform.localRotation = Quaternion.Euler(rot);
        g.GetComponent<Renderer>().sharedMaterial = m;
        Collider c = g.GetComponent<Collider>();
        if (c != null) Hancur(c);
    }
}
