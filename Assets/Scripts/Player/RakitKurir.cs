using UnityEngine;

// Merakit tokoh KURIR dari primitif + animasi lari (kaki & lengan berayun, badan naik-turun).
// [ExecuteAlways] -> tampil di editor. Menyembunyikan kapsul asli.
[ExecuteAlways]
public class RakitKurir : MonoBehaviour
{
    public bool bawaTas = false;   // tas paket hanya tampil setelah paket diambil
    Transform vis, hipKi, hipKa, bahuKi, bahuKa, tasObj;
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
        Material putih = Buat(s, Color.white);
        Material hitam = Buat(s, new Color(0.08f, 0.07f, 0.07f));
        Material mulut = Buat(s, new Color(0.55f, 0.28f, 0.26f));

        GameObject v = new GameObject("Visual");
        v.hideFlags = HideFlags.DontSave;
        v.transform.SetParent(transform, false);
        vis = v.transform;

        // badan & pinggul (kapsul membulat, bukan kotak)
        Bag(vis, PrimitiveType.Capsule, "Badan", new Vector3(0f, 0.15f, 0f), new Vector3(0.6f, 0.44f, 0.4f), baju);
        Bag(vis, PrimitiveType.Capsule, "Pinggul", new Vector3(0f, -0.15f, 0f), new Vector3(0.5f, 0.28f, 0.36f), celana);

        // lengan (berayun dari bahu) - kapsul + telapak tangan bola
        bahuKi = Pivot("BahuKiri", new Vector3(-0.42f, 0.5f, 0f));
        Bag(bahuKi, PrimitiveType.Capsule, "LenganKiri", new Vector3(0f, -0.3f, 0f), new Vector3(0.18f, 0.38f, 0.2f), baju);
        Bag(bahuKi, PrimitiveType.Sphere, "TanganKiri", new Vector3(0f, -0.62f, 0f), new Vector3(0.17f, 0.17f, 0.17f), kulit);
        bahuKa = Pivot("BahuKanan", new Vector3(0.42f, 0.5f, 0f));
        Bag(bahuKa, PrimitiveType.Capsule, "LenganKanan", new Vector3(0f, -0.3f, 0f), new Vector3(0.18f, 0.38f, 0.2f), baju);
        Bag(bahuKa, PrimitiveType.Sphere, "TanganKanan", new Vector3(0f, -0.62f, 0f), new Vector3(0.17f, 0.17f, 0.17f), kulit);

        // leher + kepala
        Bag(vis, PrimitiveType.Cylinder, "Leher", new Vector3(0f, 0.56f, 0f), new Vector3(0.16f, 0.1f, 0.16f), kulit);
        Bag(vis, PrimitiveType.Sphere, "Kepala", new Vector3(0f, 0.8f, 0f), new Vector3(0.5f, 0.55f, 0.5f), kulit);

        // wajah (mata + hidung + mulut) menghadap +z
        Bag(vis, PrimitiveType.Sphere, "MataKiP", new Vector3(-0.11f, 0.85f, 0.19f), new Vector3(0.13f, 0.16f, 0.09f), putih);
        Bag(vis, PrimitiveType.Sphere, "MataKaP", new Vector3(0.11f, 0.85f, 0.19f), new Vector3(0.13f, 0.16f, 0.09f), putih);
        Bag(vis, PrimitiveType.Sphere, "MataKi", new Vector3(-0.11f, 0.85f, 0.24f), new Vector3(0.07f, 0.08f, 0.05f), hitam);
        Bag(vis, PrimitiveType.Sphere, "MataKa", new Vector3(0.11f, 0.85f, 0.24f), new Vector3(0.07f, 0.08f, 0.05f), hitam);
        Bag(vis, PrimitiveType.Sphere, "Hidung", new Vector3(0f, 0.78f, 0.25f), new Vector3(0.09f, 0.1f, 0.12f), kulit);
        Bag(vis, PrimitiveType.Cube, "Mulut", new Vector3(0f, 0.7f, 0.23f), new Vector3(0.16f, 0.04f, 0.06f), mulut);

        // TOPI kurir: kubah membulat (bola pipih) + lidah topi di depan
        Bag(vis, PrimitiveType.Sphere, "TopiKubah", new Vector3(0f, 1.0f, 0f), new Vector3(0.58f, 0.42f, 0.58f), baju);
        Bag(vis, PrimitiveType.Cube, "TopiLidah", new Vector3(0f, 0.95f, 0.34f), new Vector3(0.5f, 0.06f, 0.32f), baju);

        // tas paket di punggung (-z, menghadap kamera) - tampil hanya jika sudah bawa paket
        Bag(vis, PrimitiveType.Cube, "TasPaket", new Vector3(0f, 0.2f, -0.33f), new Vector3(0.5f, 0.62f, 0.32f), tas);
        tasObj = vis.Find("TasPaket");
        if (tasObj != null) tasObj.gameObject.SetActive(bawaTas);

        // kaki (berayun dari pinggul) - kapsul + sepatu
        hipKi = Pivot("HipKiri", new Vector3(-0.18f, -0.1f, 0f));
        Bag(hipKi, PrimitiveType.Capsule, "KakiKiri", new Vector3(0f, -0.45f, 0f), new Vector3(0.28f, 0.46f, 0.3f), celana);
        Bag(hipKi, PrimitiveType.Cube, "SepatuKiri", new Vector3(0f, -0.87f, 0.06f), new Vector3(0.3f, 0.18f, 0.44f), sepatu);
        hipKa = Pivot("HipKanan", new Vector3(0.18f, -0.1f, 0f));
        Bag(hipKa, PrimitiveType.Capsule, "KakiKanan", new Vector3(0f, -0.45f, 0f), new Vector3(0.28f, 0.46f, 0.3f), celana);
        Bag(hipKa, PrimitiveType.Cube, "SepatuKanan", new Vector3(0f, -0.87f, 0.06f), new Vector3(0.3f, 0.18f, 0.44f), sepatu);
    }

    // dipanggil saat paket diambil -> tampilkan/sembunyikan tas paket
    public void SetBawaTas(bool b)
    {
        bawaTas = b;
        if (tasObj == null && vis != null) tasObj = vis.Find("TasPaket");
        if (tasObj != null) tasObj.gameObject.SetActive(b);
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
