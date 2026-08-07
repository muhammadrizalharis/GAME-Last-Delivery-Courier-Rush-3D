using UnityEngine;

// Membangun suasana KAWASAN INDUSTRI: lantai logam + dinding + pipa memanjang + mesin di sisi
// + tangki/drum/peti + garis bahaya. [ExecuteAlways]. Beda dari level lain (metalik & dingin).
[ExecuteAlways]
public class RakitPabrik : MonoBehaviour
{
    public float mulaiZ = -20f;
    public float panjang = 380f;
    public float lebar = 6.3f;
    public float jarakMesin = 22f;
    Transform root;

    void OnEnable() { Bangun(); }

    void Hancur(Object o)
    {
        if (o == null) return;
        if (Application.isPlaying) Destroy(o); else DestroyImmediate(o);
    }

    void Bangun()
    {
        Transform lama = transform.Find("Pabrik");
        if (lama) Hancur(lama.gameObject);

        Shader s = Shader.Find("Universal Render Pipeline/Lit");
        if (s == null) s = Shader.Find("Standard");
        Material lantai = Buat(s, new Color(0.2f, 0.2f, 0.23f), false);
        Material dinding = Buat(s, new Color(0.16f, 0.16f, 0.19f), false);
        Material mesin = Buat(s, new Color(0.3f, 0.3f, 0.34f), false);
        Material pipa = Buat(s, new Color(0.45f, 0.32f, 0.18f), false);
        Material tangki = Buat(s, new Color(0.5f, 0.52f, 0.56f), false);
        Material drumMat = Buat(s, new Color(0.72f, 0.34f, 0.12f), false);
        Material kayu = Buat(s, new Color(0.5f, 0.36f, 0.2f), false);
        Material girder = Buat(s, new Color(0.26f, 0.26f, 0.3f), false);
        Material hazard = Buat(s, new Color(0.92f, 0.74f, 0.1f), true);

        GameObject vis = new GameObject("Pabrik");
        vis.hideFlags = HideFlags.DontSave;
        vis.transform.SetParent(transform, false);
        root = vis.transform;

        float tengahZ = mulaiZ + panjang * 0.5f;
        Bag(new Vector3(0, 0f, tengahZ), new Vector3(lebar * 2f, 0.2f, panjang), lantai);
        Bag(new Vector3(-lebar, 2.2f, tengahZ), new Vector3(0.5f, 4.4f, panjang), dinding);
        Bag(new Vector3(lebar, 2.2f, tengahZ), new Vector3(0.5f, 4.4f, panjang), dinding);
        Bag(new Vector3(-lebar + 0.45f, 3f, tengahZ), new Vector3(0.25f, 0.25f, panjang), pipa);
        Bag(new Vector3(lebar - 0.45f, 3f, tengahZ), new Vector3(0.25f, 0.25f, panjang), pipa);
        // garis pengaman kuning-hitam di tepi lantai (kiri & kanan)
        Bag(new Vector3(-lebar + 1.6f, 0.12f, tengahZ), new Vector3(0.25f, 0.06f, panjang), hazard);
        Bag(new Vector3(lebar - 1.6f, 0.12f, tengahZ), new Vector3(0.25f, 0.06f, panjang), hazard);

        int n = Mathf.Max(1, Mathf.RoundToInt(panjang / jarakMesin));
        for (int i = 0; i <= n; i++)
        {
            float z = mulaiZ + i * jarakMesin;
            Bag(new Vector3(-lebar + 0.8f, 1f, z), new Vector3(1.3f, 2f, 1.7f), mesin);
            Bag(new Vector3(lebar - 0.8f, 1f, z), new Vector3(1.3f, 2f, 1.7f), mesin);

            if (i % 2 == 0)
            {
                float sisi = (i % 4 == 0) ? -1f : 1f;
                float zt = z + jarakMesin * 0.5f;
                // tangki penyimpanan besar + tutup kubah + pipa penghubung
                BagJenis(PrimitiveType.Cylinder, new Vector3(sisi * (lebar - 0.9f), 2f, zt), new Vector3(1.4f, 2f, 1.4f), Quaternion.identity, tangki);
                BagJenis(PrimitiveType.Sphere, new Vector3(sisi * (lebar - 0.9f), 4f, zt), new Vector3(1.4f, 0.7f, 1.4f), Quaternion.identity, tangki);
                BagJenis(PrimitiveType.Cylinder, new Vector3(sisi * (lebar - 0.45f), 3f, zt), new Vector3(0.18f, 0.9f, 0.18f), Quaternion.Euler(0, 0, 90), pipa);
                // tumpukan peti kayu + 2 drum oli di sisi seberang
                float s2 = -sisi;
                BagJenis(PrimitiveType.Cube, new Vector3(s2 * (lebar - 1.0f), 0.6f, zt), new Vector3(1f, 1f, 1f), Quaternion.identity, kayu);
                BagJenis(PrimitiveType.Cube, new Vector3(s2 * (lebar - 1.0f), 1.45f, zt), new Vector3(0.85f, 0.7f, 0.85f), Quaternion.identity, kayu);
                BagJenis(PrimitiveType.Cylinder, new Vector3(s2 * (lebar - 1.9f), 0.55f, zt - 0.8f), new Vector3(0.6f, 0.55f, 0.6f), Quaternion.identity, drumMat);
                BagJenis(PrimitiveType.Cylinder, new Vector3(s2 * (lebar - 1.9f), 0.55f, zt + 0.5f), new Vector3(0.6f, 0.55f, 0.6f), Quaternion.identity, drumMat);
                // girder rangka baja melintang di atas lintasan
                BagJenis(PrimitiveType.Cube, new Vector3(0, 4.7f, z), new Vector3(lebar * 2f, 0.28f, 0.32f), Quaternion.identity, girder);
            }
        }
    }

    void BagJenis(PrimitiveType t, Vector3 lp, Vector3 ls, Quaternion rot, Material m)
    {
        GameObject g = GameObject.CreatePrimitive(t);
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.localPosition = lp;
        g.transform.localScale = ls;
        g.transform.localRotation = rot;
        g.GetComponent<Renderer>().sharedMaterial = m;
        Collider c = g.GetComponent<Collider>();
        if (c != null) Hancur(c);
    }

    Material Buat(Shader s, Color c, bool emisi)
    {
        Material m = new Material(s) { hideFlags = HideFlags.DontSave };
        m.color = c;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
        if (m.HasProperty("_Metallic")) m.SetFloat("_Metallic", 0.6f);
        if (emisi && m.HasProperty("_EmissionColor")) { m.EnableKeyword("_EMISSION"); m.SetColor("_EmissionColor", c * 1.8f); }
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
