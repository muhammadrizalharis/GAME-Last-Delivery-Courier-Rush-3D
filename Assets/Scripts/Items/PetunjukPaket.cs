using UnityEngine;

// Petunjuk ambil paket: tulisan "PAKET" huruf-balok NEMPEL di box + PANAH memantul & berputar di atasnya.
// Sembunyi otomatis saat paket sudah diambil (MeshRenderer host dimatikan PaketFinish). Kompensasi skala induk.
[ExecuteAlways]
public class PetunjukPaket : MonoBehaviour
{
    public float hadapX = -1f;    // sisi box yang menghadap lorong/pemain
    Transform holder, panah;
    MeshRenderer hostMr;

    void OnEnable() { hostMr = GetComponent<MeshRenderer>(); Bangun(); }

    void Hancur(Object o) { if (o == null) return; if (Application.isPlaying) Destroy(o); else DestroyImmediate(o); }

    void Bangun()
    {
        Transform lama = transform.Find("Petunjuk");
        if (lama != null) Hancur(lama.gameObject);

        GameObject r = new GameObject("Petunjuk");
        r.hideFlags = HideFlags.DontSave;
        r.transform.SetParent(transform, false);
        float inv = 1f / Mathf.Max(0.01f, transform.lossyScale.x);   // batalkan skala box kecil -> ukuran dunia tetap
        r.transform.localScale = new Vector3(inv, inv, inv);
        holder = r.transform;

        int hd = hadapX >= 0 ? 1 : -1;
        HurufBalok.Tulis(holder, "PAKET", new Vector3(hd * 0.31f, 0.02f, 0f), 0.28f, hd, new Color(0.12f, 0.1f, 0.08f));

        Shader s = Shader.Find("Universal Render Pipeline/Lit");
        if (s == null) s = Shader.Find("Standard");
        Material m = new Material(s) { hideFlags = HideFlags.DontSave };
        Color kuning = new Color(1f, 0.82f, 0.1f);
        m.color = kuning;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", kuning);
        if (m.HasProperty("_EmissionColor")) { m.EnableKeyword("_EMISSION"); m.SetColor("_EmissionColor", kuning * 1.6f); }

        GameObject p = new GameObject("Panah");
        p.hideFlags = HideFlags.DontSave;
        p.transform.SetParent(holder, false);
        p.transform.localPosition = new Vector3(0f, 0.85f, 0f);
        panah = p.transform;
        Bag(p.transform, new Vector3(0f, 0.32f, 0f), new Vector3(0.16f, 0.44f, 0.16f), m, Quaternion.identity);        // batang
        Bag(p.transform, new Vector3(0f, 0.02f, 0f), new Vector3(0.4f, 0.4f, 0.16f), m, Quaternion.Euler(0f, 0f, 45f)); // kepala (menunjuk bawah)
    }

    void Bag(Transform parent, Vector3 lp, Vector3 ls, Material m, Quaternion rot)
    {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(parent, false);
        g.transform.localPosition = lp; g.transform.localScale = ls; g.transform.localRotation = rot;
        g.GetComponent<Renderer>().sharedMaterial = m;
        Collider c = g.GetComponent<Collider>();
        if (c != null) Hancur(c);
    }

    void Update()
    {
        if (transform.Find("Petunjuk") == null) { Bangun(); return; }
        if (!Application.isPlaying) return;
        if (hostMr != null && holder != null && holder.gameObject.activeSelf != hostMr.enabled)
            holder.gameObject.SetActive(hostMr.enabled);          // ikut hilang saat paket diambil
        if (panah == null || (holder != null && !holder.gameObject.activeSelf)) return;
        float b = Mathf.Abs(Mathf.Sin(Time.time * 3f)) * 0.45f;
        panah.localPosition = new Vector3(0f, 0.85f + b, 0f);
        panah.localRotation = Quaternion.Euler(0f, Time.time * 120f, 0f);
    }
}
