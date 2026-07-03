using UnityEngine;

// Membangun diorama menu (5 kawasan) dari primitif + warna, via kode.
// [ExecuteAlways] -> tampil di editor (sebelum Play) juga. Objek DontSave (dibangun ulang tiap load, tidak dobel).
[ExecuteAlways]
public class MenuDiorama : MonoBehaviour
{
    Shader shaderLit;
    Transform root;

    void OnEnable()
    {
        Bangun();
    }

    void Hancur(Object o)
    {
        if (o == null) return;
        if (Application.isPlaying) Destroy(o); else DestroyImmediate(o);
    }

    void Bangun()
    {
        Transform lama = transform.Find("Diorama3D");
        if (lama != null) Hancur(lama.gameObject);

        shaderLit = Shader.Find("Universal Render Pipeline/Lit");
        if (shaderLit == null) shaderLit = Shader.Find("Standard");

        GameObject r = new GameObject("Diorama3D");
        r.hideFlags = HideFlags.DontSave;
        r.transform.SetParent(transform, false);
        root = r.transform;

        BuatTanah();
        BuatKota();
        BuatRelKereta();
        BuatPasar();
        BuatTerowongan();
        BuatIndustri();
        BuatMatahari();
    }

    Material Mat(Color c, float emisi = 0f)
    {
        Material m = new Material(shaderLit);
        m.hideFlags = HideFlags.DontSave;
        m.color = c;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
        if (emisi > 0f)
        {
            m.EnableKeyword("_EMISSION");
            m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            m.SetColor("_EmissionColor", c * emisi);
        }
        return m;
    }

    GameObject Kotak(string nama, Vector3 pos, Vector3 skala, Color warna, float emisi = 0f)
    {
        return Bentuk(PrimitiveType.Cube, nama, pos, skala, warna, emisi);
    }

    GameObject Bentuk(PrimitiveType t, string nama, Vector3 pos, Vector3 skala, Color warna, float emisi = 0f)
    {
        GameObject g = GameObject.CreatePrimitive(t);
        g.name = nama;
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(root, false);
        g.transform.localPosition = pos;
        g.transform.localScale = skala;
        g.GetComponent<Renderer>().sharedMaterial = Mat(warna, emisi);
        Collider c = g.GetComponent<Collider>();
        if (c != null) Hancur(c);
        return g;
    }

    void BuatTanah()
    {
        Kotak("Tanah", new Vector3(0, -0.5f, 4), new Vector3(48, 1, 22), new Color(0.45f, 0.7f, 0.35f));
        Kotak("Dasar", new Vector3(0, 0.02f, 4), new Vector3(46, 0.1f, 20), new Color(0.55f, 0.57f, 0.55f));
    }

    Color BiruGedung(int i)
    {
        Color[] c = {
            new Color(0.55f,0.6f,0.68f), new Color(0.4f,0.45f,0.55f),
            new Color(0.62f,0.64f,0.66f), new Color(0.35f,0.4f,0.48f)
        };
        return c[i % c.Length];
    }

    void BuatKota()
    {
        float[] tinggi = { 5, 7, 4, 6, 8 };
        for (int i = 0; i < tinggi.Length; i++)
        {
            float x = -20 + i * 2.4f;
            float h = tinggi[i];
            Kotak("Gedung" + i, new Vector3(x, h / 2f, 9), new Vector3(2.2f, h, 2.2f), BiruGedung(i));
            Kotak("Jdl" + i, new Vector3(x, h / 2f, 7.88f), new Vector3(1.6f, h * 0.8f, 0.05f), new Color(0.2f, 0.25f, 0.32f), 0.3f);
        }
        Kotak("JalanKota", new Vector3(-16, 0.12f, 4.5f), new Vector3(12, 0.12f, 3.2f), new Color(0.22f, 0.22f, 0.25f));
        Kotak("Mobil1", new Vector3(-18, 0.6f, 4.5f), new Vector3(1.6f, 0.9f, 0.9f), new Color(0.85f, 0.3f, 0.25f));
        Kotak("Mobil2", new Vector3(-14, 0.6f, 4.2f), new Vector3(1.6f, 0.9f, 0.9f), new Color(0.3f, 0.55f, 0.85f));
        Kotak("Krd1", new Vector3(-21, 0.5f, 4), new Vector3(1, 1, 1), new Color(0.78f, 0.6f, 0.36f));
        Kotak("Krd2", new Vector3(-21, 1.4f, 4), new Vector3(0.9f, 0.9f, 0.9f), new Color(0.82f, 0.64f, 0.4f));
    }

    void BuatRelKereta()
    {
        Kotak("RelDasar", new Vector3(-6, 0.14f, 12), new Vector3(14, 0.15f, 3), new Color(0.4f, 0.32f, 0.25f));
        Kotak("Rel1", new Vector3(-6, 0.25f, 11.2f), new Vector3(14, 0.12f, 0.2f), new Color(0.3f, 0.3f, 0.33f));
        Kotak("Rel2", new Vector3(-6, 0.25f, 12.8f), new Vector3(14, 0.12f, 0.2f), new Color(0.3f, 0.3f, 0.33f));
        Color kereta = new Color(0.35f, 0.42f, 0.55f);
        Kotak("Loko", new Vector3(-3, 0.9f, 12), new Vector3(3, 1.3f, 1.6f), kereta);
        Kotak("Gerbong1", new Vector3(-6.5f, 0.9f, 12), new Vector3(3, 1.3f, 1.6f), new Color(0.5f, 0.3f, 0.3f));
        Kotak("Gerbong2", new Vector3(-10, 0.9f, 12), new Vector3(3, 1.3f, 1.6f), new Color(0.3f, 0.5f, 0.4f));
        Kotak("TiangSinyal", new Vector3(-1, 1, 10.5f), new Vector3(0.2f, 2, 0.2f), new Color(0.9f, 0.9f, 0.9f));
        Kotak("LampuMerah", new Vector3(-1, 2, 10.5f), new Vector3(0.4f, 0.4f, 0.4f), new Color(0.9f, 0.2f, 0.2f), 0.6f);
    }

    void BuatPasar()
    {
        Kotak("AlasPasar", new Vector3(0, 0.13f, 3), new Vector3(9, 0.12f, 5), new Color(0.3f, 0.28f, 0.3f));
        Color[] kanopi = { new Color(0.85f,0.3f,0.3f), new Color(0.9f,0.8f,0.4f), new Color(0.3f,0.6f,0.8f) };
        for (int i = 0; i < 3; i++)
        {
            float x = -3 + i * 3f;
            Kotak("Meja" + i, new Vector3(x, 0.6f, 3), new Vector3(1.6f, 0.9f, 1.2f), new Color(0.6f, 0.42f, 0.28f));
            GameObject kan = Kotak("Kanopi" + i, new Vector3(x, 1.6f, 3), new Vector3(2f, 0.1f, 1.6f), kanopi[i]);
            kan.transform.localRotation = Quaternion.Euler(12, 0, 0);
            Kotak("Lampu" + i, new Vector3(x, 1.35f, 3), new Vector3(0.25f, 0.25f, 0.25f), new Color(1f, 0.8f, 0.4f), 1.2f);
        }
        Color[] baju = { new Color(0.9f,0.5f,0.4f), new Color(0.4f,0.6f,0.9f), new Color(0.5f,0.8f,0.5f), new Color(0.9f,0.85f,0.5f), new Color(0.7f,0.5f,0.8f) };
        System.Random rnd = new System.Random(7);
        for (int i = 0; i < 8; i++)
        {
            float x = -3.5f + (float)rnd.NextDouble() * 7f;
            float z = 1.2f + (float)rnd.NextDouble() * 3f;
            Bentuk(PrimitiveType.Capsule, "Orang" + i, new Vector3(x, 0.6f, z), new Vector3(0.32f, 0.5f, 0.32f), baju[i % baju.Length]);
        }
    }

    void BuatTerowongan()
    {
        Bentuk(PrimitiveType.Sphere, "Batu", new Vector3(9, 1.5f, 12), new Vector3(7, 5, 6), new Color(0.28f, 0.28f, 0.32f));
        Kotak("LubangTerowongan", new Vector3(9, 1.1f, 9.2f), new Vector3(2.4f, 2.2f, 1.2f), new Color(0.03f, 0.03f, 0.05f));
        Kotak("Truk", new Vector3(6.5f, 0.7f, 8), new Vector3(2.2f, 1.1f, 1.2f), new Color(0.85f, 0.75f, 0.4f));
    }

    void BuatIndustri()
    {
        Kotak("Pabrik", new Vector3(16, 2, 9), new Vector3(4.5f, 4, 4), new Color(0.5f, 0.53f, 0.58f));
        Kotak("AtapPabrik", new Vector3(16, 4.1f, 9), new Vector3(4.6f, 0.4f, 4.1f), new Color(0.35f, 0.37f, 0.4f));
        Kotak("CraneTiang", new Vector3(13, 3, 12), new Vector3(0.3f, 6, 0.3f), new Color(0.9f, 0.75f, 0.2f));
        Kotak("CraneLengan", new Vector3(14.5f, 5.8f, 12), new Vector3(4, 0.3f, 0.3f), new Color(0.9f, 0.75f, 0.2f));
        Kotak("CraneKabel", new Vector3(16.3f, 5f, 12), new Vector3(0.05f, 1.6f, 0.05f), new Color(0.1f, 0.1f, 0.1f));
        Kotak("CraneKait", new Vector3(16.3f, 4.1f, 12), new Vector3(0.6f, 0.6f, 0.6f), new Color(0.78f, 0.6f, 0.36f));
        Color[] kont = { new Color(0.8f,0.3f,0.25f), new Color(0.3f,0.5f,0.8f), new Color(0.4f,0.7f,0.45f), new Color(0.85f,0.7f,0.3f) };
        for (int i = 0; i < 4; i++)
        {
            int baris = i % 2, kolom = i / 2;
            Kotak("Kontainer" + i, new Vector3(19.5f, 0.7f + baris * 1.4f, 6 + kolom * 2.6f), new Vector3(2.6f, 1.3f, 2.2f), kont[i]);
        }
    }

    void BuatMatahari()
    {
        Bentuk(PrimitiveType.Sphere, "Matahari", new Vector3(15, 11, 14), new Vector3(3, 3, 3), new Color(1f, 0.85f, 0.25f), 1.4f);
    }
}
