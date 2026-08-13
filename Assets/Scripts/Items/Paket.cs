using UnityEngine;

// PAKET yang WAJIB diambil sebelum finish. Saat diambil: +50 skor & kecepatan pemain turun.
// Dibangun dari primitif (kardus + lakban + penanda). Play-only interaksinya.
[ExecuteAlways]
public class Paket : MonoBehaviour
{
    Transform vis;
    bool diambil = false;

    void OnEnable() { Bangun(); }

    void Bangun()
    {
        Transform lama = transform.Find("VisualPaket");
        if (lama != null) { if (Application.isPlaying) Destroy(lama.gameObject); else DestroyImmediate(lama.gameObject); }

        Shader s = Shader.Find("Universal Render Pipeline/Lit");
        if (s == null) s = Shader.Find("Standard");
        Material kardus = Buat(s, new Color(0.72f, 0.53f, 0.32f), false);
        Material lakban = Buat(s, new Color(0.85f, 0.78f, 0.6f), false);
        Material tanda = Buat(s, new Color(0.95f, 0.45f, 0.15f), true);

        GameObject g = new GameObject("VisualPaket");
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(transform, false);
        vis = g.transform;

        Bag(PrimitiveType.Cube, new Vector3(0, 0, 0), new Vector3(1.1f, 1.1f, 1.1f), kardus);
        Bag(PrimitiveType.Cube, new Vector3(0, 0.56f, 0), new Vector3(1.14f, 0.06f, 0.3f), lakban);
        Bag(PrimitiveType.Cube, new Vector3(0, 0.56f, 0), new Vector3(0.3f, 0.06f, 1.14f), lakban);
        Bag(PrimitiveType.Sphere, new Vector3(0, 0.92f, 0), new Vector3(0.32f, 0.32f, 0.32f), tanda);
    }

    void Update()
    {
        if (!Application.isPlaying || vis == null) return;
        vis.localRotation = Quaternion.Euler(0, Time.time * 60f, 0);
        vis.localPosition = new Vector3(0, Mathf.Sin(Time.time * 2f) * 0.15f, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (diambil || !other.CompareTag("Player")) return;
        diambil = true;
        if (RunGame.instance != null) RunGame.instance.AmbilPaket();
        Kilau.Ledak(transform.position, new Color(0.95f, 0.6f, 0.2f));
        Destroy(gameObject);
    }

    Material Buat(Shader s, Color c, bool emisi)
    {
        Material m = new Material(s) { hideFlags = HideFlags.DontSave };
        m.color = c;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
        if (emisi && m.HasProperty("_EmissionColor")) { m.EnableKeyword("_EMISSION"); m.SetColor("_EmissionColor", c * 1.6f); }
        return m;
    }

    void Bag(PrimitiveType t, Vector3 lp, Vector3 ls, Material m)
    {
        GameObject g = GameObject.CreatePrimitive(t);
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(vis, false);
        g.transform.localPosition = lp;
        g.transform.localScale = ls;
        g.GetComponent<Renderer>().sharedMaterial = m;
        Collider c = g.GetComponent<Collider>();
        if (c != null) { if (Application.isPlaying) Destroy(c); else DestroyImmediate(c); }
    }
}
