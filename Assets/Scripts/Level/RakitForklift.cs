using UnityEngine;

// Merakit FORKLIFT INDUSTRI dari primitif (pengganti lokomotif di level pabrik).
// Kabin + rangka pelindung + tiang angkat (mast) + garpu + muatan peti/drum + lampu beacon oranye.
// [ExecuteAlways] -> tampil di editor. warna dipakai untuk drum muatan (variasi tiap unit).
[ExecuteAlways]
public class RakitForklift : MonoBehaviour
{
    public Color warna = new Color(0.15f, 0.35f, 0.7f); // warna drum muatan
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

        Material kuning = Buat(s, new Color(0.86f, 0.66f, 0.08f));
        Material kuningTua = Buat(s, new Color(0.6f, 0.46f, 0.05f));
        Material gelap = Buat(s, new Color(0.12f, 0.12f, 0.14f));
        Material logam = Buat(s, new Color(0.55f, 0.57f, 0.62f));
        Material kayu = Buat(s, new Color(0.55f, 0.38f, 0.19f));
        Material drum = Buat(s, warna);
        Material beacon = Buat(s, new Color(1f, 0.45f, 0.05f), true);
        Material lampu = Buat(s, new Color(1f, 0.95f, 0.7f), true);

        // sasis utama + pemberat belakang
        Bag(PrimitiveType.Cube, "Sasis", new Vector3(0, 0.55f, 0.2f), new Vector3(1.7f, 0.8f, 2.3f), Quaternion.identity, kuning);
        Bag(PrimitiveType.Cube, "Pemberat", new Vector3(0, 0.75f, 1.35f), new Vector3(1.8f, 1.15f, 0.7f), Quaternion.identity, kuningTua);
        // kursi + sandaran
        Bag(PrimitiveType.Cube, "Kursi", new Vector3(0, 1.1f, 0.95f), new Vector3(0.8f, 0.5f, 0.7f), Quaternion.identity, gelap);
        Bag(PrimitiveType.Cube, "Sandaran", new Vector3(0, 1.45f, 1.28f), new Vector3(0.8f, 0.6f, 0.16f), Quaternion.identity, gelap);

        // rangka pelindung kabin (4 tiang + atap)
        Bag(PrimitiveType.Cube, "Tiang1", new Vector3(-0.72f, 1.7f, 0.35f), new Vector3(0.13f, 1.8f, 0.13f), Quaternion.identity, gelap);
        Bag(PrimitiveType.Cube, "Tiang2", new Vector3(0.72f, 1.7f, 0.35f), new Vector3(0.13f, 1.8f, 0.13f), Quaternion.identity, gelap);
        Bag(PrimitiveType.Cube, "Tiang3", new Vector3(-0.72f, 1.7f, 1.25f), new Vector3(0.13f, 1.8f, 0.13f), Quaternion.identity, gelap);
        Bag(PrimitiveType.Cube, "Tiang4", new Vector3(0.72f, 1.7f, 1.25f), new Vector3(0.13f, 1.8f, 0.13f), Quaternion.identity, gelap);
        Bag(PrimitiveType.Cube, "Atap", new Vector3(0, 2.6f, 0.8f), new Vector3(1.7f, 0.13f, 1.9f), Quaternion.identity, gelap);

        // tiang angkat (mast) di depan + pelat penahan garpu
        Bag(PrimitiveType.Cube, "MastKi", new Vector3(-0.45f, 1.55f, -0.95f), new Vector3(0.16f, 2.9f, 0.16f), Quaternion.identity, logam);
        Bag(PrimitiveType.Cube, "MastKa", new Vector3(0.45f, 1.55f, -0.95f), new Vector3(0.16f, 2.9f, 0.16f), Quaternion.identity, logam);
        Bag(PrimitiveType.Cube, "MastAtas", new Vector3(0, 3.0f, -0.95f), new Vector3(1.1f, 0.16f, 0.16f), Quaternion.identity, logam);
        Bag(PrimitiveType.Cube, "Pelat", new Vector3(0, 0.75f, -1.05f), new Vector3(1.15f, 1.1f, 0.12f), Quaternion.identity, logam);

        // garpu (2 prong ke depan -Z)
        Bag(PrimitiveType.Cube, "GarpuKi", new Vector3(-0.4f, 0.2f, -1.75f), new Vector3(0.22f, 0.12f, 1.5f), Quaternion.identity, logam);
        Bag(PrimitiveType.Cube, "GarpuKa", new Vector3(0.4f, 0.2f, -1.75f), new Vector3(0.22f, 0.12f, 1.5f), Quaternion.identity, logam);

        // muatan: peti kayu bertumpuk + drum di atas garpu
        Bag(PrimitiveType.Cube, "Peti1", new Vector3(0, 0.65f, -1.7f), new Vector3(1.05f, 0.85f, 1.05f), Quaternion.identity, kayu);
        Bag(PrimitiveType.Cube, "Peti2", new Vector3(-0.02f, 1.45f, -1.68f), new Vector3(0.9f, 0.7f, 0.9f), Quaternion.identity, kayu);
        Bag(PrimitiveType.Cylinder, "Drum", new Vector3(0f, 2.1f, -1.68f), new Vector3(0.55f, 0.35f, 0.55f), Quaternion.identity, drum);

        // lampu beacon oranye di atap
        Bag(PrimitiveType.Cylinder, "BeaconDudukan", new Vector3(0, 2.72f, 0.05f), new Vector3(0.18f, 0.06f, 0.18f), Quaternion.identity, gelap);
        Bag(PrimitiveType.Sphere, "Beacon", new Vector3(0, 2.86f, 0.05f), new Vector3(0.28f, 0.3f, 0.28f), Quaternion.identity, beacon);

        // lampu depan kecil (kiri-kanan)
        Bag(PrimitiveType.Cube, "LampuKi", new Vector3(-0.55f, 0.6f, -1.18f), new Vector3(0.2f, 0.2f, 0.08f), Quaternion.identity, lampu);
        Bag(PrimitiveType.Cube, "LampuKa", new Vector3(0.55f, 0.6f, -1.18f), new Vector3(0.2f, 0.2f, 0.08f), Quaternion.identity, lampu);

        // roda (4) - silinder diputar Z 90
        Roda("RodaDepanKi", new Vector3(-0.82f, 0.4f, -0.55f), gelap);
        Roda("RodaDepanKa", new Vector3(0.82f, 0.4f, -0.55f), gelap);
        Roda("RodaBelakangKi", new Vector3(-0.78f, 0.32f, 1.15f), gelap);
        Roda("RodaBelakangKa", new Vector3(0.78f, 0.32f, 1.15f), gelap);
    }

    Material Buat(Shader s, Color c, bool emisi = false)
    {
        Material m = new Material(s);
        m.hideFlags = HideFlags.DontSave;
        m.color = c;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
        if (m.HasProperty("_Metallic")) m.SetFloat("_Metallic", 0.5f);
        if (emisi && m.HasProperty("_EmissionColor")) { m.EnableKeyword("_EMISSION"); m.SetColor("_EmissionColor", c * 2f); }
        return m;
    }

    void Bag(PrimitiveType t, string n, Vector3 lp, Vector3 ls, Quaternion rot, Material m)
    {
        GameObject g = GameObject.CreatePrimitive(t);
        g.name = n;
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.localPosition = lp;
        g.transform.localScale = ls;
        g.transform.localRotation = rot;
        g.GetComponent<Renderer>().sharedMaterial = m;
        Collider c = g.GetComponent<Collider>();
        if (c != null) Hancur(c);
    }

    void Roda(string n, Vector3 lp, Material m)
    {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        g.name = n;
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.localPosition = lp;
        g.transform.localScale = new Vector3(0.75f, 0.14f, 0.75f);
        g.transform.localRotation = Quaternion.Euler(0, 0, 90);
        g.GetComponent<Renderer>().sharedMaterial = m;
        Collider c = g.GetComponent<Collider>();
        if (c != null) Hancur(c);
    }
}
