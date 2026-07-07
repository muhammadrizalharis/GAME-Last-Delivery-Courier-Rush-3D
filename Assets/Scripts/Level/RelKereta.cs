using UnityEngine;

// Membangun REL KERETA (kerikil + bantalan + rel besi) dari primitif. [ExecuteAlways].
[ExecuteAlways]
public class RelKereta : MonoBehaviour
{
    public float mulaiZ = -20f;
    public float panjang = 380f;
    public float lebar = 6f;           // setengah lebar tanah kerikil
    public float jarakBantalan = 6f;   // jarak antar bantalan kayu
    public float[] jalur = { -3f, 0f, 3f };
    Transform root;

    void OnEnable() { Bangun(); }

    void Hancur(Object o)
    {
        if (o == null) return;
        if (Application.isPlaying) Destroy(o); else DestroyImmediate(o);
    }

    void Bangun()
    {
        Transform lama = transform.Find("Rel");
        if (lama != null) Hancur(lama.gameObject);

        Shader s = Shader.Find("Universal Render Pipeline/Lit");
        if (s == null) s = Shader.Find("Standard");
        Material kerikil = Buat(s, new Color(0.33f, 0.32f, 0.3f));
        Material kayu = Buat(s, new Color(0.32f, 0.22f, 0.14f));
        Material besi = Buat(s, new Color(0.62f, 0.64f, 0.68f));

        GameObject vis = new GameObject("Rel");
        vis.hideFlags = HideFlags.DontSave;
        vis.transform.SetParent(transform, false);
        root = vis.transform;

        float tengahZ = mulaiZ + panjang * 0.5f;

        // dasar kerikil (permukaan atas ~ y=0.15)
        Bag(new Vector3(0, 0f, tengahZ), new Vector3(lebar * 2f, 0.3f, panjang), kerikil);

        // bantalan kayu melintang
        int n = Mathf.Max(1, Mathf.RoundToInt(panjang / jarakBantalan));
        for (int i = 0; i <= n; i++)
        {
            float z = mulaiZ + i * jarakBantalan;
            Bag(new Vector3(0, 0.17f, z), new Vector3(lebar * 2f - 1f, 0.12f, 0.6f), kayu);
        }

        // rel besi (2 batang per jalur)
        if (jalur != null)
        {
            foreach (float x in jalur)
            {
                Bag(new Vector3(x - 0.7f, 0.28f, tengahZ), new Vector3(0.12f, 0.14f, panjang), besi);
                Bag(new Vector3(x + 0.7f, 0.28f, tengahZ), new Vector3(0.12f, 0.14f, panjang), besi);
            }
        }
    }

    Material Buat(Shader s, Color c)
    {
        Material m = new Material(s);
        m.hideFlags = HideFlags.DontSave;
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
}
