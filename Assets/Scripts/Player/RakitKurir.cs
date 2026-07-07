using UnityEngine;

// Merakit tokoh KURIR dari primitif + animasi lari (kaki & lengan berayun, badan naik-turun).
// [ExecuteAlways] -> tampil di editor. Menyembunyikan kapsul asli.
[ExecuteAlways]
public class RakitKurir : MonoBehaviour
{
    Transform vis, hipKi, hipKa, bahuKi, bahuKa;
    Rigidbody rb;
    Vector3 posTerakhir;
    float gerak;

    void OnEnable()
    {
        Bangun();
        posTerakhir = transform.position;
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
        if (mr != null) mr.enabled = false; // sembunyikan kapsul

        Shader s = Shader.Find("Universal Render Pipeline/Lit");
        if (s == null) s = Shader.Find("Standard");

        Material baju = Buat(s, new Color(0.95f, 0.5f, 0.15f));
        Material celana = Buat(s, new Color(0.2f, 0.22f, 0.3f));
        Material sepatu = Buat(s, new Color(0.12f, 0.12f, 0.14f));
        Material kulit = Buat(s, new Color(0.95f, 0.78f, 0.6f));
        Material tas = Buat(s, new Color(0.78f, 0.6f, 0.38f));

        GameObject v = new GameObject("Visual");
        v.hideFlags = HideFlags.DontSave;
        v.transform.SetParent(transform, false);
        vis = v.transform;

        // badan
        Bag(vis, PrimitiveType.Cube, "Badan", new Vector3(0f, 0.15f, 0f), new Vector3(0.62f, 0.8f, 0.4f), baju);

        // lengan (berayun dari bahu)
        bahuKi = Pivot("BahuKiri", new Vector3(-0.42f, 0.5f, 0f));
        Bag(bahuKi, PrimitiveType.Cube, "LenganKiri", new Vector3(0f, -0.3f, 0f), new Vector3(0.18f, 0.7f, 0.24f), baju);
        bahuKa = Pivot("BahuKanan", new Vector3(0.42f, 0.5f, 0f));
        Bag(bahuKa, PrimitiveType.Cube, "LenganKanan", new Vector3(0f, -0.3f, 0f), new Vector3(0.18f, 0.7f, 0.24f), baju);

        // leher + kepala
        Bag(vis, PrimitiveType.Cube, "Leher", new Vector3(0f, 0.58f, 0f), new Vector3(0.2f, 0.16f, 0.2f), kulit);
        Bag(vis, PrimitiveType.Sphere, "Kepala", new Vector3(0f, 0.8f, 0f), new Vector3(0.5f, 0.55f, 0.5f), kulit);

        // TOPI: kubah membulat (bola pipih) + lidah topi di depan
        Bag(vis, PrimitiveType.Sphere, "TopiKubah", new Vector3(0f, 1.0f, 0f), new Vector3(0.58f, 0.42f, 0.58f), baju);
        Bag(vis, PrimitiveType.Cube, "TopiLidah", new Vector3(0f, 0.95f, 0.34f), new Vector3(0.5f, 0.06f, 0.32f), baju);

        // tas paket di punggung (-z, menghadap kamera)
        Bag(vis, PrimitiveType.Cube, "TasPaket", new Vector3(0f, 0.2f, -0.33f), new Vector3(0.5f, 0.62f, 0.32f), tas);

        // kaki (berayun dari pinggul)
        hipKi = Pivot("HipKiri", new Vector3(-0.18f, -0.1f, 0f));
        Bag(hipKi, PrimitiveType.Cube, "KakiKiri", new Vector3(0f, -0.45f, 0f), new Vector3(0.28f, 0.9f, 0.32f), celana);
        Bag(hipKi, PrimitiveType.Cube, "SepatuKiri", new Vector3(0f, -0.87f, 0.06f), new Vector3(0.3f, 0.18f, 0.44f), sepatu);
        hipKa = Pivot("HipKanan", new Vector3(0.18f, -0.1f, 0f));
        Bag(hipKa, PrimitiveType.Cube, "KakiKanan", new Vector3(0f, -0.45f, 0f), new Vector3(0.28f, 0.9f, 0.32f), celana);
        Bag(hipKa, PrimitiveType.Cube, "SepatuKanan", new Vector3(0f, -0.87f, 0.06f), new Vector3(0.3f, 0.18f, 0.44f), sepatu);
    }

    Transform Pivot(string n, Vector3 lp)
    {
        GameObject g = new GameObject(n);
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(vis, false);
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

    void Bag(Transform parent, PrimitiveType t, string n, Vector3 lp, Vector3 ls, Material m)
    {
        GameObject g = GameObject.CreatePrimitive(t);
        g.name = n;
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
        if (!Application.isPlaying || vis == null) return;

        float dt = Time.deltaTime;
        if (dt <= 0f) return;

        // seberapa cepat pemain bergerak (pakai kecepatan Rigidbody -> stabil) -> blend animasi lari
        if (rb == null) rb = GetComponent<Rigidbody>();
        float laju;
        if (rb != null)
        {
            Vector3 v = rb.linearVelocity; v.y = 0f;
            laju = v.magnitude;
        }
        else
        {
            laju = (transform.position - posTerakhir).magnitude / dt;
            posTerakhir = transform.position;
        }
        float target = laju > 0.6f ? 1f : 0f;
        gerak = Mathf.MoveTowards(gerak, target, dt * 5f);

        float t = Time.time * 10f;
        float ayun = Mathf.Sin(t) * 45f * gerak;
        if (hipKi) hipKi.localRotation = Quaternion.Euler(ayun, 0f, 0f);
        if (hipKa) hipKa.localRotation = Quaternion.Euler(-ayun, 0f, 0f);
        if (bahuKi) bahuKi.localRotation = Quaternion.Euler(-ayun, 0f, 0f);
        if (bahuKa) bahuKa.localRotation = Quaternion.Euler(ayun, 0f, 0f);
        vis.localPosition = new Vector3(0f, Mathf.Abs(Mathf.Sin(t)) * 0.06f * gerak, 0f);
    }
}
